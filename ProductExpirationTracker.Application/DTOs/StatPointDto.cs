namespace ProductExpirationTracker.Application.DTOs;

public class StatPointDto
{
    // For month granularity: first day of month
    public DateTime Date { get; set; }
    public int ExpiredCount { get; set; }
    public decimal Cost { get; set; }
    public int ConsumedCount { get; set; }
}
