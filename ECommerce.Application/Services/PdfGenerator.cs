using ECommerce.Application.Interfaces;
using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class PdfGenerator : IPdfGenerator
    {
        public PdfGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(Customer customer, Order order, List<OrderItem> items)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Text($"INVOICE - {order.InvoiceNumber ?? "NEW"}")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(10);

                                // Customer Info
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Text($"Customer: {customer.FullName}").FontSize(14);
                                        col.Item().Text($"Email: {customer.Email}").FontSize(14);
                                        col.Item().Text($"Phone: {customer.PhoneNumber ?? "N/A"}").FontSize(14);
                                    });
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Text($"Date: {order.CreatedAt:dd/MM/yyyy HH:mm}").FontSize(14);
                                        col.Item().Text($"Order ID: {order.Id:D6}").FontSize(14);
                                        col.Item().Text($"Status: {order.Status}").FontSize(14);
                                    });
                                });

                                column.Item().LineHorizontal(1);

                                // Table Header
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Product").Bold().FontColor(Colors.White);
                                        header.Cell().Text("Quantity").Bold().FontColor(Colors.White);
                                        header.Cell().Text("Unit Price").Bold().FontColor(Colors.White);
                                        header.Cell().Text("Total").Bold().FontColor(Colors.White);

                                        header.Cell().Background(Colors.Blue.Darken2).Padding(5);
                                        header.Cell().Background(Colors.Blue.Darken2).Padding(5);
                                        header.Cell().Background(Colors.Blue.Darken2).Padding(5);
                                        header.Cell().Background(Colors.Blue.Darken2).Padding(5);
                                    });
                                });

                                // Table Rows
                                foreach (var item in items)
                                {
                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(2);
                                        });

                                        table.Cell().Text(item.Product?.Name ?? item.ProductName ?? "Product");
                                        table.Cell().Text(item.Quantity.ToString());
                                        table.Cell().Text($"{item.UnitPrice:C}");
                                        table.Cell().Text($"{item.Quantity * item.UnitPrice:C}");
                                    });
                                }

                                column.Item().LineHorizontal(1);

                                // Total
                                column.Item().AlignRight().Column(col =>
                                {
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("Subtotal:").FontSize(14);
                                        row.RelativeItem().Text($"{order.Subtotal:C}").FontSize(14);
                                    });

                                    if (order.DiscountAmount > 0)
                                    {
                                        col.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text("Discount:").FontSize(14);
                                            row.RelativeItem().Text($"-{order.DiscountAmount:C}").FontSize(14).FontColor(Colors.Green.Medium);
                                        });
                                    }

                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("Tax:").FontSize(14);
                                        row.RelativeItem().Text($"{order.TaxAmount:C}").FontSize(14);
                                    });

                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("Shipping:").FontSize(14);
                                        row.RelativeItem().Text($"{order.ShippingFee:C}").FontSize(14);
                                    });

                                    col.Item().LineHorizontal(1);

                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("TOTAL:").Bold().FontSize(18);
                                        row.RelativeItem().Text($"{order.TotalAmount:C}").Bold().FontSize(18).FontColor(Colors.Green.Darken2);
                                    });
                                });
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text($"Generated on {DateTime.UtcNow:dd/MM/yyyy HH:mm}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Medium);
                    });
                });

                using var stream = new MemoryStream();
                document.GeneratePdf(stream);
                return stream.ToArray();
            });
        }
    }
}
