using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerce.DAL.Enums;

namespace ECommerce.DAL.Entities;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingFee { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(50)]
    public string? InvoiceNumber { get; set; }

    public bool InvoiceGenerated { get; set; } = false;

    public byte[]? InvoicePdf { get; set; }

    // Navigation Properties
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public virtual Payment? Payment { get; set; }

    // Domain Methods
    public void CalculateTotal()
    {
        Subtotal = Items.Sum(item => item.UnitPrice * item.Quantity);
        TotalAmount = Subtotal - DiscountAmount + TaxAmount + ShippingFee;
    }

    public void MarkInvoiceAsGenerated(byte[] pdfContent)
    {
        if (pdfContent == null || pdfContent.Length == 0)
            throw new ArgumentException("PDF content cannot be empty");

        InvoiceGenerated = true;
        InvoicePdf = pdfContent;
        InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Id:D6}";
        Status = OrderStatus.Invoiced;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Invoiced && Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot ship order without invoice");

        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot deliver order that hasn't been shipped");

        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }
}