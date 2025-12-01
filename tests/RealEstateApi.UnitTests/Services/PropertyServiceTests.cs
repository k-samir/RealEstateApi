using Moq;
using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Exceptions;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Application.Services;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Enums;
using Xunit;

namespace RealEstateApi.UnitTests.Services;

public class PropertyServiceTests
{
    private readonly Mock<IPropertyRepository> _mockRepository;
    private readonly PropertyService _service;

    public PropertyServiceTests()
    {
        _mockRepository = new Mock<IPropertyRepository>();
        _service = new PropertyService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetPropertyByIdAsync_ExistingProperty_ReturnsPropertyDto()
    {
        // Arrange
        var property = Property.Create("agent-123", "Test Property", "Dubai", "Apartment", "Test description");
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);

        // Act
        var result = await _service.GetPropertyByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Property", result.Name);
        Assert.Equal("Dubai", result.Location);
    }

    [Fact]
    public async Task GetPropertyByIdAsync_NonExistingProperty_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Property?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPropertyByIdAsync(999));
    }

    [Fact]
    public async Task GetAllPropertiesAsync_ReturnsPagedResponse()
    {
        // Arrange
        var properties = new List<Property>
        {
            Property.Create("agent-123", "Property 1", "Dubai", "Apartment", "Desc 1"),
            Property.Create("agent-123", "Property 2", "Abu Dhabi", "Villa", "Desc 2")
        };

        var filter = new PropertyFilter { Page = 1, PageSize = 10 };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<PropertyFilter>(), It.IsAny<CancellationToken>())).ReturnsAsync((properties, 2));

        // Act
        var result = await _service.GetAllPropertiesAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Projects.Count);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task CreatePropertyAsync_ValidDto_CreatesProperty()
    {
        // Arrange
        var dto = new CreatePropertyDto
        {
            Name = "New Property",
            Location = "Dubai Marina",
            Type = "Apartment",
            Description = "Luxury apartment",
            IsPublished = false
        };

        var createdProperty = Property.Create("agent-123", dto.Name, dto.Location, dto.Type, dto.Description);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdProperty);

        // Act
        var result = await _service.CreatePropertyAsync(dto, "agent-123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Location, result.Location);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePropertyAsync_AsOwner_UpdatesProperty()
    {
        // Arrange
        var property = Property.Create("agent-123", "Old Name", "Dubai", "Apartment", "Old description");
        var dto = new UpdatePropertyDto
        {
            Name = "Updated Name",
            Description = "Updated description"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);

        // Act
        var result = await _service.UpdatePropertyAsync(1, dto, "agent-123", null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePropertyAsync_AsNonOwner_ThrowsForbiddenException()
    {
        // Arrange
        var property = Property.Create("agent-123", "Test Property", "Dubai", "Apartment", "Test");
        var dto = new UpdatePropertyDto { Name = "Updated Name" };

        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.UpdatePropertyAsync(1, dto, "different-agent", null));
    }

    [Fact]
    public async Task UpdatePropertyAsync_AsAdmin_UpdatesAnyProperty()
    {
        // Arrange
        var property = Property.Create("agent-123", "Old Name", "Dubai", "Apartment", "Old description");
        var dto = new UpdatePropertyDto { Name = "Updated Name" };

        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);

        // Act
        var result = await _service.UpdatePropertyAsync(1, dto, "admin-user", "admin");

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletePropertyAsync_AsOwner_DeletesProperty()
    {
        // Arrange
        var property = Property.Create("agent-123", "Test Property", "Dubai", "Apartment", "Test");
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeletePropertyAsync(1, "agent-123", null);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletePropertyAsync_AsNonOwner_ThrowsForbiddenException()
    {
        // Arrange
        var property = Property.Create("agent-123", "Test Property", "Dubai", "Apartment", "Test");
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.DeletePropertyAsync(1, "different-agent", null));
    }

    [Fact]
    public async Task PublishPropertyAsync_AsOwner_PublishesProperty()
    {
        // Arrange
        var property = Property.Create("agent-123", "Test Property", "Dubai", "Apartment", "Test");
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);

        // Act
        var result = await _service.PublishPropertyAsync(1, "agent-123");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsPublished);
        Assert.Equal("Published", result.Status);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishPropertyAsync_AsNonOwner_ThrowsForbiddenException()
    {
        // Arrange
        var property = Property.Create("agent-123", "Test Property", "Dubai", "Apartment", "Test");
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.PublishPropertyAsync(1, "different-agent"));
    }

    [Fact]
    public async Task GetAgentPropertiesAsync_ReturnsPropertiesForAgent()
    {
        // Arrange
        var properties = new List<Property>
        {
            Property.Create("agent-123", "Property 1", "Dubai", "Apartment", "Desc 1"),
            Property.Create("agent-123", "Property 2", "Abu Dhabi", "Villa", "Desc 2")
        };

        _mockRepository.Setup(r => r.GetByAgentIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(properties);

        // Act
        var result = await _service.GetAgentPropertiesAsync("agent-123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal("agent-123", p.AgentId));
    }
}
