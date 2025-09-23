using API.DbContext;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ----- Configure Azure Key Vault -----
// Get the Key Vault URI from the application's configuration (e.g., appsettings.json).
var keyVaultUri = builder.Configuration.GetValue<Uri>("KeyVaultUrl");

// If a Key Vault URI is found, add it as a configuration source.
if (keyVaultUri != null)
{
    // DefaultAzureCredential authenticates using the environment's security principal.
    // This could be a developer's credentials (Visual Studio, Azure CLI) in a local environment,
    // or a managed identity when deployed to Azure.
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

// ----- Database Configuration -----
// Get the database connection string. It will first look in sources like Azure Key Vault.
// If not found, it will fall back to the local development database string.
var connectionString = builder.Configuration.GetValue<string>("DbConnection")
                         ?? "Server=(localdb)\\mssllocaldb;Database=BookingDb;Trusted_Connection=True;";

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

// ----- Dependency Injection (DI) -----
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

// ----- API Services -----
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

// ----- Swagger/OpenAPI Configuration -----
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });
});

var app = builder.Build();

// ----- HTTP Request Middleware Pipeline -----
// Configure Swagger and Swagger UI.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API V1");
    // Serve Swagger UI at the app's root.
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();

