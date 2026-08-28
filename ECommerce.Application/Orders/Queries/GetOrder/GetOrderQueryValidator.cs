using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Queries.GetOrder
{
    public sealed class GetOrderQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderQueryValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("A valid order id is required.");
        }
    }
}
