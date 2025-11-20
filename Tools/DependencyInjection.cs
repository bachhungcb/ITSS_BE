using Application.Interfaces;
using DataAccess.EFCore.Persistence;
using DataAccess.EFCore.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tools.Services;
using Tools.Utils;

namespace Tools;

public static class DependencyInjection
{
    public static void AddTools(this IServiceCollection services)
    {
        // THÊM CÁC DỊCH VỤ BỊ THIẾU
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IToken, TokenService>();

        // Các dịch vụ khác bạn đã có (ví dụ)
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
    }
}