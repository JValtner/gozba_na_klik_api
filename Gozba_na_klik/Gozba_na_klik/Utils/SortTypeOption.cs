using Gozba_na_klik.Enums;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Utils
{
    public class SortTypeOption
    {
        public int Key { get; set; }
        public string Name { get; set; }

        public SortTypeOption(MealSortType sortType) 
        {
            Key = (int)sortType;
            Name = sortType.ToString(); 
        }
        public SortTypeOption(AddonSortType sortType)
        {
            Key = (int)sortType;
            Name = sortType.ToString();
        }
        public SortTypeOption(AlergenSortType sortType)
        {
            Key = (int)sortType;
            Name = sortType.ToString();
        }
        public SortTypeOption(RestaurantSortType sortType)
        {
            Key = (int)sortType;
            Name = sortType.ToString();
        }
    }

}
