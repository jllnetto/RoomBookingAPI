using System.ComponentModel.DataAnnotations;

namespace Business.Models.DTOs
{
    public class BookingDTO
    {
        public Guid Id { get; set; }



        [Required(ErrorMessage = "The field {0} is required ")]
        public DateTime BookingStarts { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        public DateTime BookingEnds { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        public Guid RoomId { get; set; }

        public RoomDTO Room { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }

        [ScaffoldColumn(false)]
        public DateTime UpdateDate { get; set; }
    }
}
