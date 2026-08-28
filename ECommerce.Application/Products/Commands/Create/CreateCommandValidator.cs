using ECommerce.Application.Products.Commands.Create;
using FluentValidation;

namespace ECommerce.Application.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandValidator : AbstractValidator<CreateCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Product.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

            RuleFor(x => x.Product.SKU)
                .NotEmpty().WithMessage("SKU is required.");

            RuleFor(x => x.Product.Price)
                .GreaterThan(0).WithMessage("Product price must be greater than zero.");

            RuleFor(x => x.Product.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
        }
    }
}