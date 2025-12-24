using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Options;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // Tạo một Swagger document cho mỗi phiên bản API được phát hiện
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                
                CreateInfoForApiVersion(description)
            );
        }

        // Kích hoạt Annotations (nếu cần)
        options.EnableAnnotations();
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = "Your API",
            Version = description.ApiVersion.ToString(),
            Description = "Your API Description",
            Contact = new OpenApiContact
            {
                Name = "Your Name",
                Email = "your.email@example.com"
            }
        };

        if (description.IsDeprecated)
        {
            info.Description += " - This API version has been deprecated.";
        }

        return info;
    }
}