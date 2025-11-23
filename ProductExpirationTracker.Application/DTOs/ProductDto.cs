namespace ProductExpirationTracker.Application.DTOs;

public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? PurchaseDate { get; set; }
    public string ExpirationDate { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? ArchivedDate { get; set; }
    public string? ArchiveReason { get; set; }
    public bool IsArchived { get; set; }
    public bool IsExpired { get; set; }
    public int DaysUntilExpiration { get; set; }
}




