using Microsoft.AspNetCore.SignalR;

namespace Gozba_na_klik.Hubs
{
    public class CourierLocationHub : Hub
    {
        public async Task SendLocation(string orderId, double latitude, double longitude)
        {
            var groupName = $"order-{orderId}";
            await Clients.Group(orderId).SendAsync("ReceiveLocation", latitude, longitude);
        }

        // Kada se porudžbina završi
        public async Task CompleteOrder(string orderId)
        {
            var groupName = $"order-{orderId}";
            await Clients.Group(orderId).SendAsync("OrderCompleted");
        }

        // Dodaj metode za join/leave group
        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
        }

        public async Task LeaveOrderGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
        }
    }
}
