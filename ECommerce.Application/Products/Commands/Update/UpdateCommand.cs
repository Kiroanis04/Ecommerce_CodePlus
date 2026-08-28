using ECommerce.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Commands.Update
{
    public record UpdateProductCommand(int ProductId, UpdateProductResponse Product) : IRequest<UpdateProductResponse>;
}
