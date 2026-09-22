using System.ComponentModel.DataAnnotations;

public class CreateProductRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    [Required]
    [MaxLength(50)]
    public string Sku { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}