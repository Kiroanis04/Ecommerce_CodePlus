using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Invoice.Commands
{
    public class GenerateInvoiceCommand : IRequest<GenerateInvoiceResult>
    {
        public int MaxInvoicesPerBatch { get; set; } = 10;
    }

    public class GenerateInvoiceResult
    {
        public int InvoicesGenerated { get; set; }
        public List<InvoiceDto> Invoices { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

    public class InvoiceDto
    {
        public int OrderId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}
