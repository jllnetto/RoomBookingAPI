using System.ComponentModel.DataAnnotations;

namespace Business.Models.DTOs.Output
{
    public class RoomOutputDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        [StringLength(10, ErrorMessage = "The field {0} must have between {2} and {1} characters", MinimumLength = 1)]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        [StringLength(1000, ErrorMessage = "The field {0} must have between {2} and {1} characters", MinimumLength = 10)]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        public int AdultCapacity { get; set; }

        [Required(ErrorMessage = "The field {0} is required ")]
        public int ChildrenCapacity { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }

        [ScaffoldColumn(false)]
        public DateTime UpdateDate { get; set; }


    }
}
