using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public class CustomerService : ICustomerService
{
    private readonly OrderManagementDbContext _context;

    public CustomerService(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerResponseDto>> GetAll()
    {
        return await _context.Customers
            .AsNoTracking()
            .Select(c => new CustomerResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                IsActive = c.IsActive
            })
            .ToListAsync();
    }

    public async Task<CustomerResponseDto> GetById(int id)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CustomerResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                IsActive = c.IsActive
            })
            .FirstOrDefaultAsync();

        if (customer is null)
        {
            throw new CustomerNotFoundException(
                $"Customer {id} was not found.");
        }

        return customer;
    }

    public async Task<CustomerResponseDto> Create(
        CreateCustomerRequestDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        _context.Customers.Add(customer);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException sqlException &&
            (sqlException.Number == 2601 ||
             sqlException.Number == 2627) &&
            sqlException.Message.Contains(
                "IX_Customers_Email",
                StringComparison.Ordinal))
        {
            throw new ConflictException(
                "A customer with this email already exists.",
                ex);
        }

        return ToResponseDto(customer);
    }

    private static CustomerResponseDto ToResponseDto(
        Customer customer)
    {
        return new CustomerResponseDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            IsActive = customer.IsActive
        };
    }
}