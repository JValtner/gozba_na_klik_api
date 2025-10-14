using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Employee;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Services.EmployeeServices
{
    public interface IEmployeeService
    {
        Task<IEnumerable<User>> GetEmployeesByRestaurantAsync(int restaurantId, int ownerId);
        Task<User> RegisterEmployeeAsync(int restaurantId, int ownerId, RegisterEmployeeDto dto);
        Task<User> UpdateEmployeeAsync(int employeeId, int ownerId, UpdateEmployeeDto dto);
        Task SuspendEmployeeAsync(int employeeId, int ownerId);
        Task ActivateEmployeeAsync(int employeeId, int ownerId);
    }
}