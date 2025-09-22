using API.DbContext;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Get database connection string (with a dummy fallback) ---
var connectionString = builder.Configuration.GetValue<string>("DbConnection")
                         ?? "Server=(localdb)\\mssllocaldb;Database=BookingDb;Trusted_Connection=True;";

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Dependency Injection (DI) ---
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();


// --- Other Services ---
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddEndpointsApiExplorer();

// --- Swagger ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });
});

var app = builder.Build();

// --- Middleware Pipeline ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();