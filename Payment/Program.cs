using Microsoft.EntityFrameworkCore;
using Payment.DB;
using Payment.Services.CreationExceptions.Fabric;
using Payment.Services.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddExceptionFabric();

builder.Services.AddExceptionHandler<CreationExceptionHandler>();

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgres"]));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
