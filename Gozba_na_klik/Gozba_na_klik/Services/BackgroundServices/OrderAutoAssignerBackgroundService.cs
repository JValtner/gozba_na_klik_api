using Gozba_na_klik.Services.OrderAutoAssignerServices;

public class OrderAutoAssignerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderAutoAssignerBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var assigner = scope.ServiceProvider.GetRequiredService<IOrderAutoAssignerService>();
                await assigner.AssignOrderToCourierAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
