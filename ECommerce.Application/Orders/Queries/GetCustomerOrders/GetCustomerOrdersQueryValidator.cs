using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Queries.GetCustomerOrders
{
    public sealed class GetCustomerOrdersQueryValidator : AbstractValidator<GetCustomerOrdersQuery>
    {
        public GetCustomerOrdersQueryValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("A valid customer id is required.");
        }
    }
}
