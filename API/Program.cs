var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddCors(o =>
{
    o.AddPolicy("CorsPolicy", p =>
    {
        if (builder.Environment.IsDevelopment())
            p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        else
            p.WithOrigins("frontend link here")
             .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();


//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();


app.Run();