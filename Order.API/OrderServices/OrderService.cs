using Common.Shared.DTOs;
using OpenTelemetry.Shared;
using Order.API.Models;
using Order.API.StockServices;
using System.Net;

namespace Order.API.OrderServices
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly StockService _stockService;

        public OrderService(AppDbContext context, StockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        public async Task<ResponseDto<OrderCreateResponseDto>> CreateAsync(OrderCreateRequestDto requestDto)
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


            StockCheckAndPaymentProcessRequestDto request = new()
            {
                OrderCode = newOrder.OrderCode,
                OrderItems = requestDto.Items
            };


            activity?.SetTag("order user id", requestDto.UserId);

            activity?.AddEvent(new("Sipariş süreci tamamlandı!"));

            var (isSuccess, failMessage) = await _stockService.CheckStockAndPaymentStartAsync(request);
            if (!isSuccess)
                return ResponseDto<OrderCreateResponseDto>.Fail(HttpStatusCode.InternalServerError.GetHashCode(), failMessage!);


            return ResponseDto<OrderCreateResponseDto>.Success(HttpStatusCode.OK.GetHashCode(), new()
            {
                Id = newOrder.Id
            });
        }
    }
}