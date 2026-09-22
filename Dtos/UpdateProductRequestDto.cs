using System.ComponentModel.DataAnnotations;

public class UpdateProductRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Sku { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; }
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Required]
    public byte[] RowVersion { get; set; } = null!;
}