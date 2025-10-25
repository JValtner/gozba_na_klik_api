namespace Gozba_na_klik.Services.OrderAutoAssignerServices
{
    public class OrderAutoAssignerService : IOrderAutoAssignerService
    {
        private readonly ILogger<OrderAutoAssignerService> _logger;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public OrderAutoAssignerService(
            ILogger<OrderAutoAssignerService> logger,
            IUserService userService,
            IOrderService orderService)
        {
            _logger = logger;
            _userService = userService;
            _orderService = orderService;
        }

        public async Task AssignOrderToCourierAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Dobavljam sve porudzbine sa statusom 'PRIHVAĆENA'.");
                var ordersOnWait = await _orderService.GetAllAcceptedOrdersAsync();

                _logger.LogInformation("Dobavljam sve dostavljace bez dostave.");
                var availableCouriers = await _userService.GetAllAvailableCouriersAsync();

                if (!availableCouriers.Any())
                {
                    _logger.LogInformation("Nema slobodnih kurira trenutno.");
                }

                else if (!ordersOnWait.Any())
                {
                    _logger.LogInformation("Nema porudzbina na cekanju.");
                }
                else
                {
                    int count = Math.Min(ordersOnWait.Count, availableCouriers.Count);
                    for (int i = 0; i < count; i++)
                    {
                        var order = ordersOnWait[i];
                        var courier = availableCouriers[i];

                        await _orderService.AssignCourierToOrderAsync(order.Id, courier.Id);
                        await _userService.AssignOrderToCourierAsync(courier.Id, order.Id);

                        _logger.LogInformation($"Porudzbina {order.Id} dodeljena kuriru {courier.Id}.");
                    }
                }

                // Čekaj 30 sekundi pre sledeće iteracije
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
