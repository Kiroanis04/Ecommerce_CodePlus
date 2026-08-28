using ECommerce.Application.Customers.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Queries.GetById
{
    public record GetCustomerByIdQuery(int CustomerId) : IRequest<CustomerResponse?>;
}
