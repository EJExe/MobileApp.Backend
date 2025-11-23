using FluentAssertions;
using Moq;
using ProductExpirationTracker.Application.DTOs;
using ProductExpirationTracker.Application.Services;
using ProductExpirationTracker.Domain.Entities;
using ProductExpirationTracker.Domain.Interfaces;
using Xunit;

namespace ProductExpirationTracker.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new ProductService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnProductDto()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Молоко",
            Category = "Молочные продукты",
            ExpirationDate = "2024-12-31",
            Price = 100
        };

        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            Category = dto.Category,
            ExpirationDate = DateTime.Parse(dto.ExpirationDate),
            Price = dto.Price
        };

        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Category.Should().Be(dto.Category);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProductDto()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();
        var product = new Product
        {
            Id = productId,
            Name = "Хлеб",
            Category = "Хлеб и выпечка",
            ExpirationDate = DateTime.UtcNow.AddDays(3)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductNotExists_ShouldReturnNull()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();
        _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        result.Should().BeNull();
    }
}




