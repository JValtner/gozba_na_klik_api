using System.Collections.Generic;
using System.Threading.Tasks;
using Gozba_na_klik.Models.Customers;

namespace Gozba_na_klik.Repositories.AddressRepositories
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetMyAsync(int userId);
        Task<Address?> GetByIdAsync(int id);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(Address address);
        Task SaveAsync();
    }
}
