using Microsoft.EntityFrameworkCore;
using Payment.DB;
using Payment.Repositories;
using Payment.Services.Bank;
using Payment.Services.BankRequester.Bank;
using Payment.Services.CreationExceptions.Fabric;
using Payment.Services.ExceptionHandlers;
using Payment.Services.HttpHandler;
using Payment.Services.HttpHandlers;
using Payment.Services.Payment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddExceptionFabric();

builder.Services.AddExceptionHandler<CreationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IBankRequester, BankRequester>();

builder.Services.AddTransient<AuthMessageHandler>();
builder.Services.AddTransient<IdempotenceKeyHandler>();

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgres"]));

builder.Services.AddHttpClient<IBankRequester, BankRequester>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Services:Bank"]!);
    c.Timeout = TimeSpan.FromSeconds(40);
}).AddHttpMessageHandler<AuthMessageHandler>()
  .AddHttpMessageHandler<IdempotenceKeyHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
