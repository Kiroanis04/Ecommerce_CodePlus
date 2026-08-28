using ECommerce.Application.Orders.DTOs;
using ECommerce.Application.Orders.Queries.GetOrder;
using ECommerce.Application.Repository_Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Queries.GetOrderById
{
    public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse?>
    {
        private readonly IOrderRepository _repository;

        public GetOrderByIdQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderResponse?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetOrderWithDetailsAsync(request.OrderId);

            if (order == null)
                return null;

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

            return new OrderResponse(
                order.Id,
                order.CustomerId,
                order.CreatedAt,
                order.Subtotal,
                order.DiscountAmount,
                order.TaxAmount,
                order.ShippingFee,
                order.TotalAmount,
                items);
        }
    }
}