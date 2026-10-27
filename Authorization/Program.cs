using Authorization.DB;
using Authorization.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddDbContext<AppDbContext>(options => { 
        options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgres"]);
        options.UseOpenIddict();
    });

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AppDbContext>();
    });

builder.Services.AddOpenIddict()

    .AddServer(options =>
    {
        options.SetTokenEndpointUris("api/Auth/ServicesAuth");

        options.AllowClientCredentialsFlow();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .DisableTransportSecurityRequirement();
    });


var app = builder.Build();

using var scope = app.Services.CreateAsyncScope();

var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

var ct = app.Lifetime.ApplicationStopping;

await authService.SeedServicesCardentials(ct);

// Configure the HTTP request pipeline.


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
