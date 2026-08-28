using FluentValidation;

namespace ECommerce.Application.Orders.Commands.Checkout
{
    public sealed class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
    {
        public CheckoutCommandValidator()
        {
            RuleFor(x => x.Order.CustomerId)
                .GreaterThan(0).WithMessage("A valid customer id is required.");

            RuleFor(x => x.Order.Items)
                .NotNull().WithMessage("Order items are required.")
                .Must(items => items.Count > 0).WithMessage("Cannot checkout an empty order.");

            RuleForEach(x => x.Order.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Product quantity must be at least 1.");

                item.RuleFor(i => i.ProductId)
                    .GreaterThan(0).WithMessage("A valid product id is required.");
            });
        }
    }
}