using OpenTelemetry.Shared;
using Order.API.Models;

namespace Order.API.OrderServices
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderCreateResponseDto> CreateAsync(OrderCreateRequestDto requestDto)
        {
            using var activity = ActivitySourceProvider.Source.StartActivity();
            activity?.AddEvent(new("Sipariş süreci başladı!"));

            var newOrder = new Order()
            {
                Created = DateTime.Now,
                OrderCode = Guid.NewGuid().ToString(),
                Status = OrderStatus.Success,
                UserId = Guid.Parse(requestDto.UserId),
                Items = requestDto.Items.Select(x => new OrderItem()
                {
                    Count = x.Count,
                    ProductId = x.ProductId,
                    UnitPrice = x.UnitPrice
                }).ToList()
            };

            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            activity?.SetTag("order user id", requestDto.UserId);

            activity?.AddEvent(new("Sipariş süreci tamamlandı!"));
            return new OrderCreateResponseDto()
            {
                Id = newOrder.Id
            };
        }
    }
}
