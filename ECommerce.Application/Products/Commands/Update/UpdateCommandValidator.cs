using ECommerce.Application.Products.Commands.Update;
using FluentValidation;

namespace ECommerce.Application.Products.Commands.UpdateProduct
{
    public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("A valid product id is required.");

            RuleFor(x => x.Product.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

            RuleFor(x => x.Product.SKU)
                .NotEmpty().WithMessage("SKU is required.");

            RuleFor(x => x.Product.Price)
                .GreaterThan(0).WithMessage("Price must be positive.");

            RuleFor(x => x.Product.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
        }
    }
}