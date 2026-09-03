using ECommerce.Application.Basket.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Basket.Handlers
{
    public class CleanExpiredBasketItemsHandler
    : IRequestHandler<CleanExpiredBasketItemsCommand, CleanExpiredBasketItemsResult>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CleanExpiredBasketItemsHandler> _logger;

        public CleanExpiredBasketItemsHandler(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            ILogger<CleanExpiredBasketItemsHandler> logger)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CleanExpiredBasketItemsResult> Handle(
            CleanExpiredBasketItemsCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CleanExpiredBasketItemsResult();

            try
            {
                var expiredItems = await _basketRepository.GetExpiredItemsAsync(request.ExpiryDays);

                if (!expiredItems.Any())
                {
                    _logger.LogInformation("No expired basket items found");
                    return result;
                }

                foreach (var item in expiredItems)
                {
                    try
                    {
                        result.DeletedItemNames.Add(item.ProductName);
                        _basketRepository.Delete(item);
                        _logger.LogInformation($"Deleted expired basket item: {item.ProductName} (ID: {item.Id})");
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Failed to delete item {item.Id}: {ex.Message}");
                        _logger.LogError(ex, $"Error deleting basket item {item.Id}");
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                result.ItemsDeleted = expiredItems.Count;

                _logger.LogInformation($"Cleanup completed: {result.ItemsDeleted} items deleted, {result.Errors.Count} errors");
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Cleanup failed: {ex.Message}");
                _logger.LogError(ex, "Error during basket cleanup");
            }

            return result;
        }
    }
}
