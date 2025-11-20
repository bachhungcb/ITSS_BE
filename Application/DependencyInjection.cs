using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Interfaces;
namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddAutoMapper(cfg =>
        {
            // Cấu hình bổ sung (nếu cần) ở đây
            // Bỏ trống nếu không cần
        }, Assembly.GetExecutingAssembly());
    }
    
}