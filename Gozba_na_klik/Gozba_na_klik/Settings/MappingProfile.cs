using AutoMapper;
using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.DeliveryPersonSchedule;
using Gozba_na_klik.DTOs.Employee;
using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Gozba_na_klik.Utils;
using System.Text.Json;
using System.Linq;

namespace Gozba_na_klik.Settings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------- User ----------
            CreateMap<RegistrationDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())          
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserImage, opt => opt.Ignore())  
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); 

            // --- User -> ProfileDto ---
            CreateMap<User, ProfileDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.UserImage));
            CreateMap<RequestUpdateUserByAdminDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<RequestUpdateAlergenByUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .AfterMap((dto, user) =>
                {
                    user.UserAlergens.Clear();

                    foreach (var alergenId in dto.AlergensIds)
                    {
                        user.UserAlergens.Add(new UserAlergen
                        {
                            UserId = user.Id,
                            AlergenId = alergenId
                        });
                    }
                });

            // ---------- Restaurant ----------
            CreateMap<RequestCreateRestaurantByAdminDto, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<RequestUpdateRestaurantByAdminDto, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // ---------- Address ----------
            CreateMap<AddressCreateDto, Address>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<AddressUpdateDto, Address>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            CreateMap<Address, AddressListItemDto>();

            // ---------- Restaurant → DTO ----------
            CreateMap<Restaurant, ResponseRestaurantDTO>()
                .ForMember(dest => dest.isOpen, opt => opt.Ignore()) // custom logic
                .ReverseMap();

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
            CreateMap<Alergen, ResponseAlergenBasicDto>();

            // Map UserAlergen → ResponseAlergenBasicDto
            CreateMap<UserAlergen, ResponseAlergenBasicDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Alergen.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Alergen.Name));

            // User → ResponseUserAlergenDto
            CreateMap<User, ResponseUserAlergenDto>()
                .ForMember(dest => dest.Alergens, opt => opt.MapFrom(src => src.UserAlergens.Select(ua => ua.Alergen)));

            // ---------- Employee ----------
            CreateMap<User, EmployeeListItemDto>();

            CreateMap<RegisterEmployeeDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.UserImage, opt => opt.Ignore());

            CreateMap<UpdateEmployeeDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.UserImage, opt => opt.Ignore());
            // ✅ Removed mapping for Password

            // ---------- DeliveryPersonSchedule ----------
            CreateMap<DeliveryPersonSchedule, DeliveryScheduleDto>()
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => (int)src.DayOfWeek))
                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => DateTimeHelper.GetDayName(src.DayOfWeek)))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString(@"hh\:mm")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString(@"hh\:mm")))
                .ForMember(dest => dest.Hours, opt => opt.MapFrom(src => Math.Round((src.EndTime - src.StartTime).TotalHours, 2)));

            // ---------- Orders ----------
            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Name : ""))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.Address != null ? $"{src.Address.Street}, {src.Address.City}, {src.Address.PostalCode}" : ""))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.MealName, opt => opt.MapFrom(src => src.Meal != null ? src.Meal.Name : "Unknown"))
                .ForMember(dest => dest.MealImagePath, opt => opt.MapFrom(src => src.Meal != null ? src.Meal.ImagePath : null))
                .ForMember(dest => dest.SelectedAddons, opt => opt.MapFrom(src => JsonHelper.DeserializeStringList(src.SelectedAddons)));

            CreateMap<Order, CourierActiveOrderDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Buyer, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Restaurant, opt => opt.MapFrom(src => src.Restaurant))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<User, CourierOrderUserDto>();
            CreateMap<Restaurant, CourierOrderRestaurantDto>();
            CreateMap<Address, CourierOrderAddressDto>();
            CreateMap<OrderItem, CourierOrderItemDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Meal.Name));

            CreateMap<Order, OrderStatusDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // Order response
            CreateMap<Order, OrderStatusResponseDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.AcceptedAt ?? src.CancelledAt ?? src.OrderDate))
                .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DeliveryPerson, opt => opt.MapFrom(src => src.DeliveryPerson))
                .ForMember(dest => dest.Restaurant, opt => opt.MapFrom(src => src.Restaurant))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items));

                    // Podmape (unutar OrderStatusResponseDto)
                    CreateMap<User, DeliveryPersonDto>();
                    CreateMap<Restaurant, RestaurantDto>();
                    CreateMap<Address, CustomerAddressDto>();
                    CreateMap<OrderItem, OrderItemDto>()
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Meal.Name));
                

            // ---------- Order History ----------
            CreateMap<Order, OrderHistoryResponseDto>()
                .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Name : "Nepoznat restoran"))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.Address != null ? $"{src.Address.Street}, {src.Address.City}" : "Adresa nije dostupna"))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        }
    }
}
