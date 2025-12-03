using ProductExpirationTracker.Application.DTOs;
using ProductExpirationTracker.Application.Interfaces;
using ProductExpirationTracker.Domain.Entities;
using ProductExpirationTracker.Domain.Interfaces;

namespace ProductExpirationTracker.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(bool? archived = null, CancellationToken cancellationToken = default)
    {
        IEnumerable<Product> products;
        
        if (archived == true)
            products = await _repository.GetArchivedAsync(cancellationToken);
        else if (archived == false)
            products = await _repository.GetActiveAsync(cancellationToken);
        else
            products = await _repository.GetAllAsync(cancellationToken);

        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = dto.Name,
            Category = dto.Category,
            PurchaseDate = dto.PurchaseDate != null ? DateTime.Parse(dto.PurchaseDate) : null,
            ExpirationDate = DateTime.Parse(dto.ExpirationDate),
            Price = dto.Price
        };

        var createdProduct = await _repository.CreateAsync(product, cancellationToken);
        return MapToDto(createdProduct);
    }

    public async Task<IEnumerable<ProductDto>> CreateRangeAsync(IEnumerable<CreateProductDto> dtos, CancellationToken cancellationToken = default)
    {
        var products = dtos.Select(dto => new Product
        {
            Name = dto.Name,
            Category = dto.Category,
            PurchaseDate = dto.PurchaseDate != null ? DateTime.Parse(dto.PurchaseDate) : null,
            ExpirationDate = DateTime.Parse(dto.ExpirationDate),
            Price = dto.Price
        }).ToList();

        var createdProducts = await _repository.CreateRangeAsync(products, cancellationToken);
        return createdProducts.Select(MapToDto);
    }

    public async Task<ProductDto> UpdateAsync(string id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with id {id} not found");

        product.Name = dto.Name;
        product.Category = dto.Category;
        product.PurchaseDate = dto.PurchaseDate != null ? DateTime.Parse(dto.PurchaseDate) : null;
        product.ExpirationDate = DateTime.Parse(dto.ExpirationDate);
        product.Price = dto.Price;

        var updatedProduct = await _repository.UpdateAsync(product, cancellationToken);
        return MapToDto(updatedProduct);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<ProductDto> MarkAsUsedAsync(string id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.MarkAsUsedAsync(id, cancellationToken);
        return MapToDto(product);
    }

    public async Task ClearHistoryAsync(CancellationToken cancellationToken = default)
    {
        await _repository.DeleteArchivedAsync(cancellationToken);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            PurchaseDate = product.PurchaseDate?.ToString("yyyy-MM-dd"),
            ExpirationDate = product.ExpirationDate.ToString("yyyy-MM-dd"),
            Price = product.Price,
            ArchivedDate = product.ArchivedDate?.ToString("yyyy-MM-dd"),
            ArchiveReason = product.ArchiveReason?.ToString().ToLower(),
            IsArchived = product.IsArchived,
            IsExpired = product.IsExpired,
            DaysUntilExpiration = product.DaysUntilExpiration
        };
    }
}





