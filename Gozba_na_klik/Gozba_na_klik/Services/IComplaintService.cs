using Gozba_na_klik.DTOs.Complaints;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gozba_na_klik.Services
{
    public interface IComplaintService
    {
        Task<ComplaintResponseDto> CreateComplaintAsync(CreateComplaintDto dto, int userId);
        Task<List<ComplaintResponseDto>> GetComplaintsByRestaurantIdAsync(int restaurantId);
        Task<List<ComplaintResponseDto>> GetComplaintsByOwnerIdAsync(int ownerId);
        Task<bool> HasComplaintForOrderAsync(int orderId, int userId);
        Task<ComplaintResponseDto> GetComplaintByOrderIdAsync(int orderId, int userId);
    }
}

