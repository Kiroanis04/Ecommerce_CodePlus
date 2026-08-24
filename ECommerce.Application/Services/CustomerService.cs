using ECommerce.API.DTOs;
using ECommerce.Application.Repository_Interfaces;
using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class CustomerService 
    {
        private readonly ICustomerRepository _repository;
        private const decimal VipThreshold = 500m;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdWithOrdersAsync(id);
        }

        public async Task<(bool Success, string? Error, Customer? Customer)> CreateAsync(CreateCustomerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return (false, "Full name is required.", null);

            if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
                return (false, "A valid email address is required.", null);

            if (await _repository.EmailExistsAsync(dto.Email))
                return (false, "Email is already registered.", null);

            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                IsVip = dto.IsVip
            };

            await _repository.AddAsync(customer);
            await _repository.SaveChangesAsync();

            return (true, null, customer);
        }

        public async Task<(bool Success, string? Error)> UpgradeToVipAsync(int id)
        {
            var customer = await _repository.GetByIdWithOrdersAsync(id);
            if (customer == null)
                return (false, "NotFound");

            var totalSpent = customer.Orders
                .Where(o => o.Status == OrderStatus.Paid)
                .Sum(o => o.TotalAmount);

            if (totalSpent < VipThreshold)
                return (false, $"Customer does not qualify for VIP. Total spend {totalSpent:C} is less than required {VipThreshold:C}.");

            customer.IsVip = true;
            await _repository.SaveChangesAsync();

            return (true, null);
        }
    }
}
