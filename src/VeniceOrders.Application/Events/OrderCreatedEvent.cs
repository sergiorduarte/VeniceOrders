using System;

namespace VeniceOrders.Application.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public Guid ClienteId { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
