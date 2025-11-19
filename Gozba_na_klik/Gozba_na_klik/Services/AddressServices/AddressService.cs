using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Exceptions;
using Microsoft.EntityFrameworkCore;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Services.AddressServices
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repo;
        private readonly GozbaNaKlikDbContext _ctx;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository repo, GozbaNaKlikDbContext ctx, IMapper mapper)
        {
            _repo = repo;
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<List<AddressListItemDto>> GetMyAsync(int userId)
        {
            var addresses = await _repo.GetMyAsync(userId);
            return _mapper.Map<List<AddressListItemDto>>(addresses);
        }

        public async Task<AddressListItemDto> CreateAsync(int userId, AddressCreateDto dto)
        {
            Address address = _mapper.Map<Address>(dto);
            address.UserId = userId;
            address.IsActive = true;

            if (dto.IsDefault)
            {
                List<Address> current = await _ctx.Addresses
                    .Where(a => a.UserId == userId && a.IsActive)
                    .ToListAsync();

                foreach (Address a in current)
                {
                    a.IsDefault = false;
                }

                _ctx.Addresses.UpdateRange(current);
            }

            await _repo.AddAsync(address);
            await _repo.SaveAsync();
            return _mapper.Map<AddressListItemDto>(address);
        }

        public async Task UpdateAsync(int userId, int id, AddressUpdateDto dto)
        {
            Address? existing = await _repo.GetByIdAsync(id);
            if (existing == null)
            {
                throw new NotFoundException("Adresa nije pronađena.");
            }
            if (existing.UserId != userId)
            {
                throw new ForbiddenException("Nije dozvoljeno.");
            }

            _mapper.Map(dto, existing);

            bool wantsDefault = dto.IsDefault;
            if (wantsDefault && !existing.IsDefault)
            {
                List<Address> current = await _ctx.Addresses
                    .Where(a => a.UserId == userId && a.IsActive)
                    .ToListAsync();

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
        }

        public async Task SetDefaultAsync(int userId, int id)
        {
            Address? existing = await _repo.GetByIdAsync(id);
            if (existing == null)
            {
                throw new NotFoundException("Adresa nije pronađena.");
            }
            if (existing.UserId != userId)
            {
                throw new ForbiddenException("Nije dozvoljeno.");
            }

            List<Address> current = await _ctx.Addresses
                .Where(a => a.UserId == userId && a.IsActive)
                .ToListAsync();

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
            if (existing == null)
            {
                throw new NotFoundException("Adresa nije pronađena.");
            }
            if (existing.UserId != userId)
            {
                throw new ForbiddenException("Nije dozvoljeno.");
            }

            await _repo.DeleteAsync(existing); 
            await _repo.SaveAsync();
        }
    }
}
