using Order.API.OpenTelemetry;

namespace Order.API.OrderServices
{
    public class OrderService
    {
        public Task CreateAsync(OrderCreateRequestDto requestDto)
        {
            using var activity = ActivitySourceProvider.Source.StartActivity();
            activity?.AddEvent(new("Sipariş süreci başladı!"));

            //veritabanına kaydettik
            activity?.SetTag("order user id", requestDto.UserId);

            activity?.AddEvent(new("Sipariş süreci tamamlandı!"));
            return Task.CompletedTask;
        }
    }
}
