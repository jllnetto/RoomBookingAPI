using Microsoft.Extensions.Options;
using RoomBooking.Business.Interfaces.Notifications;
using RoomBooking.Business.Interfaces.Repositories;
using RoomBooking.Business.Interfaces.Services;
using RoomBooking.Business.Notifications;
using RoomBooking.Business.Services;
using RoomBooking.Data.Context;
using RoomBooking.Data.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RoomBooking.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IBookingService, BookingService>();

            services.AddScoped<INotificator, Notificator>();

            services.AddScoped<BookingDbContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}
