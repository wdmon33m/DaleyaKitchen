using Daleya.API;
using Daleya.API.Data;
using Daleya.API.Extentions;
using Daleya.API.Repository;
using Daleya.API.Repository.IRepository;
using Daleya.API.Service;
using Daleya.API.Service.IService;
using Daleya.API.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefualtConnection"));
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
builder.Services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default30",
        new CacheProfile()
        {
            Duration = 30
        }
        );
    //option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Daleya Kitchen Version 1",
        Description = "API to Authentication",
        TermsOfService = new Uri("https://example.com/term"),
        Contact = new OpenApiContact
        {
            Name = "Mohamed Ibrahem",
            Email = "wdmon33m@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Example license",
            Url = new Uri("https://example.com/license")
        }
    });
});
var app = builder.Build();

SD.Cloudinary_CloudName = builder.Configuration.GetSection("Cloudinary:CloudName").Get<string>();
SD.Cloudinary_Secretkey = builder.Configuration.GetSection("Cloudinary:Secretkey").Get<string>();
SD.Cloudinary_Apikey = builder.Configuration.GetSection("Cloudinary:Key").Get<string>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Coupon API");
    });
}
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Coupon API");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.ApplyMigration();
app.Run();
