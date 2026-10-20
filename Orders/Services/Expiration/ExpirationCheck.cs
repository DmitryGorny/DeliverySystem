using Orders.Repositories;

namespace Orders.Services.Expiration
{
    public class ExpirationCheck : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ExpirationCheck> _logger;
        private readonly IConfiguration _configuration;

        public ExpirationCheck(
            IServiceScopeFactory serviceScopeFactory, 
            ILogger<ExpirationCheck> logger,
            IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        { 
            using var timer = new PeriodicTimer(
                TimeSpan.FromMinutes(double.Parse(_configuration["BackgroundService:IntervalMin"]!))
                );

            while (await timer.WaitForNextTickAsync(stoppingToken))
            { 
                try
                {
                    await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();

                    IOrdersRepository ordersRepository =
                        scope.ServiceProvider.GetRequiredService<IOrdersRepository>();

                    var changedRows = await ordersRepository.SetPaymentExpired(stoppingToken);

                    if (changedRows >= 1) _logger.LogInformation("Изменено строк: {rows}", changedRows);
                } catch (Exception e)
                {
                    _logger.LogError(e, "Ошибка при истечении заказов");
                }
            }
        }
    }
}
