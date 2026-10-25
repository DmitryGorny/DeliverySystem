using Microsoft.EntityFrameworkCore;
using Orders.DB;
using Orders.Models.enums;
using Orders.Repositories;
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

builder.Services.AddHttpClient<ICatalogRequester, CatalogRequester>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Services:Catalog"]!);
    c.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgres"]));

builder.Services.AddHostedService<ExpirationCheck>();

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
