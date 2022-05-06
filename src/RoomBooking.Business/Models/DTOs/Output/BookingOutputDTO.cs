using System.ComponentModel.DataAnnotations;

namespace RoomBooking.Business.Models.DTOs.Output
{
    public class BookingOutputDTO
    {
        public Guid Id { get; set; }

        public DateTime BookingStarts { get; set; }

        public DateTime BookingEnds { get; set; }

        public decimal Total { get; set; }

        public Guid RoomId { get; set; }
        public string? BookingStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
