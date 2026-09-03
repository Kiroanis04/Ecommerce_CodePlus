using ECommerce.DAL.Enums;
using ECommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.DAL.Entities;

public class BasketItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BasketId { get; set; }

    [ForeignKey(nameof(BasketId))]
    public virtual Basket? Basket { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    public bool EmailSent { get; set; } = false;

    public BasketItemStatus Status { get; set; } = BasketItemStatus.Active;

    public void MarkEmailAsSent()
    {
        EmailSent = true;
    }

    public bool IsExpired(int days)
    {
        return (DateTime.UtcNow - AddedDate).TotalDays >= days;
    }

    public void Delete()
    {
        Status = BasketItemStatus.Deleted;
    }

    public void ConvertToOrder()
    {
        Status = BasketItemStatus.ConvertedToOrder;
    }
}