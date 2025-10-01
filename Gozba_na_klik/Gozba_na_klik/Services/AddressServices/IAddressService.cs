using System.Collections.Generic;
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Models.Customers;

namespace Gozba_na_klik.Services.AddressServices
{
    public interface IAddressService
    {
        Task<List<Address>> GetMyAsync(int userId);
        Task<Address> CreateAsync(int userId, AddressCreateDto dto);
        Task<Address> UpdateAsync(int userId, int id, AddressUpdateDto dto);
        Task SetDefaultAsync(int userId, int id);
        Task DeleteAsync(int userId, int id);
    }
}
