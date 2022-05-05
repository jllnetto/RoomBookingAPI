using Business.Models.Base;

namespace Business.Models
{
    public class Room : Entity
    {
        public string RoomNumber { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int AdultCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public List<Booking> Booking { get; set; }


    }
}
