using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Options;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    // Dependency Injection hoạt động chính xác tại đây
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // Lúc này _provider đã có đầy đủ dữ liệu version
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "ITSS_BE API", // Tên dự án của bạn
            Version = description.ApiVersion.ToString(),
            Description = "Hệ thống API backend.",
            Contact = new OpenApiContact { Name = "ITSS_rauma", Email = "ITSSrauma@example.com" }
        };

        if (description.IsDeprecated)
        {
            info.Description += " (Phiên bản này đã ngưng hỗ trợ)";
        }

        return info;
    }
}