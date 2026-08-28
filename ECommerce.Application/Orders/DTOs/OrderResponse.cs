using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.DTOs
{
    public record OrderResponse(
         int Id,
         int CustomerId,
         DateTime CreatedAt,
         decimal Subtotal,
         decimal DiscountAmount,
         decimal TaxAmount,
         decimal ShippingFee,
         decimal TotalAmount,
         IReadOnlyList<OrderItemResponse> Items
         );
}
