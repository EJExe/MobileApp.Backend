using Microsoft.EntityFrameworkCore;
using ProductExpirationTracker.Domain.Entities;
using ProductExpirationTracker.Domain.Interfaces;
using ProductExpirationTracker.Infrastructure.Data;

namespace ProductExpirationTracker.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => !p.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetArchivedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<IEnumerable<Product>> CreateRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        var productList = products.ToList();
        _context.Products.AddRange(productList);
        await _context.SaveChangesAsync(cancellationToken);
        return productList;
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteArchivedAsync(CancellationToken cancellationToken = default)
    {
        var archivedProducts = await GetArchivedAsync(cancellationToken);
        _context.Products.RemoveRange(archivedProducts);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product> MarkAsUsedAsync(string id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with id {id} not found");

        product.ArchivedDate = DateTime.UtcNow;
        product.ArchiveReason = ArchiveReason.Used;

        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }
}




