using AutoMapper;
using Business.Models;
using Business.Models.DTOs;
using Business.Utils.Domain.Utils;

namespace API.Configuration
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<Paginator<Booking>, Paginator<BookingDTO>>().ReverseMap();
            CreateMap<Paginator<Room>, Paginator<RoomDTO>>().ReverseMap();
        }
    }

}
