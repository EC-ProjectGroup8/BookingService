using API.DbContext;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Konfiguration (Key Vault & Connection String) ---
// Laddar in secrets från Key Vault till IConfiguration
var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
}

// Hämta anslutningssträngen för databasen från Key Vault
var connectionString = builder.Configuration.GetValue<string>("DbConnection");
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Dependency Injection (DI) ---
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Registrera HttpClient för att BookingService ska kunna anropa andra API:er
builder.Services.AddHttpClient<IBookingService, BookingService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:WorkoutApi"]);
});

// --- Säkerhet (Authentication & Authorization) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// --- Övriga Services ---
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

// --- Swagger med JWT Authorize-knapp ---
// --- Swagger med JWT-support ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });

    // Definiera JWT Bearer-schemat
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer", // Case-sensitive, ska vara "Bearer"
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Skriv 'Bearer {token}'"
    });

    // Applicera schemat på alla endpoints som är markerade med [Authorize]
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// =======================================================================
var app = builder.Build();

// --- Middleware Pipeline ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API V1");
});


app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
