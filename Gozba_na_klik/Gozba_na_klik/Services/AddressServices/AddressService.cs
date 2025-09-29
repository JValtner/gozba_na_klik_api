using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Models.Customers;
using Gozba_na_klik.Repositories.AddressRepositories;
using Microsoft.EntityFrameworkCore;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Services.AddressServices
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repo;
        private readonly GozbaNaKlikDbContext _ctx; // a default beállításhoz praktikus

        public AddressService(IAddressRepository repo, GozbaNaKlikDbContext ctx)
        {
            _repo = repo;
            _ctx = ctx;
        }

        public Task<List<Address>> GetMyAsync(int userId)
        {
            return _repo.GetMyAsync(userId);
        }

        public async Task<Address> CreateAsync(int userId, AddressCreateDto dto)
        {
            Address address = new Address
            {
                UserId = userId,
                Label = dto.Label,
                Street = dto.Street,
                City = dto.City,
                PostalCode = dto.PostalCode,
                Entrance = dto.Entrance,
                Floor = dto.Floor,
                Apartment = dto.Apartment,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Notes = dto.Notes,
                IsDefault = dto.IsDefault,
                IsActive = true
            };

            if (dto.IsDefault)
            {
                List<Address> current = await _ctx.Addresses.Where(a => a.UserId == userId && a.IsActive).ToListAsync();
                foreach (Address a in current)
                {
                    a.IsDefault = false;
                }
                _ctx.Addresses.UpdateRange(current);
            }

            await _repo.AddAsync(address);
            await _repo.SaveAsync();
            return address;
        }

        public async Task<Address> UpdateAsync(int userId, int id, AddressUpdateDto dto)
        {
            Address? existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId)
            {
                throw new System.UnauthorizedAccessException("Nije dozvoljeno.");
            }

            existing.Label = dto.Label;
            existing.Street = dto.Street;
            existing.City = dto.City;
            existing.PostalCode = dto.PostalCode;
            existing.Entrance = dto.Entrance;
            existing.Floor = dto.Floor;
            existing.Apartment = dto.Apartment;
            existing.Latitude = dto.Latitude;
            existing.Longitude = dto.Longitude;
            existing.Notes = dto.Notes;

            bool wantsDefault = dto.IsDefault;
            if (wantsDefault && !existing.IsDefault)
            {
                List<Address> current = await _ctx.Addresses.Where(a => a.UserId == userId && a.IsActive).ToListAsync();
                foreach (Address a in current)
                {
                    a.IsDefault = false;
                }
                existing.IsDefault = true;
                _ctx.Addresses.UpdateRange(current);
            }
            else if (!wantsDefault && existing.IsDefault)
            {
                existing.IsDefault = false;
            }

            await _repo.UpdateAsync(existing);
            await _repo.SaveAsync();
            return existing;
        }

        public async Task SetDefaultAsync(int userId, int id)
        {
            Address? existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId)
            {
                throw new System.UnauthorizedAccessException("Nije dozvoljeno.");
            }

            List<Address> current = await _ctx.Addresses.Where(a => a.UserId == userId && a.IsActive).ToListAsync();
            foreach (Address a in current)
            {
                a.IsDefault = false;
            }
            existing.IsDefault = true;

            _ctx.Addresses.UpdateRange(current);
            await _repo.UpdateAsync(existing);
            await _repo.SaveAsync();
        }

        public async Task DeleteAsync(int userId, int id)
        {
            Address? existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId)
            {
                throw new System.UnauthorizedAccessException("Nije dozvoljeno.");
            }

            await _repo.DeleteAsync(existing); // soft delete
            await _repo.SaveAsync();
        }
    }
}
