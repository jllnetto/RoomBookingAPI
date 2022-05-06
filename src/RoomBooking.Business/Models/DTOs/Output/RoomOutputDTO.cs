namespace RoomBooking.Business.Models.DTOs.Output
{
    public class RoomOutputDTO
    {
        public Guid Id { get; set; }

        public string RoomNumber { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int AdultCapacity { get; set; }

        public int ChildrenCapacity { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
