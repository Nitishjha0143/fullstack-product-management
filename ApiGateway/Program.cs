using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Port for API Gateway
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000); // API Gateway port
});

// Load Ocelot routes
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173") // frontend URL
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ✅ JWT Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://localhost:5001"; // Auth service URL (without /api)
        options.RequireHttpsMetadata = false; // allow HTTP in dev
        options.Audience = "your-audience"; // match the token's "aud" claim
    });

builder.Services.AddAuthorization();

// Ocelot
builder.Services.AddOcelot();

var app = builder.Build();

// Order matters
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
