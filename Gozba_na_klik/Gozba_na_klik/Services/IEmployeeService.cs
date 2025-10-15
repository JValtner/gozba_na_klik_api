using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Employee;

namespace Gozba_na_klik.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeListItemDto>> GetEmployeesByRestaurantAsync(int restaurantId, int ownerId);
        Task<EmployeeListItemDto> RegisterEmployeeAsync(int restaurantId, int ownerId, RegisterEmployeeDto dto);
        Task<EmployeeListItemDto> UpdateEmployeeAsync(int employeeId, int ownerId, UpdateEmployeeDto dto);
        Task SuspendEmployeeAsync(int employeeId, int ownerId);
        Task ActivateEmployeeAsync(int employeeId, int ownerId);
    }
}