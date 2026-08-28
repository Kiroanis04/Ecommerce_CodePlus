using ECommerce.Application.Products.Commands.Create;
using ECommerce.Application.Products.DTOs;
using ECommerce.Application.Repository_Interfaces;
using ECommerce.DAL.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Commands.CreateProduct
{
    public sealed class CreateCommandHandler : IRequestHandler<CreateCommand, CreateProductResponse>
    {
        private readonly IProductRepository _repository;

        public CreateCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateProductResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Product;

            if (await _repository.SkuExistsAsync(dto.SKU))
                return new CreateProductResponse(false, $"Product with SKU '{dto.SKU}' already exists.", null);

            var product = new Product
            {
                Name = dto.Name,
                SKU = dto.SKU.ToUpper(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            var response = new ProductResponse(product.Id, product.Name, product.SKU, product.Price, product.StockQuantity);
            return new CreateProductResponse(true, null, response);
        }
    }
}