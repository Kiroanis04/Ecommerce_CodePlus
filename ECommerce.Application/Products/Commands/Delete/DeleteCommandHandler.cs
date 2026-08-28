using ECommerce.Application.Products.Commands.Delete;
using ECommerce.Application.Products.DTOs;
using ECommerce.Application.Repository_Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Commands.DeleteProduct
{
    public sealed class DeleteCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
    {
        private readonly IProductRepository _repository;

        public DeleteCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeleteProductResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId);
            if (product == null)
                return new DeleteProductResponse(false, $"Product with ID {request.ProductId} not found.");

            _repository.Remove(product);
            await _repository.SaveChangesAsync();

            return new DeleteProductResponse(true, null);
        }
    }
}