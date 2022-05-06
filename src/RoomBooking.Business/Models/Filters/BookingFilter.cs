namespace RoomBooking.Business.Models.Filters
{
    public class BookingFilter
    {
        public Guid? Id { get; set; }
        public string? RoomNumber { get; set; }
        public int? BookingStatus { get; set; }
        public DateTime? StartDateBegin { get; set; }
        public DateTime? StartDateFinish { get; set; }
        public DateTime? EndDateBegin { get; set; }
        public DateTime? EndDateFinish { get; set; }
        public Guid? RoomId { get; set; }

    }
}
