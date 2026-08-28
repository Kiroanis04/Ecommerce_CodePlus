using ECommerce.Application.Orders.DTOs;
using ECommerce.Application.Repository_Interfaces;
using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repository;

        private const decimal VipDiscountRate = 0.15m;
        private const decimal TaxRate = 0.14m;
        private const decimal FreeShippingThreshold = 1000m;
        private const decimal StandardShippingFee = 75m;
        private const decimal MaxOrderAmount = 50000m;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await _repository.GetOrderWithDetailsAsync(id);
        }

        public async Task<List<Order>> GetCustomerOrdersAsync(int customerId)
        {
            return await _repository.GetOrdersByCustomerAsync(customerId);
        }

        public async Task<(bool Success, string? Error)> CancelOrderAsync(int id)
        {
            var order = await _repository.GetOrderWithItemsAsync(id);
            if (order == null)
                return (false, "Order not found");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "Order is already cancelled");

            if (order.Status == OrderStatus.Paid)
            {
                foreach (var item in order.Items)
                {
                    var product = await _repository.GetProductAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                    }
                }
            }

            order.Status = OrderStatus.Cancelled;
            await _repository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error, object? Result)> CheckoutAsync(CreateOrderDto request)
        {
            if (request.Items == null || !request.Items.Any())
                return (false, "Cannot checkout an empty order.", null);

            var customer = await _repository.GetCustomerAsync(request.CustomerId);
            if (customer == null)
                return (false, $"Customer with ID {request.CustomerId} not found.", null);

            decimal subtotal = 0m;
            var orderItemsToSave = new List<OrderItem>();
            var productsToUpdate = new List<Product>();

            foreach (var itemDto in request.Items)
            {
                if (itemDto.Quantity <= 0)
                    return (false, "Product quantity must be at least 1.", null);

                var product = await _repository.GetProductAsync(itemDto.ProductId);
                if (product == null)
                    return (false, $"Product with ID {itemDto.ProductId} not found.", null);

                if (product.StockQuantity < itemDto.Quantity)
                    return (false, $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}", null);

                subtotal += product.Price * itemDto.Quantity;

                orderItemsToSave.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                });

                product.StockQuantity -= itemDto.Quantity;
                productsToUpdate.Add(product);
            }

            decimal discount = 0m;
            if (customer.IsVip)
            {
                discount += Math.Round(subtotal * VipDiscountRate, 2);
            }

            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var coupon = await _repository.GetActiveCouponAsync(request.CouponCode);
                if (coupon != null)
                {
                    discount += Math.Round(subtotal * (coupon.DiscountPercentage / 100m), 2);
                }
                else
                {
                    return (false, $"Invalid or inactive coupon code '{request.CouponCode}'.", null);
                }
            }

            if (discount > subtotal)
                discount = subtotal;

            var netAmount = subtotal - discount;
            var tax = Math.Round(netAmount * TaxRate, 2);
            var shipping = netAmount >= FreeShippingThreshold ? 0m : StandardShippingFee;
            var finalTotal = netAmount + tax + shipping;

            if (finalTotal > MaxOrderAmount)
                return (false, "Payment processing failed. Amount exceeds limit.", null);

            var txRef = $"TX-LEGACY-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var order = new Order
            {
                CustomerId = customer.Id,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Paid,
                Subtotal = subtotal,
                DiscountAmount = discount,
                TaxAmount = tax,
                ShippingFee = shipping,
                TotalAmount = finalTotal,
                Items = orderItemsToSave
            };

            var payment = new Payment
            {
                Order = order,
                Amount = finalTotal,
                PaymentDate = DateTime.UtcNow,
                TransactionReference = txRef,
                IsSuccess = true
            };

            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                await _repository.AddOrderAsync(order);
                await _repository.AddPaymentAsync(payment);
                await _repository.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return (false, "__INTERNAL_ERROR__:An error occurred while saving the order.", null);
            }

            var result = new
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                Subtotal = order.Subtotal,
                Discount = order.DiscountAmount,
                Tax = order.TaxAmount,
                Shipping = order.ShippingFee,
                Total = order.TotalAmount,
                TransactionReference = txRef
            };

            return (true, null, result);
        }
    
    }
}
