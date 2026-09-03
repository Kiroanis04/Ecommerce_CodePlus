using ECommerce.Application.Interfaces;
using ECommerce.Application.Invoice.Commands;
using ECommerce.Application.Repository_Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Invoice.Handler
{
    public class GenerateInvoiceHandler
     : IRequestHandler<GenerateInvoiceCommand, GenerateInvoiceResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPdfGenerator _pdfGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GenerateInvoiceHandler> _logger;

        public GenerateInvoiceHandler(
            IOrderRepository orderRepository,
            IPdfGenerator pdfGenerator,
            IUnitOfWork unitOfWork,
            ILogger<GenerateInvoiceHandler> logger)
        {
            _orderRepository = orderRepository;
            _pdfGenerator = pdfGenerator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<GenerateInvoiceResult> Handle(
            GenerateInvoiceCommand request,
            CancellationToken cancellationToken)
        {
            var result = new GenerateInvoiceResult();

            try
            {
                var pendingOrders = await _orderRepository.GetOrdersWithoutInvoiceAsync(
                    request.MaxInvoicesPerBatch);

                if (!pendingOrders.Any())
                {
                    _logger.LogInformation("No pending invoices to generate");
                    return result;
                }

                foreach (var order in pendingOrders)
                {
                    try
                    {
                        var customer = await _orderRepository.GetCustomerByIdAsync(order.CustomerId);
                        var orderItems = await _orderRepository.GetOrderItemsAsync(order.Id);

                        if (customer == null)
                        {
                            result.Errors.Add($"Customer not found for order {order.Id}");
                            continue;
                        }

                        var pdfBytes = await _pdfGenerator.GenerateInvoicePdfAsync(
                            customer,
                            order,
                            orderItems);

                        order.MarkInvoiceAsGenerated(pdfBytes);

                        result.Invoices.Add(new InvoiceDto
                        {
                            OrderId = order.Id,
                            InvoiceNumber = order.InvoiceNumber ?? string.Empty,
                            TotalAmount = order.TotalAmount,
                            GeneratedDate = DateTime.UtcNow,
                            CustomerName = customer.FullName
                        });

                        result.InvoicesGenerated++;
                        _logger.LogInformation($"Invoice generated for Order {order.Id} - Number: {order.InvoiceNumber}");
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Failed to generate invoice for order {order.Id}: {ex.Message}");
                        _logger.LogError(ex, $"Error generating invoice for order {order.Id}");
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Invoices generated: {result.InvoicesGenerated}, Errors: {result.Errors.Count}");
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Generation process failed: {ex.Message}");
                _logger.LogError(ex, "Error during invoice generation");
            }

            return result;
        }
    }
}
