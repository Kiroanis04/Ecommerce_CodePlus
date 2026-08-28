using ECommerce.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Commands.Create
{
    public record CreateCommand(CreateProductDto Product) : IRequest<CreateProductResponse>;
}
