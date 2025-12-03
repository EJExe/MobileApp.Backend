using System.Collections.Generic;

namespace ProductExpirationTracker.Application.DTOs;

public class StatsResponseDto
{
    public string? UserId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Granularity { get; set; } = "month";
    public StatsSummaryDto Summary { get; set; } = new StatsSummaryDto();
    public List<StatPointDto> Series { get; set; } = new();
    public List<CategoryStatDto>? ByCategory { get; set; }
}
