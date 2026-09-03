using ECommerce.Application.Invoice.Commands;
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
    public class InvoiceGenerationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InvoiceGenerationBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

        public InvoiceGenerationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<InvoiceGenerationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Invoice Generation Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessInvoiceGeneration(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during invoice generation process.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Invoice Generation Service is stopping.");
        }

        private async Task ProcessInvoiceGeneration(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            _logger.LogInformation("Starting invoice generation process...");
            var result = await mediator.Send(
                new GenerateInvoiceCommand
                {
                    MaxInvoicesPerBatch = 10
                },
                cancellationToken);

            _logger.LogInformation($"Generated {result.InvoicesGenerated} invoices. Errors: {result.Errors.Count}");

            foreach (var invoice in result.Invoices)
            {
                _logger.LogInformation(
                    $"Invoice {invoice.InvoiceNumber} generated for Order {invoice.OrderId} " +
                    $"with total: {invoice.TotalAmount:C} for customer: {invoice.CustomerName}");
            }
        }
    }
}
