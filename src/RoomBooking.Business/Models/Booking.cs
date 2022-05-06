using RoomBooking.Business.Models.Base;
using RoomBooking.Business.Models.Enums;

namespace RoomBooking.Business.Models
{
    public class Booking : Entity
    {
        public BookingStatus BookingStatus { get; set; }

        public DateTime BookingStarts { get; set; }

        public DateTime BookingEnds { get; set; }

        public decimal Total { get; set; }

        public Guid RoomId { get; set; }

        public Room Room { get; set; }
    }
}
