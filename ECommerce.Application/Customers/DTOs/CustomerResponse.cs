using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.DTOs
{
    public record CustomerResponse(int Id, string FullName, string Email, bool IsVip);
}
