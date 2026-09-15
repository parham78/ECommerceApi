public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public ICollection<Order> Orders { get; set; }
        = new List<Order>();

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}