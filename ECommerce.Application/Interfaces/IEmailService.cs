using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendBasketReminderEmailAsync(string email, string customerName, string productName, int quantity, decimal price);
        Task SendInvoiceEmailAsync(string email, string customerName, string invoiceNumber, byte[] pdfAttachment);
        Task SendOrderConfirmationEmailAsync(string email, string customerName, string orderNumber);
    }
}
