using AutoMapper;
using Business.Models;
using Business.Models.DTOs;

namespace API.Configuration
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<Booking, BookingDTO>().ReverseMap();
        }
    }

}
