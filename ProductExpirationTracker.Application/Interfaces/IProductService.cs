using ProductExpirationTracker.Application.DTOs;

namespace ProductExpirationTracker.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllAsync(bool? archived = null, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> CreateRangeAsync(IEnumerable<CreateProductDto> dtos, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateAsync(string id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<ProductDto> MarkAsUsedAsync(string id, CancellationToken cancellationToken = default);
    Task ClearHistoryAsync(CancellationToken cancellationToken = default);
}





