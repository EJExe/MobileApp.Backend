namespace ProductExpirationTracker.Domain.Entities;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string Name { get; set; } = string.Empty;
    
    public string Category { get; set; } = string.Empty;
    
    public DateTime? PurchaseDate { get; set; }
    
    public DateTime ExpirationDate { get; set; }
    
    public decimal? Price { get; set; }
    
    public DateTime? ArchivedDate { get; set; }
    
    public ArchiveReason? ArchiveReason { get; set; }
    
    public bool IsArchived => ArchivedDate != null;
    
    public bool IsExpired => DateTime.UtcNow.Date > ExpirationDate.Date;
    
    public int DaysUntilExpiration => (ExpirationDate.Date - DateTime.UtcNow.Date).Days;
}





