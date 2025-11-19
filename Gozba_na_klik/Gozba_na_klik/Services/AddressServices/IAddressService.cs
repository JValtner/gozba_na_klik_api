using System.Collections.Generic;
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Addresses;

namespace Gozba_na_klik.Services.AddressServices
{
    public interface IAddressService
    {
        Task<List<AddressListItemDto>> GetMyAsync(int userId);
        Task<AddressListItemDto> CreateAsync(int userId, AddressCreateDto dto);
        Task UpdateAsync(int userId, int id, AddressUpdateDto dto);
        Task SetDefaultAsync(int userId, int id);
        Task DeleteAsync(int userId, int id);
    }
}
