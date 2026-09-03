using ECommerce.Application.Basket.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Validators
{
    public class SendReminderEmailsValidator : AbstractValidator<SendReminderEmailsCommand>
    {
        public SendReminderEmailsValidator()
        {
            RuleFor(x => x.DaysAfterAddition)
                .GreaterThan(0).WithMessage("Days after addition must be greater than 0")
                .LessThanOrEqualTo(10).WithMessage("Days after addition cannot exceed 10");

            RuleFor(x => x.MaxEmailsPerBatch)
                .GreaterThan(0).WithMessage("Max emails per batch must be greater than 0")
                .LessThanOrEqualTo(500).WithMessage("Max emails per batch cannot exceed 500");
        }
    }
}
