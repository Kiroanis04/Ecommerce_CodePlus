using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IPdfGenerator
    {
        Task<byte[]> GenerateInvoicePdfAsync(Customer customer, Order order, List<OrderItem> items);
    }
}
