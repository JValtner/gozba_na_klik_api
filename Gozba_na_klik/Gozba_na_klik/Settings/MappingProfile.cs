using AutoMapper;
using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Employee;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Settings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------- Restaurant ----------
            CreateMap<RequestCreateRestaurantByAdminDto, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<RequestUpdateRestaurantByAdminDto, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // ---------- Meal ----------
            CreateMap<RequestMealDto, Meal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForMember(dest => dest.Addons, opt => opt.Ignore())
                .ForMember(dest => dest.Alergens, opt => opt.Ignore());

            CreateMap<Meal, ResponseMealDto>()
                .ForMember(dest => dest.Restaurant, opt => opt.MapFrom(src => src.Restaurant))
                .ForMember(dest => dest.Addons, opt => opt.MapFrom(src => src.Addons))
                .ForMember(dest => dest.Alergens, opt => opt.MapFrom(src => src.Alergens));

            // ---------- Addon ----------
            CreateMap<RequestAddonDto, MealAddon>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Meal, opt => opt.Ignore());

            CreateMap<MealAddon, ResponseAddonDTO>();

            // ---------- Alergen ----------
            CreateMap<RequestAlergenDto, Alergen>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Meals, opt => opt.Ignore());

            CreateMap<Alergen, ResponseAlergenDto>();

            // ---------- Employee (User) ----------
            // User → EmployeeListItemDto (za response)
            CreateMap<User, EmployeeListItemDto>();

            // RegisterEmployeeDto → User (za kreiranje)
            CreateMap<RegisterEmployeeDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore()) // ← Service postavlja
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())     // ← Service postavlja
                .ForMember(dest => dest.UserImage, opt => opt.Ignore());

            // UpdateEmployeeDto → User (za update)
            CreateMap<UpdateEmployeeDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.UserImage, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Password))); // ← Samo ako postoji!
        }
    }
}