using ECommerce.Application.Customers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MediatR;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Commands.RegisterCustomer
{
    public record RegisterCustomerCommand(CreateCustomerDto CreateCustomer) : IRequest<CustomerResponse>;
}
