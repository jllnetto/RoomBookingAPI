using AutoMapper;
using Business.Utils.Domain.Utils;
using RoomBooking.Business.Models;
using RoomBooking.Business.Models.DTOs.Input;
using RoomBooking.Business.Models.DTOs.Output;

namespace RoomBooking.API.Configuration
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            //OutPut Mapping
            CreateMap<Room, RoomOutputDTO>().ReverseMap();
            CreateMap<Booking, BookingOutputDTO>().ReverseMap();
            CreateMap<Paginator<Booking>, Paginator<BookingOutputDTO>>().ReverseMap();
            CreateMap<Paginator<Room>, Paginator<RoomOutputDTO>>().ReverseMap();

            //Input Mapping
            CreateMap<Room, RoomInputDTO>().ReverseMap();
            CreateMap<BookingInputDTO, Booking>()
                .ForMember(
                    dest => dest.BookingStarts,
                    act => act.MapFrom(src => src.BookingStarts.Date)
                )
                .ForMember(
                    dest => dest.BookingEnds,
                    act => act.MapFrom(src => src.BookingEnds.Date)
                )
                .ReverseMap();
            CreateMap<Booking, BookingInputDTO>();
        }
    }

}
