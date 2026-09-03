using ECommerce.Application.Basket.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class BasketCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BasketCleanupBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24);

        public BasketCleanupBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<BasketCleanupBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Basket Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBasketOperations(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during basket cleanup process.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Basket Cleanup Service is stopping.");
        }

        private async Task ProcessBasketOperations(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            _logger.LogInformation("Starting email reminder process...");
            var emailResult = await mediator.Send(
                new SendReminderEmailsCommand
                {
                    DaysAfterAddition = 4,
                    MaxEmailsPerBatch = 100
                },
                cancellationToken);

            _logger.LogInformation($"Sent {emailResult.EmailsSent} reminder emails. Errors: {emailResult.Errors.Count}");

            _logger.LogInformation("Starting cleanup process...");
            var cleanupResult = await mediator.Send(
                new CleanExpiredBasketItemsCommand
                {
                    ExpiryDays = 7
                },
                cancellationToken);

            _logger.LogInformation($"Deleted {cleanupResult.ItemsDeleted} expired basket items. Errors: {cleanupResult.Errors.Count}");
        }
    }
}
