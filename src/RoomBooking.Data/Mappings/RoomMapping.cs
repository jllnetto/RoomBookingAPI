using RoomBooking.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RoomBooking.Data.Mappings
{
    public class RoomMapping : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.RoomNumber).IsRequired().HasColumnType("varchar(10)");
            builder.Property(r => r.Description).IsRequired().HasColumnType("varchar(1000)");
            builder.HasMany(r => r.Booking).WithOne(re => re.Room).HasForeignKey(re => re.RoomId);
            builder.ToTable("Rooms");

        }
    }
}
