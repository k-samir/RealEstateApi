using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApi.Application.DTOs;
using RealEstateApi.Infrastructure.Persistence;
using Xunit;

namespace RealEstateApi.IntegrationTests.Controllers;

public class PropertiesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PropertiesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add DbContext using in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProperties_WithoutAuth_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/properties");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedPropertiesResponse>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetProperties_WithFilters_ReturnsFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/properties?location=Dubai&type=Apartment&page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedPropertiesResponse>();
        Assert.NotNull(result);
        Assert.True(result.Page == 1);
    }

    [Fact]
    public async Task GetPropertyById_NonExisting_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/properties/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProperty_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new CreatePropertyDto
        {
            Name = "Test Property",
            Location = "Dubai",
            Type = "Apartment",
            Description = "Test description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/properties", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyProperties_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/properties/my-properties");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProperty_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Name = "Updated Property"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/properties/1", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProperty_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/properties/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PublishProperty_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/properties/1/publish", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", content);
    }
}
