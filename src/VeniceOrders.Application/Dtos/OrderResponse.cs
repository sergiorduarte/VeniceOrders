using System;
using System.Collections.Generic;
using System.Linq;
using VeniceOrders.Domain.Enum;

namespace VeniceOrders.Application.Dtos
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public DateTime Data { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemDto> Itens { get; set; } = new();
        public decimal Total => Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
    }
}
