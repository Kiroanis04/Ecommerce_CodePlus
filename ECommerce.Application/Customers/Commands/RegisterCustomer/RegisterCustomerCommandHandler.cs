using ECommerce.Application.Customers.DTOs;
using ECommerce.Application.Repository_Interfaces;
using ECommerce.DAL.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Commands.RegisterCustomer
{
    public sealed class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, CustomerResponse>
    {
        private readonly ICustomerRepository _customerRepository;

        public RegisterCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerResponse> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            var dto = request.CreateCustomer;

            if (await _customerRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email is already registered.");

            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                IsVip = dto.IsVip
            };

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return new CustomerResponse(customer.Id, customer.FullName, customer.Email, customer.IsVip);
        }
    }
}