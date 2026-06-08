using CourseWork.Business.Services;
using CourseWork.Data.Context;
using CourseWork.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CourseWork.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLayer(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ITicketBookingService, TicketBookingService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReferenceDataService, ReferenceDataService>();

        return services;
    }
}
