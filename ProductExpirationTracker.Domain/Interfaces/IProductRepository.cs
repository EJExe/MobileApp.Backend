using ProductExpirationTracker.Domain.Entities;

namespace ProductExpirationTracker.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetArchivedAsync(CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> CreateRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task DeleteArchivedAsync(CancellationToken cancellationToken = default);
    Task<Product> MarkAsUsedAsync(string id, CancellationToken cancellationToken = default);
}





