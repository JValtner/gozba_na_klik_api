using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Services.Pdf
{
    public interface IPdfReportService
    {
        // A) List stored snapshots
        Task<List<PdfMonthlyReportDocument>> ListSnapshotsAsync(
            int restaurantId, int? year, int? month);

        // B) Generate and store previous month snapshot
        Task<string> GenerateAndStorePreviousMonthSnapshotAsync(
            int restaurantId, DateTime nowUtc);

        // C) Download stored snapshot PDF
        Task<byte[]> GeneratePdfFromSnapshotAsync(string snapshotId);

        // D) On-demand monthly PDF (not stored)
        Task<byte[]> GenerateOnDemandMonthlyPdfAsync(OnDemandMonthlyReportRequest request);
    }
}
