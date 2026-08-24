using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Repository_Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<bool> SkuExistsAsync(string sku);
        Task AddAsync(Product product);
        void Update(Product product);
        void Remove(Product product);
        Task SaveChangesAsync();
    }
}
