using Gozba_na_klik.Models;

namespace Gozba_na_klik.Services.OrderAutoAssignerServices
{
    public class OrderAutoAssignerService : IOrderAutoAssignerService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderAutoAssignerService> _logger;

        public OrderAutoAssignerService(
            IUsersRepository usersRepository,
            IOrderRepository orderRepository, 
            ILogger<OrderAutoAssignerService> logger)
        {
            _usersRepository = usersRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task AssignOrderToCourierAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Dobavljam sve porudzbine sa statusom 'PRIHVAĆENA'.");
                var ordersOnWait = await _orderRepository.GetAllAcceptedOrdersAsync();

                _logger.LogInformation("Dobavljam sve dostavljace bez dostave.");
                var availableCouriers = await _usersRepository.GetAllAvailableCouriersAsync();

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

                        await _orderRepository.AssignCourierToOrderAsync(order, courier);
                        await _usersRepository.AssignOrderToCourierAsync(order, courier);

                        _logger.LogInformation($"Porudzbina {order.Id} dodeljena kuriru {courier.Id}.");
                    }
                }

                // Čekaj 30 sekundi pre sledeće iteracije
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
