using AutoMapper;
using Gozba_na_klik.Models;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;

namespace Gozba_na_klik.Settings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------- Meal ----------
            CreateMap<RequestMealDto, Meal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID handled by DB
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore()) // linked via RestaurantId
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
                .ForMember(dest => dest.Meal, opt => opt.Ignore());

            CreateMap<Alergen, ResponseAlergenDto>();
        }
    }
}
