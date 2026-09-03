using ECommerce.DAL.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class BasketRepository : GenericRepository<BasketItem>, IBasketRepository
    {
        public BasketRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<BasketItem>> GetExpiredItemsAsync(int days)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            return await _dbSet
                .Where(b => b.AddedDate <= cutoffDate
                       && b.Status == BasketItemStatus.Active)
                .Include(b => b.Customer)
                .Include(b => b.Product)
                .ToListAsync();
        }

        public async Task<List<BasketItem>> GetItemsNeedingReminderAsync(int daysAfterAddition, int maxItems)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysAfterAddition);

            return await _dbSet
                .Where(b => b.AddedDate <= cutoffDate
                       && !b.EmailSent
                       && b.Status == BasketItemStatus.Active)
                .OrderBy(b => b.AddedDate)
                .Take(maxItems)
                .Include(b => b.Customer)
                .Include(b => b.Product)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<BasketItem?> GetBasketWithCustomerAsync(int basketItemId)
        {
            return await _dbSet
                .Include(b => b.Customer)
                .Include(b => b.Product)
                .FirstOrDefaultAsync(b => b.Id == basketItemId);
        }

        public void Delete(BasketItem item)
        {
            item.Delete();
            _dbSet.Update(item);
        }

        public async Task<IEnumerable<BasketItem>> GetActiveBasketItemsByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Where(b => b.CustomerId == customerId
                       && b.Status == BasketItemStatus.Active)
                .Include(b => b.Product)
                .ToListAsync();
        }
    }
}
