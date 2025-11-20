using Gozba_na_klik.DTOs.Complaints;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gozba_na_klik.Models
{
    public interface IComplaintRepository
    {
        Task<ComplaintResponseDto> InsertComplaintAsync(CreateComplaintDto dto, int userId, int restaurantId);
        Task<bool> ComplaintExistsForOrderAsync(int orderId);
        Task<List<ComplaintResponseDto>> GetComplaintsByRestaurantIdAsync(int restaurantId);
        Task<List<ComplaintResponseDto>> GetComplaintsByRestaurantIdsAsync(List<int> restaurantIds);
        Task<bool> HasComplaintForOrderAsync(int orderId, int userId);
        Task<ComplaintResponseDto> GetComplaintByOrderIdAndUserIdAsync(int orderId, int userId);
    }
}

