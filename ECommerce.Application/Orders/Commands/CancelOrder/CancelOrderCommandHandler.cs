using ECommerce.Application.Orders.DTOs;
using ECommerce.Application.Repository_Interfaces;
using ECommerce.DAL.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CancelOrderResponse>
    {
        private readonly IOrderRepository _repository;

        public CancelOrderCommandHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<CancelOrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetOrderWithItemsAsync(request.OrderId);

            if (order == null)
                return new CancelOrderResponse(false, "Order not found");

            if (order.Status == OrderStatus.Cancelled)
                return new CancelOrderResponse(false, "Order is already cancelled");

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

            return new CancelOrderResponse(true, null);
        }
    }
}