using Microsoft.EntityFrameworkCore;
using ProductExpirationTracker.Application.DTOs;
using ProductExpirationTracker.Application.Interfaces;
using ProductExpirationTracker.Domain.Entities;
using ProductExpirationTracker.Infrastructure.Data;

namespace ProductExpirationTracker.Infrastructure.Services;

public class StatsService : IStatsService
{
    private readonly ApplicationDbContext _db;

    public StatsService(ApplicationDbContext db) => _db = db;

    public async Task<StatsResponseDto> GetStatsAsync(string? userId, DateTime from, DateTime to, string granularity)
    {
        // Note: current Product entity does not have UserId; the userId parameter is returned in the response but not used to filter products.
        from = from.Date;
        to = to.Date;

        // summary
        var totalProducts = await _db.Products.CountAsync();
        var expiredCount = await _db.Products.CountAsync(p => p.ArchiveReason == ArchiveReason.Expired);
        var expiring7Days = await _db.Products.CountAsync(p => p.ArchivedDate == null && p.ExpirationDate >= DateTime.UtcNow.Date && p.ExpirationDate <= DateTime.UtcNow.Date.AddDays(7));
        var consumedCount = await _db.Products.CountAsync(p => p.ArchiveReason == ArchiveReason.Used);
        var discardedCount = 0; // no explicit discarded state in current model

        var summary = new StatsSummaryDto
        {
            TotalProducts = totalProducts,
            ExpiredCount = expiredCount,
            Expiring7Days = expiring7Days,
            ConsumedCount = consumedCount,
            DiscardedCount = discardedCount
        };

        // series
        var archivedInRangeQuery = _db.Products
            .Where(p => p.ArchivedDate != null && p.ArchivedDate.Value.Date >= from && p.ArchivedDate.Value.Date <= to.Date);

        var series = new List<StatPointDto>();

        if (granularity?.ToLower() == "day")
        {
            // Materialize minimal fields to avoid provider limitations (SQLite SUM on decimal)
            var items = await archivedInRangeQuery
                .Select(p => new { Date = p.ArchivedDate!.Value.Date, p.Price, p.ArchiveReason })
                .ToListAsync();

            var grouped = items
                .GroupBy(p => p.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    ExpiredCount = g.Count(p => p.ArchiveReason == ArchiveReason.Expired),
                    Cost = g.Sum(p => p.Price ?? 0m),
                    ConsumedCount = g.Count(p => p.ArchiveReason == ArchiveReason.Used)
                })
                .ToList();

            var cur = from;
            while (cur <= to)
            {
                var row = grouped.FirstOrDefault(g => g.Date == cur);
                series.Add(new StatPointDto
                {
                    Date = cur,
                    ExpiredCount = row?.ExpiredCount ?? 0,
                    Cost = row?.Cost ?? 0m,
                    ConsumedCount = row?.ConsumedCount ?? 0
                });
                cur = cur.AddDays(1);
            }
        }
        else if (granularity?.ToLower() == "week")
        {
            // For week granularity we'll materialize minimal data then group by week-start (Monday)
            var items = await archivedInRangeQuery
                .Select(p => new { Date = p.ArchivedDate!.Value.Date, p.Price, p.ArchiveReason })
                .ToListAsync();

            DateTime WeekStart(DateTime dt)
            {
                var diff = (int)dt.DayOfWeek - 1; // Monday = 1
                if (diff < 0) diff = 6; // Sunday -> previous Monday
                return dt.AddDays(-diff).Date;
            }

            var grouped = items
                .GroupBy(x => WeekStart(x.Date))
                .Select(g => new
                {
                    Date = g.Key,
                    ExpiredCount = g.Count(x => x.ArchiveReason == ArchiveReason.Expired),
                    Cost = g.Sum(x => x.Price ?? 0),
                    ConsumedCount = g.Count(x => x.ArchiveReason == ArchiveReason.Used)
                })
                .ToList();

            // build continuous weeks
            var cur = WeekStart(from);
            var end = WeekStart(to);
            while (cur <= end)
            {
                var row = grouped.FirstOrDefault(g => g.Date == cur);
                series.Add(new StatPointDto
                {
                    Date = cur,
                    ExpiredCount = row?.ExpiredCount ?? 0,
                    Cost = row?.Cost ?? 0m,
                    ConsumedCount = row?.ConsumedCount ?? 0
                });
                cur = cur.AddDays(7);
            }
        }
        else // month
        {
            // Materialize minimal fields to avoid provider limitations (SQLite SUM on decimal)
            var items = await archivedInRangeQuery
                .Select(p => new { Date = p.ArchivedDate!.Value.Date, p.Price, p.ArchiveReason })
                .ToListAsync();

            var grouped = items
                .GroupBy(p => new { Year = p.Date.Year, Month = p.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    ExpiredCount = g.Count(p => p.ArchiveReason == ArchiveReason.Expired),
                    Cost = g.Sum(p => p.Price ?? 0m),
                    ConsumedCount = g.Count(p => p.ArchiveReason == ArchiveReason.Used)
                })
                .ToList();

            var cur = new DateTime(from.Year, from.Month, 1);
            var end = new DateTime(to.Year, to.Month, 1);
            while (cur <= end)
            {
                var row = grouped.FirstOrDefault(g => g.Year == cur.Year && g.Month == cur.Month);
                series.Add(new StatPointDto
                {
                    Date = cur,
                    ExpiredCount = row?.ExpiredCount ?? 0,
                    Cost = row?.Cost ?? 0m,
                    ConsumedCount = row?.ConsumedCount ?? 0
                });
                cur = cur.AddMonths(1);
            }
        }

        // by category (top categories by expired count)
        var byCategory = await _db.Products
            .Where(p => p.ArchiveReason == ArchiveReason.Expired)
            .GroupBy(p => p.Category)
            .Select(g => new CategoryStatDto
            {
                Category = g.Key ?? "Unknown",
                Count = g.Count(),
                ExpiredCount = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        return new StatsResponseDto
        {
            UserId = userId,
            From = from,
            To = to,
            Granularity = granularity ?? "month",
            Summary = summary,
            Series = series,
            ByCategory = byCategory
        };
    }
}
