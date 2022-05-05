using Business.Models.Base;
using Business.Models.Enuns;

namespace Business.Models
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
