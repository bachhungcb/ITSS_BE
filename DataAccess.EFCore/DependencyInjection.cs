using Application.Interfaces;
using DataAccess.EFCore.Context;
using DataAccess.EFCore.Persistence;
using DataAccess.EFCore.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.EFCore;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        // 2. Đăng ký IApplicationDbContext (Bạn đã làm đúng)
        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

        // 3. THIẾU: Đăng ký IUnitOfWork và Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
    
   
}