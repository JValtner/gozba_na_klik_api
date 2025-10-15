using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Repositories
{
    public class AddressDbRepository : IAddressRepository
    {
        private readonly GozbaNaKlikDbContext _ctx;

        public AddressDbRepository(GozbaNaKlikDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<List<Address>> GetMyAsync(int userId)
        {
            return _ctx.Addresses
                .Where(a => a.UserId == userId && a.IsActive)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.Id)
                .ToListAsync();
        }

        public Task<Address?> GetByIdAsync(int id)
        {
            return _ctx.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        }

        public async Task AddAsync(Address address)
        {
            await _ctx.Addresses.AddAsync(address);
        }

        public Task UpdateAsync(Address address)
        {
            _ctx.Addresses.Update(address);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Address address)
        {
            address.IsActive = false;
            _ctx.Addresses.Update(address);
            return Task.CompletedTask;
        }

        public Task SaveAsync()
        {
            return _ctx.SaveChangesAsync();
        }
    }
}
