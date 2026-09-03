using ECommerce.Application.Basket.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Validators
{
    public class CleanExpiredBasketItemsValidator : AbstractValidator<CleanExpiredBasketItemsCommand>
    {
        public CleanExpiredBasketItemsValidator()
        {
            RuleFor(x => x.ExpiryDays)
                .GreaterThan(0).WithMessage("Expiry days must be greater than 0")
                .LessThanOrEqualTo(30).WithMessage("Expiry days cannot exceed 30");
        }
    }
}
