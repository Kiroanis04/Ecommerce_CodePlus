using ECommerce.Application.Customers.DTOs;
using ECommerce.Application.Customers.Queries.GetById;
using ECommerce.Application.Repository_Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Queries.GetCustomerById
{
    public sealed class GetCustomerByIdQueryHandler
        : IRequestHandler<GetCustomerByIdQuery, CustomerResponse?>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerResponse?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdWithOrdersAsync(request.CustomerId);

            if (customer == null)
                return null;

            return new CustomerResponse(customer.Id, customer.FullName, customer.Email, customer.IsVip);
        }
    }
}