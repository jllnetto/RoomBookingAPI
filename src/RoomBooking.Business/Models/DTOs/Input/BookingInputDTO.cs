using System.ComponentModel.DataAnnotations;

namespace RoomBooking.Business.Models.DTOs.Input
{
    public class BookingInputDTO
    {
        [Required(ErrorMessage = "The field {0} is required ")]
        public DateTime BookingStarts { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        public DateTime BookingEnds { get; set; }


        [Required(ErrorMessage = "The field {0} is required ")]
        public Guid RoomId { get; set; }
    }
}
