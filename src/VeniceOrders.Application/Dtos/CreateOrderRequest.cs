using System;
using System.Collections.Generic;

namespace VeniceOrders.Application.Dtos
{
    public class CreateOrderRequest
    {
        public Guid ClienteId { get; set; }
        public List<OrderItemDto> Itens { get; set; } = [];
    }
}
