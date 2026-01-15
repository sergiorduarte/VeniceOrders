using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeniceOrders.Application.Dtos;
using VeniceOrders.Application.Events;
using VeniceOrders.Application.Services;
using VeniceOrders.Domain.Entities;
using VeniceOrders.Domain.Enum;
using VeniceOrders.Domain.Interfaces;

namespace VeniceOrders.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMessagePublisher> _messagePublisherMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _messagePublisherMock = new Mock<IMessagePublisher>();

            _orderService = new Application.Services.OrderService(
                _orderRepositoryMock.Object,
                _orderItemRepositoryMock.Object,
                _cacheServiceMock.Object,
                _messagePublisherMock.Object
            );
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrder_AndPublishEvent()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var request = new CreateOrderRequest
            {
                ClienteId = clienteId,
                Itens = new List<OrderItemDto>
            {
                new() { Produto = "Notebook", Quantidade = 2, PrecoUnitario = 3000 },
                new() { Produto = "Mouse", Quantidade = 3, PrecoUnitario = 50 }
            }
            };

            _orderRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order o) => o);

            _orderItemRepositoryMock
                .Setup(x => x.CreateManyAsync(It.IsAny<List<OrderItem>>()))
                .Returns(Task.CompletedTask);

            _messagePublisherMock
                .Setup(x => x.PublishAsync(It.IsAny<OrderCreatedEvent>(), "order.created"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.CreateOrderAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.ClienteId.Should().Be(clienteId);
            result.Itens.Should().HaveCount(2);
            result.Status.Should().Be(OrderStatus.Pendente);
            result.Total.Should().Be(6150); // (2*3000) + (3*50)

            _orderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Order>()), Times.Once);
            _orderItemRepositoryMock.Verify(x => x.CreateManyAsync(It.IsAny<List<OrderItem>>()), Times.Once);
            _messagePublisherMock.Verify(x => x.PublishAsync(It.IsAny<OrderCreatedEvent>(), "order.created"), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnFromCache_WhenCacheExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var cachedOrder = new OrderResponse
            {
                Id = orderId,
                ClienteId = Guid.NewGuid(),
                Data = DateTime.UtcNow,
                Status = OrderStatus.Aprovado,
                Itens = new List<OrderItemDto>
            {
                new() { Produto = "Teclado", Quantidade = 1, PrecoUnitario = 200 }
            }
            };

            _cacheServiceMock
                .Setup(x => x.GetAsync<OrderResponse>($"order:{orderId}"))
                .ReturnsAsync(cachedOrder);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(cachedOrder);

            _orderRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _orderItemRepositoryMock.Verify(x => x.GetByOrderIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _cacheServiceMock
                .Setup(x => x.GetAsync<OrderResponse>($"order:{orderId}"))
                .ReturnsAsync((OrderResponse?)null);

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(orderId))
                .ReturnsAsync((Order?)null);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldFetchFromDatabase_AndCacheResult_WhenCacheIsEmpty()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                ClienteId = clienteId,
                Data = DateTime.UtcNow,
                Status = OrderStatus.Processando,
                CreatedAt = DateTime.UtcNow
            };

            var items = new List<OrderItem>
        {
            new() { Id = Guid.NewGuid(), OrderId = orderId, Produto = "Monitor", Quantidade = 1, PrecoUnitario = 1500 }
        };

            _cacheServiceMock
                .Setup(x => x.GetAsync<OrderResponse>($"order:{orderId}"))
                .ReturnsAsync((OrderResponse?)null);

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _orderItemRepositoryMock
                .Setup(x => x.GetByOrderIdAsync(orderId))
                .ReturnsAsync(items);

            _cacheServiceMock
                .Setup(x => x.SetAsync($"order:{orderId}", It.IsAny<OrderResponse>(), TimeSpan.FromMinutes(2)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(orderId);
            result.ClienteId.Should().Be(clienteId);
            result.Itens.Should().HaveCount(1);
            result.Itens.First().Produto.Should().Be("Monitor");
            result.Total.Should().Be(1500);

            _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
            _orderItemRepositoryMock.Verify(x => x.GetByOrderIdAsync(orderId), Times.Once);
            _cacheServiceMock.Verify(x => x.SetAsync($"order:{orderId}", It.IsAny<OrderResponse>(), TimeSpan.FromMinutes(2)), Times.Once);
        }
    }
}
