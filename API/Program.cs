using API.DbContext;
using API.Repositories;
using API.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- Konfiguration (Key Vault & Connection String) ---
// Detta är ett modernare sätt att integrera Key Vault.
// Den läser automatiskt in dina secrets till IConfiguration.
var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Dependency Injection (DI) ---
// Här talar vi om att när någon frågar efter ett INTERFACE, 
// ska de få en specifik KLASS.
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Registrera HttpClient för att BookingService ska kunna anropa andra API:er
builder.Services.AddHttpClient<IBookingService, BookingService>(client =>
{
    // Hämta bas-URL:en till Workout-API:et från appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:WorkoutApi"]);
});


// --- Säkerhet (Authentication & Authorization) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();


// --- Övriga Services ---
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    // Din CORS-policy...
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// =======================================================================
var app = builder.Build();

// --- Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy"); // Se till att din policy har ett namn

// VIKTIGT: Dessa måste vara med, och i rätt ordning, för att [Authorize] ska fungera!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();