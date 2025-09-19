using API.DbContext;
using API.Repositories;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Hämta connection string från Key Vault
var keyVaultUrl = "https://<ditt-keyvault-namn>.vault.azure.net/";
var secretName = "<din-secret-namn>";

var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
KeyVaultSecret secret = secretClient.GetSecret(secretName);
string connectionString = secret.Value;

// Registrera DbContext med connection string från Key Vault
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add controllers
builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins("https://din-frontend-url.com") // Byt ut mot din frontend-länk
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency injection
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<BookingService>();

var app = builder.Build();

// Enable Swagger in all environments (eller bara dev om du vill)
app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();