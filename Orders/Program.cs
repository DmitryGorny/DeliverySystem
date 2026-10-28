using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Client;
using Orders.DB;
using Orders.Models.enums;
using Orders.Repositories;
using Orders.Services.Auth.AuthHttpHandler;
using Orders.Services.Auth.AuthRequester;
using Orders.Services.Auth.Token;
using Orders.Services.Catalog;
using Orders.Services.Expiration;
using Orders.Services.Order;
using Orders.Services.PaymentStrategies;
using Orders.Services.PaymentStrategy;
using Orders.Services.PaymentSystem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();

builder.Services.AddScoped<IPaymentRequester, PaymentRequester>();

builder.Services.AddKeyedScoped<IPaymentStrategy, CreateAndPayStrategy>(OrderType.CreateAndPay);
builder.Services.AddKeyedScoped<IPaymentStrategy, DelayedPaymentStrategy>(OrderType.DelayedPayment);

builder.Services.AddScoped<IAuthRequester, AuthRequester>();
builder.Services.AddSingleton<ITokenProvider, TokenProvider>();

builder.Services.AddTransient<AuthHttpHandler>();

builder.Services.AddHttpClient<ICatalogRequester, CatalogRequester>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Services:Catalog"]!);
    c.Timeout = TimeSpan.FromSeconds(40);
}).AddHttpMessageHandler<AuthHttpHandler>();

builder.Services.AddHttpClient<IAuthRequester, AuthRequester>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Services:Auth"]!);
});

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgres"]));

builder.Services.AddHostedService<ExpirationCheck>();

builder.Services.AddOpenIddict()
    .AddClient(options =>
    {
        options.AllowClientCredentialsFlow();

        options.DisableTokenStorage();

        options.UseSystemNetHttp();

        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri("http://localhost:5047", UriKind.Absolute),

            ClientId = builder.Configuration["AuthCardentials:ClientId"],
            ClientSecret = builder.Configuration["AuthCardentials:ClientSeecret"]
        });
    });

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(builder.Configuration["Services:Auth"]!);

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        // Register the System.Net.Http integration.
        options.UseSystemNetHttp();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

;

app.UseAuthorization();

app.MapControllers();

app.Run();
