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
// Detta �r ett modernare s�tt att integrera Key Vault.
// Den l�ser automatiskt in dina secrets till IConfiguration.
var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Dependency Injection (DI) ---
// H�r talar vi om att n�r n�gon fr�gar efter ett INTERFACE, 
// ska de f� en specifik KLASS.
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Registrera HttpClient f�r att BookingService ska kunna anropa andra API:er
builder.Services.AddHttpClient<IBookingService, BookingService>(client =>
{
    // H�mta bas-URL:en till Workout-API:et fr�n appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:WorkoutApi"]);
});


// --- S�kerhet (Authentication & Authorization) ---
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


// --- �vriga Services ---
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

// VIKTIGT: Dessa m�ste vara med, och i r�tt ordning, f�r att [Authorize] ska fungera!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();