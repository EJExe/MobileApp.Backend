using ProductExpirationTracker.Application.DTOs;

namespace ProductExpirationTracker.Application.Interfaces;

public interface IStatsService
{
    Task<StatsResponseDto> GetStatsAsync(string? userId, DateTime from, DateTime to, string granularity);
}
