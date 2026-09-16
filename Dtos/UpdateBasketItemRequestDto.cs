using System.ComponentModel.DataAnnotations;

public class UpdateBasketItemRequestDto
{
    [Range(1, 100)]
    public int Quantity { get; set; }
}