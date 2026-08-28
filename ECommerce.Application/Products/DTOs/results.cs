using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.DTOs
{
    public record CreateProductResponse(bool Success, string? Error, ProductResponse? Product);
    public record UpdateProductResponse(bool Success, string? Error);
    public record DeleteProductResponse(bool Success, string? Error);
}
