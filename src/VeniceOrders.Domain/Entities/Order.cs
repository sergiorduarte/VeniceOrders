using System;
using VeniceOrders.Domain.Enum;

namespace VeniceOrders.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public DateTime Data { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
