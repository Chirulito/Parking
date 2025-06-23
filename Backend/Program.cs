using Microsoft.IdentityModel.Tokens;    
using Microsoft.EntityFrameworkCore;
using Google.Cloud.SecretManager.V1;
using Backend.Context;                                              
using System.Text;
using AspNetCoreRateLimit;
using Microsoft.OpenApi.Models;

try {
    // Attempt to get the GCP Project ID from environment variables
    string? projectId = Environment.GetEnvironmentVariable("GCP_PROJECT_ID");

    // Check if the environment variable is missing or empty
    if (string.IsNullOrWhiteSpace(projectId)) {

        Console.WriteLine("Could not retrieve Project ID. Environment variable 'GCP_PROJECT_ID' is missing.");

            throw new InvalidOperationException("GCP_PROJECT_ID environment variable is required.");
    }

}
catch (Exception ex)
{
    Console.WriteLine($"Error while retrieving secrets: {ex.Message}");
    throw;
}


// Starts ASP.NET web application
var builder = WebApplication.CreateBuilder(args);

// Acceder a Google Secret Manager
var secretClient = SecretManagerServiceClient.Create();
string GetSecret(string secretName)
{
    var secretVersion = secretClient.AccessSecretVersion(
        SecretVersionName.FromProjectSecretSecretVersion("centered-oasis-459820-h0", secretName, "latest"));
    return secretVersion.Payload.Data.ToStringUtf8();

}

builder.Configuration["ConnectionStrings:SQL"] = GetSecret("sql-connection");
builder.Configuration["Jwt:Key"] = GetSecret("jwt-key");
builder.Configuration["Jwt:Issuer"] = GetSecret("jwt-issuer");
builder.Configuration["Jwt:Audience"] = GetSecret("jwt-audience");
builder.Configuration["Jwt:ExpireMinutes"] = GetSecret("jwt-expiration");

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
try {
    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseSqlServer(                             
            builder.Configuration.GetConnectionString("SQL")
        )
    );
    Console.WriteLine("\nDatabase connection established!\n");
}
catch (Exception) {
    Console.WriteLine("\nDatabase connection failed...\n");
}


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var key = builder.Configuration["Jwt:Key"];

        if (key != null) {

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT como: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


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
