namespace Gozba_na_klik.Services.OrderAutoAssignerServices
{
    public interface IOrderAutoAssignerService
    {
        Task AssignOrderToCourierAsync(CancellationToken stoppingToken);
    }
}
