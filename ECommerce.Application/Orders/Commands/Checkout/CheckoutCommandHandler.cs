using ECommerce.Application.Orders.DTOs;
using ECommerce.Application.Repository_Interfaces;
using ECommerce.DAL.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Commands.Checkout
{
    public sealed class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, CheckoutOrderResponse>
    {
        private const decimal VipDiscountRate = 0.10m;      
        private const decimal TaxRate = 0.14m;               
        private const decimal FreeShippingThreshold = 500m;  
        private const decimal StandardShippingFee = 50m;    
        private const decimal MaxOrderAmount = 100000m;      

        private readonly IOrderRepository _repository;

        public CheckoutCommandHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<CheckoutOrderResponse> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Order;

            var customer = await _repository.GetCustomerAsync(dto.CustomerId);
            if (customer == null)
                return Fail($"Customer with ID {dto.CustomerId} not found.");

            decimal subtotal = 0m;
            var orderItemsToSave = new List<OrderItem>();
            var productsToUpdate = new List<Product>();

            foreach (var itemDto in dto.Items)
            {
                var product = await _repository.GetProductAsync(itemDto.ProductId);
                if (product == null)
                    return Fail($"Product with ID {itemDto.ProductId} not found.");

                if (product.StockQuantity < itemDto.Quantity)
                    return Fail($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}");

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

            if (!string.IsNullOrWhiteSpace(dto.CouponCode))
            {
                var coupon = await _repository.GetActiveCouponAsync(dto.CouponCode);
                if (coupon == null)
                    return Fail($"Invalid or inactive coupon code '{dto.CouponCode}'.");

                discount += Math.Round(subtotal * (coupon.DiscountPercentage / 100m), 2);
            }

            if (discount > subtotal)
                discount = subtotal;

            var netAmount = subtotal - discount;
            var tax = Math.Round(netAmount * TaxRate, 2);
            var shipping = netAmount >= FreeShippingThreshold ? 0m : StandardShippingFee;
            var finalTotal = netAmount + tax + shipping;

            if (finalTotal > MaxOrderAmount)
                return Fail("Payment processing failed. Amount exceeds limit.");

            var txRef = $"TX-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

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
                return Fail("An error occurred while saving the order.");
            }

            return new CheckoutOrderResponse(
                true, null,
                order.Id, order.Status.ToString(),
                order.Subtotal, order.DiscountAmount, order.TaxAmount, order.ShippingFee, order.TotalAmount,
                txRef);
        }

        private static CheckoutOrderResponse Fail(string error) =>
            new(false, error, null, null, null, null, null, null, null, null);
    }
}