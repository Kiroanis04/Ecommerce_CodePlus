using ECommerce.Application.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Queries.GetOrder
{
    public record GetOrderByIdQuery(int OrderId) : IRequest<OrderResponse?>;
}
