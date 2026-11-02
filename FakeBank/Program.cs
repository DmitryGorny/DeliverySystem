using FakeBank.Infrastructure;
using FakeBank.Options;
using FakeBank.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o => BankJson.Apply(o.JsonSerializerOptions))
    .ConfigureApiBehaviorOptions(o =>
    {
        // Ошибки валидации тоже с полем "code"
        o.InvalidModelStateResponseFactory = context =>
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Title = "Request validation failed",
                Status = StatusCodes.Status400BadRequest
            };
            problem.Extensions["code"] = "validation_error";
            return new BadRequestObjectResult(problem);
        };
    });

// Results.Ok/Created/Problem используют эти настройки JSON
builder.Services.ConfigureHttpJsonOptions(o => BankJson.Apply(o.SerializerOptions));
builder.Services.AddProblemDetails();

builder.Services.Configure<BankOptions>(builder.Configuration.GetSection(BankOptions.SectionName));
builder.Services.Configure<MerchantOptions>(builder.Configuration.GetSection(MerchantOptions.SectionName));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<BankStore>();
builder.Services.AddSingleton<BankService>();
builder.Services.AddSingleton<IdempotencyStore>();
builder.Services.AddSingleton<WebhookSigner>();

builder.Services.AddHttpClient(BankWorker.WebhookClientName, c => c.Timeout = TimeSpan.FromSeconds(5));
builder.Services.AddHostedService<BankWorker>();

var app = builder.Build();

// Авторизация и Idempotence-Key для /v1/*
app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/v1"), branch => branch.Use(async (ctx, next) =>
{
    var apiKey = ctx.RequestServices.GetRequiredService<IOptions<MerchantOptions>>().Value.ApiKey;

    if (ctx.Request.Headers.Authorization.ToString() != $"Bearer {apiKey}")
    {
        await BankProblems.Create(StatusCodes.Status401Unauthorized, "unauthorized", "Invalid or missing API key")
            .ExecuteAsync(ctx);
        return;
    }

    var key = ctx.Request.Headers["Idempotence-Key"].ToString();
    if (HttpMethods.IsPost(ctx.Request.Method) && (string.IsNullOrWhiteSpace(key) || key.Length > 64))
    {
        await BankProblems.Create(StatusCodes.Status400BadRequest, "idempotence_key_required",
                "Header Idempotence-Key is required (1-64 characters)")
            .ExecuteAsync(ctx);
        return;
    }

    await next();
}));

app.MapControllers();

app.Run();
