using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.DTOs.Input
{
    public class RoomInputDTO
    {        

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


    }
}
