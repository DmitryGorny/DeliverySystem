using Authorization.DB;
using Authorization.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        options.AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .DisableTransportSecurityRequirement();

        options.AddEncryptionKey(new SymmetricSecurityKey(
             Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

    });


var app = builder.Build();

await using(var scope = app.Services.CreateAsyncScope()) {

    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

    var ct = app.Lifetime.ApplicationStopping;

    await authService.SeedServicesCardentials(ct);
}

// Configure the HTTP request pipeline.


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
