using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Repository_Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdWithOrdersAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(Customer customer);
        Task SaveChangesAsync();
    }
}
