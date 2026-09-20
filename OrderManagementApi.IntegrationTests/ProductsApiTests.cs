using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;

public class ProductsApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductsApiTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetById_WhenProductDoesNotExist_ReturnsNotFoundProblemDetails()
    {
        // Act
        var response =
            await _client.GetAsync(
                "/api/products/999999");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem =
            await response.Content
                .ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            problem.Status);

        Assert.Equal(
            "Not Found",
            problem.Title);

        Assert.Equal(
            "Product 999999 was not found.",
            problem.Detail);
    }
    [Fact]
    public async Task GetAllForAdmin_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response =
            await _client.GetAsync(
                "/api/products/admin");

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        int productId;

        using (var scope =
            _factory.Services.CreateScope())
        {
            var context =
                scope.ServiceProvider
                    .GetRequiredService<
                        OrderManagementDbContext>();

            var product = new Product
            {
                Name = "Integration Test Keyboard",
                Sku = $"TEST-{Guid.NewGuid():N}",
                Price = 129.99m,
                Stock = 15,
                IsActive = true
            };

            context.Products.Add(product);

            await context.SaveChangesAsync();

            productId = product.Id;
        }

        try
        {
            // Act
            var response =
                await _client.GetAsync(
                    $"/api/products/{productId}");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var product =
                await response.Content
                    .ReadFromJsonAsync<ProductResponseDto>();

            Assert.NotNull(product);

            Assert.Equal(
                productId,
                product.Id);

            Assert.Equal(
                "Integration Test Keyboard",
                product.Name);

            Assert.Equal(
                129.99m,
                product.Price);

            Assert.Equal(
                15,
                product.Stock);
        }
        finally
        {
            using var scope =
                _factory.Services.CreateScope();

            var context =
                scope.ServiceProvider
                    .GetRequiredService<
                        OrderManagementDbContext>();

            var product =
                await context.Products
                    .FindAsync(productId);

            if (product != null)
            {
                context.Products.Remove(product);

                await context.SaveChangesAsync();
            }
        }
    }
    [Fact]
    public async Task GetById_WhenProductIsInactive_ReturnsNotFound()
    {
        // Arrange
        int productId;

        using (var scope =
            _factory.Services.CreateScope())
        {
            var context =
                scope.ServiceProvider
                    .GetRequiredService<
                        OrderManagementDbContext>();

            var product = new Product
            {
                Name = "Inactive Integration Product",
                Sku = $"INACTIVE-{Guid.NewGuid():N}",
                Price = 50m,
                Stock = 10,
                IsActive = false
            };

            context.Products.Add(product);

            await context.SaveChangesAsync();

            productId = product.Id;
        }

        try
        {
            // Act
            var response =
                await _client.GetAsync(
                    $"/api/products/{productId}");

            // Assert
            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }
        finally
        {
            using var scope =
                _factory.Services.CreateScope();

            var context =
                scope.ServiceProvider
                    .GetRequiredService<
                        OrderManagementDbContext>();

            var product =
                await context.Products
                    .FindAsync(productId);

            if (product != null)
            {
                context.Products.Remove(product);

                await context.SaveChangesAsync();
            }
        }
    }
    [Fact]
    public async Task GetAll_ReturnsOnlyActiveProducts()
    {
        // Arrange
        int activeId;
        int inactiveId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            var activeProduct = new Product
            {
                Name = "Active Integration Product",
                Sku = $"ACTIVE-{Guid.NewGuid():N}",
                Price = 75m,
                Stock = 5,
                IsActive = true
            };

            var inactiveProduct = new Product
            {
                Name = "Hidden Integration Product",
                Sku = $"HIDDEN-{Guid.NewGuid():N}",
                Price = 80m,
                Stock = 5,
                IsActive = false
            };

            context.Products.AddRange(
                activeProduct,
                inactiveProduct);

            await context.SaveChangesAsync();

            activeId = activeProduct.Id;
            inactiveId = inactiveProduct.Id;
        }

        try
        {
            // Act
            var response = await _client.GetAsync(
                "/api/products?page=1&pageSize=100");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var result =
                await response.Content
                    .ReadFromJsonAsync<
                        PagedResultDto<ProductResponseDto>>();

            Assert.NotNull(result);

            Assert.Contains(
                result.Items,
                p => p.Id == activeId);

            Assert.DoesNotContain(
                result.Items,
                p => p.Id == inactiveId);
        }
        finally
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            var products = await context.Products
                .Where(p =>
                    p.Id == activeId ||
                    p.Id == inactiveId)
                .ToListAsync();

            context.Products.RemoveRange(products);

            await context.SaveChangesAsync();
        }
    }
    [Fact]
    public async Task GetAdminProduct_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response =
            await _client.GetAsync(
                "/api/products/admin/999999");

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
    private async Task<string> LoginAsync(
    string email,
    string password)
    {
        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequestDto
                {
                    Email = email,
                    Password = password
                });

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var loginResponse =
            await response.Content
                .ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(loginResponse);

        Assert.False(
            string.IsNullOrWhiteSpace(
                loginResponse.Token));

        return loginResponse.Token;
    }
    private async Task<(string Token, string Email)>
    RegisterAndLoginCustomerAsync()
    {
        var email =
            $"customer-{Guid.NewGuid():N}@test.local";

        const string password =
            "Customer123!";

        var registerResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/register",
                new RegisterRequestDto
                {
                    Name = "Integration Customer",
                    Email = email,
                    Password = password
                });

        Assert.Equal(
            HttpStatusCode.OK,
            registerResponse.StatusCode);

        var token =
            await LoginAsync(
                email,
                password);

        return (token, email);
    }
    private async Task DeleteCustomerAsync(
    string email)
    {
        using var scope =
            _factory.Services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<
                    OrderManagementDbContext>();

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<
                    UserManager<ApplicationUser>>();

        var customer =
            await context.Customers
                .FirstOrDefaultAsync(
                    c => c.Email == email);

        if (customer != null)
        {
            context.Customers.Remove(customer);

            await context.SaveChangesAsync();
        }

        var user =
            await userManager
                .FindByEmailAsync(email);

        if (user != null)
        {
            await userManager.DeleteAsync(user);
        }
    }
    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequestDto
                {
                    Email =
                        "does-not-exist@test.local",

                    Password =
                        "WrongPassword123!"
                });

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
    [Fact]
    public async Task GetAllForAdmin_WithCustomerToken_ReturnsForbidden()
    {
        // Arrange
        var (token, email) =
            await RegisterAndLoginCustomerAsync();

        try
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    "/api/products/admin");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            // Act
            var response =
                await _client.SendAsync(request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }
        finally
        {
            await DeleteCustomerAsync(email);
        }
    }
    [Fact]
    public async Task GetAllForAdmin_WithAdminToken_ReturnsOk()
    {
        // Arrange
        var token =
            await LoginAsync(
                CustomWebApplicationFactory
                    .TestAdminEmail,

                CustomWebApplicationFactory
                    .TestAdminPassword);

        using var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                "/api/products/admin");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        // Act
        var response =
            await _client.SendAsync(request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}