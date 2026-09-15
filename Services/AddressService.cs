using Microsoft.EntityFrameworkCore;

public class AddressService : IAddressService
{
    private readonly OrderManagementDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddressService(
        OrderManagementDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<AddressResponseDto>> GetMyAddresses()
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId.Value)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Id)
            .Select(a => new AddressResponseDto
            {
                Id = a.Id,
                Label = a.Label,
                RecipientName = a.RecipientName,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                Province = a.Province,
                PostalCode = a.PostalCode,
                Country = a.Country,
                PhoneNumber = a.PhoneNumber,
                IsDefault = a.IsDefault
            })
            .ToListAsync();
    }

    public async Task<AddressResponseDto> GetMyAddressById(int id)
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        var address = await _context.Addresses
            .AsNoTracking()
            .Where(a =>
                a.Id == id &&
                a.CustomerId == customerId.Value)
            .Select(a => new AddressResponseDto
            {
                Id = a.Id,
                Label = a.Label,
                RecipientName = a.RecipientName,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                Province = a.Province,
                PostalCode = a.PostalCode,
                Country = a.Country,
                PhoneNumber = a.PhoneNumber,
                IsDefault = a.IsDefault
            })
            .FirstOrDefaultAsync();

        if (address == null)
        {
            throw new AddressNotFoundException(
                $"Address {id} was not found.");
        }

        return address;
    }

    public async Task<AddressResponseDto> Create(
        CreateAddressRequestDto dto)
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        var hasAnyAddress = await _context.Addresses
            .AnyAsync(a =>
                a.CustomerId == customerId.Value);

        var shouldBeDefault = dto.IsDefault;

        if (!hasAnyAddress)
        {
            shouldBeDefault = true;
        }

        if (shouldBeDefault)
        {
            var currentDefaultAddresses =
                await _context.Addresses
                    .Where(a =>
                        a.CustomerId == customerId.Value &&
                        a.IsDefault)
                    .ToListAsync();

            foreach (var currentDefaultAddress
                     in currentDefaultAddresses)
            {
                currentDefaultAddress.IsDefault = false;
            }
        }

        var address = new Address
        {
            CustomerId = customerId.Value,
            Label = dto.Label,
            RecipientName = dto.RecipientName,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            Province = dto.Province,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            PhoneNumber = dto.PhoneNumber,
            IsDefault = shouldBeDefault
        };

        _context.Addresses.Add(address);

        await _context.SaveChangesAsync();

        return MapToDto(address);
    }

    public async Task<AddressResponseDto> Update(
    int id,
    UpdateAddressRequestDto dto)
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                a.CustomerId == customerId.Value);

        if (address == null)
        {
            throw new AddressNotFoundException(
                $"Address {id} was not found.");
        }

        // Selecting this address as default clears the previous default.
        if (dto.IsDefault)
        {
            var otherDefaultAddresses = await _context.Addresses
                .Where(a =>
                    a.CustomerId == customerId.Value &&
                    a.Id != id &&
                    a.IsDefault)
                .ToListAsync();

            foreach (var otherAddress in otherDefaultAddresses)
            {
                otherAddress.IsDefault = false;
            }
        }

        var isDefault = dto.IsDefault;

        // Keep the current default until another address is selected.
        if (address.IsDefault && !dto.IsDefault)
        {
            isDefault = true;
        }

        address.Label = dto.Label;
        address.RecipientName = dto.RecipientName;
        address.AddressLine1 = dto.AddressLine1;
        address.AddressLine2 = dto.AddressLine2;
        address.City = dto.City;
        address.Province = dto.Province;
        address.PostalCode = dto.PostalCode;
        address.Country = dto.Country;
        address.PhoneNumber = dto.PhoneNumber;
        address.IsDefault = isDefault;

        await _context.SaveChangesAsync();

        return MapToDto(address);
    }

    public async Task Delete(int id)
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                a.CustomerId == customerId.Value);

        if (address == null)
        {
            throw new AddressNotFoundException(
                $"Address {id} was not found.");
        }

        var wasDefault = address.IsDefault;

        _context.Addresses.Remove(address);

        if (wasDefault)
        {
            var nextAddress = await _context.Addresses
                .Where(a =>
                    a.CustomerId == customerId.Value &&
                    a.Id != id)
                .OrderBy(a => a.Id)
                .FirstOrDefaultAsync();

            if (nextAddress != null)
            {
                nextAddress.IsDefault = true;
            }
        }

        await _context.SaveChangesAsync();
    }

    private static AddressResponseDto MapToDto(
        Address address)
    {
        return new AddressResponseDto
        {
            Id = address.Id,
            Label = address.Label,
            RecipientName = address.RecipientName,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            Province = address.Province,
            PostalCode = address.PostalCode,
            Country = address.Country,
            PhoneNumber = address.PhoneNumber,
            IsDefault = address.IsDefault
        };
    }
}