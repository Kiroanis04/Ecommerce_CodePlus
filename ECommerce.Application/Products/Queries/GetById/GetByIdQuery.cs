using ECommerce.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Queries.GetById
{
    public record GetByIdQuery(int ProductId) : IRequest<ProductResponse?>;
}
