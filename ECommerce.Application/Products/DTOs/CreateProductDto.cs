namespace ECommerce.Application.Products.DTOs;

public class CreateProductDto
{
    private object value;
    private string v1;
    private double v2;
    private int v3;

    public CreateProductDto(string? value, string v1, double v2, int v3)
    {
        this.value = value;
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }

    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
