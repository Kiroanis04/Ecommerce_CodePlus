using ECommerce.Application.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Commands.Checkout
{
    public record CheckoutCommand(CreateOrderDto Order) : IRequest<CheckoutOrderResponse>;
}
