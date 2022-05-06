using Business.Interfaces.Notifications;
using Business.Interfaces.Repositories;
using Business.Interfaces.Services;
using Business.Notifications;
using Business.Services;
using Data.Context;
using Data.Repositories;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<BookingDbContext>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            services.AddScoped<INotificator, Notificator>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IBookingService, BookingService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}
