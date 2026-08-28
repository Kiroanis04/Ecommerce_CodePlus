using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.DTOs
{
    public record ProductResponse(int Id, string Name, string SKU, decimal Price, int StockQuantity);
}
