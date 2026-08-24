using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Repository_Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderWithDetailsAsync(int id);
        Task<Order?> GetOrderWithItemsAsync(int id);
        Task<List<Order>> GetOrdersByCustomerAsync(int customerId);
        Task<Customer?> GetCustomerAsync(int customerId);
        Task<Product?> GetProductAsync(int productId);
        Task<Coupon?> GetActiveCouponAsync(string code);
        Task AddOrderAsync(Order order);
        Task AddPaymentAsync(Payment payment);
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
    }
}
