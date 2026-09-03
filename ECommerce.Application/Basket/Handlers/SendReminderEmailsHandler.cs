using ECommerce.Application.Basket.Commands;
using ECommerce.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Basket.Handlers
{
    public class SendReminderEmailsHandler
    : IRequestHandler<SendReminderEmailsCommand, SendReminderEmailsResult>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SendReminderEmailsHandler> _logger;

        public SendReminderEmailsHandler(
            IBasketRepository basketRepository,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            ILogger<SendReminderEmailsHandler> logger)
        {
            _basketRepository = basketRepository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SendReminderEmailsResult> Handle(
            SendReminderEmailsCommand request,
            CancellationToken cancellationToken)
        {
            var result = new SendReminderEmailsResult();

            try
            {
                var itemsNeedingReminder = await _basketRepository.GetItemsNeedingReminderAsync(
                    request.DaysAfterAddition,
                    request.MaxEmailsPerBatch);

                if (!itemsNeedingReminder.Any())
                {
                    _logger.LogInformation("No items need reminder emails");
                    return result;
                }

                foreach (var item in itemsNeedingReminder)
                {
                    try
                    {
                        var customer = await _basketRepository.GetCustomerByIdAsync(item.CustomerId);

                        if (customer == null || string.IsNullOrEmpty(customer.Email))
                        {
                            result.Errors.Add($"Customer not found or email missing for item {item.Id}");
                            continue;
                        }

                        await _emailService.SendBasketReminderEmailAsync(
                            customer.Email,
                            customer.FullName,
                            item.ProductName,
                            item.Quantity,
                            item.Price
                        );

                        item.MarkEmailAsSent();
                        result.Recipients.Add(customer.Email);
                        result.EmailsSent++;

                        _logger.LogInformation($"Reminder email sent to {customer.Email} for product: {item.ProductName}");
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Failed to send email for item {item.Id}: {ex.Message}");
                        _logger.LogError(ex, $"Error sending reminder email for item {item.Id}");
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Emails sent: {result.EmailsSent}, Errors: {result.Errors.Count}");
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Process failed: {ex.Message}");
                _logger.LogError(ex, "Error during email reminder process");
            }

            return result;
        }
    }
}
