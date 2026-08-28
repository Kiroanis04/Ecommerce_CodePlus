using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Queries.GetById
{
    public sealed class GetProductByIdQueryValidator : AbstractValidator<GetByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("A valid product id is required.");
        }
    }
}
