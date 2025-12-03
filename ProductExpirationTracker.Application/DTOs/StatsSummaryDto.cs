namespace ProductExpirationTracker.Application.DTOs;

public class StatsSummaryDto
{
    public int TotalProducts { get; set; }
    public int ExpiredCount { get; set; }
    public int Expiring7Days { get; set; }
    public int ConsumedCount { get; set; }
    public int DiscardedCount { get; set; }
}
