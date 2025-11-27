using Gozba_na_klik.DTOs.Request;

namespace Gozba_na_klik.Services.Pdf
{
    public interface IPdfRenderer
    {
        byte[] Render(MonthlyReportDTO dto);
    }
}
