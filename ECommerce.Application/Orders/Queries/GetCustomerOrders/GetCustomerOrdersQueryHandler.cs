using ECommerce.Application.Orders.DTOs;
using ECommerce.Application.Repository_Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Queries.GetCustomerOrders
{
    public sealed class GetCustomerOrdersQueryHandler
        : IRequestHandler<GetCustomerOrdersQuery, IReadOnlyList<OrderResponse>>
    {
        private readonly IOrderRepository _repository;

        public GetCustomerOrdersQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<OrderResponse>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _repository.GetOrdersByCustomerAsync(request.CustomerId);
            var result = new List<OrderResponse>();

            foreach (var order in orders)
            {
                var items = new List<OrderItemResponse>();
                foreach (var i in order.Items)
                {
                    var product = await _repository.GetProductAsync(i.ProductId);
                    items.Add(new OrderItemResponse(
                        i.ProductId,
                        product?.Name ?? "Unknown Product",
                        i.Quantity,
                        i.UnitPrice,
                        i.UnitPrice * i.Quantity));
                }

                result.Add(new OrderResponse(
                    order.Id,
                    order.CustomerId,
                    order.CreatedAt,
                    order.Subtotal,
                    order.DiscountAmount,
                    order.TaxAmount,
                    order.ShippingFee,
                    order.TotalAmount,
                    items));
            }

            return result;
        }
    }
}