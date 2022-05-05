using Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings
{
    public class BookingsMapping : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.BookingStarts).IsRequired();
            builder.Property(r => r.BookingStarts).IsRequired();
            builder.Property(r => r.BookingEnds).IsRequired();
            builder.HasOne(r => r.Room).WithMany(ro => ro.Booking).HasForeignKey(r => r.RoomId);
            builder.ToTable("Bookings");
        }
    }
}
