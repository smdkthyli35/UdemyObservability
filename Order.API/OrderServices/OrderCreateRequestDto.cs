using Common.Shared.DTOs;

namespace Order.API.OrderServices
{
    public record OrderCreateRequestDto
    {
        public string UserId { get; set; } = null!;
        public List<OrderItemDto> Items { get; set; } = null!;
    }
}
