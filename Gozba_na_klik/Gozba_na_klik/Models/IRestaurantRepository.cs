using BookstoreApplication.Utils;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Utils;

namespace Gozba_na_klik.Models;

public interface IRestaurantRepository
{
    Task<IEnumerable<Restaurant>> GetAllAsync();
    Task<Restaurant?> GetByIdAsync(int id);
    Task<Restaurant> AddAsync(Restaurant restaurant);
    Task<Restaurant> UpdateAsync(Restaurant restaurant);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Restaurant>> GetByOwnerAsync(int ownerId);
    Task<PaginatedList<Restaurant>> GetAllFilteredSortedPagedAsync(RestaurantFilter filter, int sortType, int page, int pageSize);
    Task<List<SortTypeOption>> GetSortTypesAsync();

}
