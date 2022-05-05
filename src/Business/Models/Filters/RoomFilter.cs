namespace Business.Models.Filters
{
    public class RoomFilter
    {
        public Guid? Id { get; set; }
        public string RoomNumber { get; set; }
        public decimal? PriceBegin { get; set; }
        public decimal? PriceFinish { get; set; }
        public int? AdultCapacityBegin { get; set; }
        public int? AdultCapacityFinish { get; set; }
        public int? ChildrenCapacityBegin { get; set; }
        public int? ChildrenCapacityFinish { get; set; }

    }
}
