namespace ProductExpirationTracker.Application.DTOs;

public class CategoryStatDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public int ExpiredCount { get; set; }
}
