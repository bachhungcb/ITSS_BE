using Api.Services;
using Application;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using DataAccess.EFCore;
using Microsoft.OpenApi;
using Tools;
using Api.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region API Versioning

// 1. Cấu hình API Versioning với tên mới
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        // Cấu hình cách đọc version từ URL
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
// 2. PHẦN QUAN TRỌNG NHẤT: Dùng AddApiExplorer thay vì AddVersionedApiExplorer
    .AddMvc()
    .AddApiExplorer(options =>
    {
        // Định dạng tên version trong Swagger UI: v1, v2, ...
        options.GroupNameFormat = "'v'VVV";

        // Tự động thay thế tham số version trong route
        options.SubstituteApiVersionInUrl = true;
    });

#endregion

#region Swagger

builder.Services.AddSwaggerGen(options => { options.EnableAnnotations(); });

// 2. CẤU HÌNH CHI TIẾT (Giải pháp sửa lỗi)
// Sử dụng AddOptions để inject IApiVersionDescriptionProvider một cách an toàn
builder.Services.AddOptions<SwaggerGenOptions>()
    .Configure<IApiVersionDescriptionProvider>((options, provider) =>
    {
        // Debug: Nếu provider.ApiVersionDescriptions rỗng, Swagger sẽ lỗi.
        // Bước 1 ở trên sẽ đảm bảo danh sách này có dữ liệu.
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"ITSS Backend API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated ? "API cũ" : "API hệ thống"
            });
        }
    });

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

#endregion

#region Services

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUriService, UriService>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddTools();

#endregion


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Lấy lại thông tin các phiên bản API
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        // Tạo một endpoint trong Swagger UI cho mỗi phiên bản
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

// app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();