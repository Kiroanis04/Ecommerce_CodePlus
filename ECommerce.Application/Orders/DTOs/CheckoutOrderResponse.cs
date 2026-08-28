using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.DTOs
{
    public record CheckoutOrderResponse(
        bool Success,
        string? Error,
        int? OrderId,
        string? status,
        decimal? subtotal,
        decimal? Discount,
        decimal? tax,
        decimal? shipping,
        decimal? Total,
        string? TransactionReference);
}
