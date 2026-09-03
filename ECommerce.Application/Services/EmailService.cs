using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendBasketReminderEmailAsync(string email, string customerName, string productName, int quantity, decimal price)
        {
            var subject = "Reminder: Items in your basket";
            var body = $@"
            <h2>Hello {customerName},</h2>
            <p>You have items in your basket that you haven't purchased yet:</p>
            <ul>
                <li><strong>{productName}</strong> - Quantity: {quantity} - Price: {price:C}</li>
            </ul>
            <p>Complete your purchase now!</p>
            <a href='https://yourecommerce.com/basket'>Go to Basket</a>
        ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendInvoiceEmailAsync(string email, string customerName, string invoiceNumber, byte[] pdfAttachment)
        {
            var subject = $"Invoice {invoiceNumber}";
            var body = $@"
            <h2>Hello {customerName},</h2>
            <p>Thank you for your order!</p>
            <p>Your invoice number: <strong>{invoiceNumber}</strong></p>
            <p>Please find attached your invoice.</p>
        ";

            await SendEmailWithAttachmentAsync(email, subject, body, pdfAttachment, $"invoice-{invoiceNumber}.pdf");
        }

        public async Task SendOrderConfirmationEmailAsync(string email, string customerName, string orderNumber)
        {
            var subject = $"Order Confirmation - {orderNumber}";
            var body = $@"
            <h2>Hello {customerName},</h2>
            <p>Your order has been confirmed!</p>
            <p>Order number: <strong>{orderNumber}</strong></p>
            <p>We'll notify you when your items are shipped.</p>
        ";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]);

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }

        private async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            builder.Attachments.Add(attachmentName, attachment);

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]);

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email with attachment sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email with attachment to {to}");
                throw;
            }
        }
    }
    }
