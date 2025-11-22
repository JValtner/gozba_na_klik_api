using Gozba_na_klik.DTOs.Complaints;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gozba_na_klik.Models
{
    public interface IComplaintRepository
    {
        Task<ComplaintResponseDto> InsertComplaintAsync(CreateComplaintDto dto, int userId, int restaurantId);
        Task<bool> ComplaintExistsForOrderAsync(int orderId);
        Task<bool> HasComplaintForOrderAsync(int orderId, int userId);
        Task<ComplaintResponseDto> GetComplaintByOrderIdAndUserIdAsync(int orderId, int userId);
        Task<(List<ComplaintResponseDto> complaints, int totalCount)> GetAllComplaintsLast30DaysAsync(int page, int pageSize);
        Task<ComplaintResponseDto> GetComplaintByIdAsync(string complaintId);
    }
}

