using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Invoiced = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
