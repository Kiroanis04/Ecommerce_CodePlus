using ECommerce.Application.Products.Commands.Update;
using ECommerce.Application.Products.DTOs;
using ECommerce.Application.Repository_Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Commands.UpdateProduct
{
    public sealed class UpdateCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
    {
        private readonly IProductRepository _repository;

        public UpdateCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(request.ProductId);
            if (existing == null)
                return new UpdateProductResponse(false, $"Product with ID {request.ProductId} not found.");

            var dto = request.Product;

            existing.Name = dto.Name;
            existing.SKU = dto.SKU;
            existing.Price = dto.Price;
            existing.StockQuantity = dto.StockQuantity;

            _repository.Update(existing);
            await _repository.SaveChangesAsync();

            return new UpdateProductResponse(true, null);
        }
    }
}