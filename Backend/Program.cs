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
    string? connectionString = builder.Configuration.GetConnectionString("Database");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Missing connection string for 'Database'. Check appsettings.json.");
    }

    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseSqlServer(connectionString)
    );

    Console.WriteLine("\nDatabase connection established!\n");
}
catch (Exception ex)
{
    Console.WriteLine($"\nDatabase connection failed: {ex.Message}\n");
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
