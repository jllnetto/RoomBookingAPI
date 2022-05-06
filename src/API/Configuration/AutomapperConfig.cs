using AutoMapper;
using Business.Models;
using Business.Models.DTOs.Input;
using Business.Models.DTOs.Output;
using Business.Utils.Domain.Utils;

namespace API.Configuration
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Room, RoomOutputDTO>().ReverseMap();
            CreateMap<Booking, BookingOutputDTO>().ReverseMap();
            CreateMap<Room, RoomInputDTO>().ReverseMap();
            CreateMap<Booking, BookingInputDTO>().ReverseMap();
            CreateMap<Paginator<Booking>, Paginator<BookingOutputDTO>>().ReverseMap();
            CreateMap<Paginator<Room>, Paginator<RoomOutputDTO>>().ReverseMap();
        }
    }

}
