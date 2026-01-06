using Api.Services;
using Application;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using DataAccess.EFCore;
using Tools;
using Api.Options;

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

builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://bachhungcb.github.io",
                                "http://scic.navistar.io:3636",
                                "https://scic.navistar.io:3636", // Chỉ định chính xác domain FE
                                "https://bachhungcb.github.io",
                                "http://localhost:5173") // Chỉ định chính xác domain FE
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
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
    // SỬA ĐOẠN NÀY: Thêm options để ép phiên bản
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
//         c.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
//     });
// }

// app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();