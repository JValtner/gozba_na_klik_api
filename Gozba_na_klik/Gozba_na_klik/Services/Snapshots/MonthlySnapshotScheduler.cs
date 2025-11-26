using System;
using System.Linq;
using System.Threading;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services.Pdf;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Services.Snapshots
{
    public class PdfSnapshotScheduler : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<PdfSnapshotScheduler> _logger;

        public PdfSnapshotScheduler(IServiceProvider sp, ILogger<PdfSnapshotScheduler> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PdfSnapshotScheduler started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRun = NextRunUtc(now);
                var delay = nextRun - now;
                if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;

                _logger.LogInformation("Next snapshot run at {nextRunUtc}", nextRun);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException) { break; }

                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<GozbaNaKlikDbContext>();
                var pdfService = scope.ServiceProvider.GetRequiredService<IPdfReportService>();

                var restaurantIds = await db.Restaurants.Select(r => r.Id).ToListAsync(stoppingToken);

                foreach (var rid in restaurantIds)
                {
                    try
                    {
                        await pdfService.GenerateAndStorePreviousMonthSnapshotAsync(rid, DateTime.UtcNow);
                        _logger.LogInformation("Snapshot generation attempted for restaurant {rid}", rid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to generate snapshot for restaurant {rid}", rid);
                    }
                }
            }

            _logger.LogInformation("PdfSnapshotScheduler stopped");
        }

        private static DateTime NextRunUtc(DateTime nowUtc)
        {
            // Run at first second of next month UTC
            var firstOfThisMonth = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 1, DateTimeKind.Utc);
            if (nowUtc < firstOfThisMonth) return firstOfThisMonth;
            return firstOfThisMonth.AddMonths(1);
        }
    }
}
