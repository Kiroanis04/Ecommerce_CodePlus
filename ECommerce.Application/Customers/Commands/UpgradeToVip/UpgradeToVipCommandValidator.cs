using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Commands.UpgradeToVip
{
    public sealed class UpgradeCustomerToVipCommandValidator : AbstractValidator<UpgradeCustomerToVipCommand>
    {
        public UpgradeCustomerToVipCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("A valid customer id is required.");
        }
    }
}
