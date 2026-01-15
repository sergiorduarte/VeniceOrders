using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VeniceOrders.Domain.Entities;

namespace VeniceOrders.Domain.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId);
        Task CreateManyAsync(List<OrderItem> items);
    }
}
