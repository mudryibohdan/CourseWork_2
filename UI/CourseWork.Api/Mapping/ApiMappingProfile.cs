using AutoMapper;
using CourseWork.Api.Models;
using CourseWork.Business.DTOs;

namespace CourseWork.Api.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<TourDto, TourResponse>();
        CreateMap<CreateTourRequest, CreateTourDto>();
        CreateMap<UpdateTourRequest, UpdateTourDto>();
        CreateMap<TourSearchRequest, TourSearchFilterDto>();
        CreateMap<RegisterRequest, RegisterUserDto>();
        CreateMap<LoginRequest, LoginDto>();
        CreateMap<AuthResultDto, AuthResponse>();
        CreateMap<TourBookingDto, TourBookingResponse>();
        CreateMap<TicketBookingDto, TicketBookingResponse>();
        CreateMap<CreateTourBookingRequest, CreateTourBookingDto>();
        CreateMap<CreateTransportBookingRequest, CreateTransportTicketBookingDto>();
        CreateMap<CreateHotelRoomBookingRequest, CreateHotelRoomBookingDto>();
        CreateMap<CountryDto, CountryResponse>();
        CreateMap<RegionDto, RegionResponse>();
        CreateMap<TransportDto, TransportResponse>();
        CreateMap<HotelRoomDto, HotelRoomResponse>();
    }
}
