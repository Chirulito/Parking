using Microsoft.EntityFrameworkCore;
using Backend.Context;
using AspNetCoreRateLimit;


// Starts ASP.NET web application
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Adds services to the ASP.NET web container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Adds DatabaseContext to the ASP.NET web container by sending options to the constructor.
try
{
    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("Database-Esteban")
        )
    );
    builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Database-Jus")
        )
    );
    Console.WriteLine("\nDatabase connection established!\n");
}
catch (Exception)
{
    Console.WriteLine("\nDatabase connection failed...\n");
}


// Adds authorization policies to protect endpoint routes: [Authorize]
builder.Services.AddAuthorization();

// Builds the application using the configured services
var app = builder.Build();

app.UseIpRateLimiting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
