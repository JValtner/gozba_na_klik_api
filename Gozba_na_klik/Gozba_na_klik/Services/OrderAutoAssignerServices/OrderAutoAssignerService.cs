using Gozba_na_klik.Models;

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

                _logger.LogInformation($"Pronađeno {ordersOnWait.Count} porudžbina na čekanju i {availableCouriers.Count} slobodnih kurira.");

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
                    // Dodatna provera: filtriraj kurire koji stvarno nemaju aktivnu porudžbinu
                    var trulyAvailableCouriers = new List<User>();
                    foreach (var courier in availableCouriers)
                    {
                        var activeOrder = await _orderService.GetCourierOrderInPickupAsync(courier.Id);
                        if (activeOrder == null)
                        {
                            trulyAvailableCouriers.Add(courier);
                        }
                        else
                        {
                            _logger.LogWarning($"Kurir {courier.Id} ima aktivnu porudžbinu {activeOrder.OrderId}, ali ActiveOrderId je null. Ispravljam...");
                            // Ispravi ActiveOrderId ako postoji aktivna porudžbina
                            await _userService.AssignOrderToCourierAsync(courier.Id, activeOrder.OrderId);
                        }
                    }

                    int count = Math.Min(ordersOnWait.Count, trulyAvailableCouriers.Count);
                    _logger.LogInformation($"Dodeljujem {count} porudžbina kuririma (od {trulyAvailableCouriers.Count} stvarno slobodnih).");
                    
                    for (int i = 0; i < count; i++)
                    {
                        var order = ordersOnWait[i];
                        var courier = trulyAvailableCouriers[i];

                        try
                        {
                            await _orderService.AssignCourierToOrderAsync(order.Id, courier.Id);
                            await _userService.AssignOrderToCourierAsync(courier.Id, order.Id);

                            _logger.LogInformation($"Porudzbina {order.Id} dodeljena kuriru {courier.Id}.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Greška pri dodeli porudžbine {order.Id} kuriru {courier.Id}.");
                        }
                    }
                }

                // Čekaj 30 sekundi pre sledeće iteracije
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
