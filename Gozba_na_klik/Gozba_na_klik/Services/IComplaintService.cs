using Gozba_na_klik.DTOs.Complaints;
using Gozba_na_klik.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gozba_na_klik.Services
{
    public interface IComplaintService
    {
        Task<ComplaintResponseDto> CreateComplaintAsync(CreateComplaintDto dto, int userId);
        Task<bool> HasComplaintForOrderAsync(int orderId, int userId);
        Task<ComplaintResponseDto> GetComplaintByOrderIdAsync(int orderId, int userId);
        Task<PaginatedList<ComplaintResponseDto>> GetAllComplaintsLast30DaysAsync(int page, int pageSize);
        Task<ComplaintResponseDto> GetComplaintByIdAsync(string complaintId);
    }
}

