using System.ComponentModel.DataAnnotations;

public class UpdateStockRequestDto
{
    [Required(ErrorMessage = "NewStock is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
    public int? NewStock { get; set; }

    [Required(ErrorMessage = "RowVersion is required.")]
    public byte[] RowVersion { get; set; } = null!;
}