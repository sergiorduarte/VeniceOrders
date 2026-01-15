using System;
using System.Linq;
using System.Threading.Tasks;
using VeniceOrders.Application.Dtos;
using VeniceOrders.Application.Events;
using VeniceOrders.Domain.Entities;
using VeniceOrders.Domain.Enum;
using VeniceOrders.Domain.Interfaces;

namespace VeniceOrders.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICacheService _cacheService;
        private readonly IMessagePublisher _messagePublisher;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICacheService cacheService,
            IMessagePublisher messagePublisher)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cacheService = cacheService;
            _messagePublisher = messagePublisher;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ClienteId = request.ClienteId,
                Data = DateTime.UtcNow,
                Status = OrderStatus.Pendente,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepository.CreateAsync(order);

            var items = request.Itens.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Produto = i.Produto,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList();

            await _orderItemRepository.CreateManyAsync(items);

            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                ClienteId = order.ClienteId,
                Data = order.Data,
                Total = items.Sum(i => i.Total),
                CreatedAt = DateTime.UtcNow
            };

            await _messagePublisher.PublishAsync(orderEvent, "order.created");

            return new OrderResponse
            {
                Id = order.Id,
                ClienteId = order.ClienteId,
                Data = order.Data,
                Status = order.Status,
                Itens = request.Itens
            };
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(Guid id)
        {
            var cacheKey = $"order:{id}";
            var cached = await _cacheService.GetAsync<OrderResponse>(cacheKey);
            if (cached != null) return cached;

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            var items = await _orderItemRepository.GetByOrderIdAsync(id);

            var response = new OrderResponse
            {
                Id = order.Id,
                ClienteId = order.ClienteId,
                Data = order.Data,
                Status = order.Status,
                Itens = items.Select(i => new OrderItemDto
                {
                    Produto = i.Produto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            };

            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(2));

            return response;
        }
    }
}
