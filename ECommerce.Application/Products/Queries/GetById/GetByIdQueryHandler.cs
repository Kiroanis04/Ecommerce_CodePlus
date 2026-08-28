using ECommerce.Application.Products.DTOs;
using ECommerce.Application.Products.Queries.GetById;
using ECommerce.Application.Repository_Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Queries.GetProductById
{
    public sealed class GetProductByIdQueryHandler : IRequestHandler<GetByIdQuery, ProductResponse?>
    {
        private readonly IProductRepository _repository;

        public GetProductByIdQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductResponse?> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId);

            if (product == null)
                return null;

            return new ProductResponse(product.Id, product.Name, product.SKU, product.Price, product.StockQuantity);
        }
    }
}