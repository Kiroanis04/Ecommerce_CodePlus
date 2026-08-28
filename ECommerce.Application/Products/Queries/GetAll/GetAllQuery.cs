using ECommerce.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Queries.GetAll
{
    public record GetAllProductsQuery : IRequest<IReadOnlyList<ProductResponse>>;
}
