using ECommerce.Application.Products.DTOs;
using ECommerce.Application.Products.Queries.GetAll;
using ECommerce.Application.Repository_Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Queries.GetAllProducts
{
    public sealed class GetAllQueryHandler : IRequestHandler<GetAllProductsQuery, IReadOnlyList<ProductResponse>>
    {
        private readonly IProductRepository _repository;

        public GetAllQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetAllAsync();

            return products
                .Select(p => new ProductResponse(p.Id, p.Name, p.SKU, p.Price, p.StockQuantity))
                .ToList();
        }
    }
}