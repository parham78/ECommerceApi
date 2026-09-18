using System.ComponentModel.DataAnnotations;

public class CheckoutRequestDto
{
    [Range(1, int.MaxValue)]
    public int AddressId { get; set; }
}