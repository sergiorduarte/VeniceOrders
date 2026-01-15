using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VeniceOrders.Domain.Entities;
using VeniceOrders.Domain.Interfaces;

namespace VeniceOrders.Infrastructure.Perssitense.Mongo
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly MongoDbContext _context;

        public OrderItemRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderItems
                .Find(i => i.OrderId == orderId)
                .ToListAsync();
        }

        public async Task CreateManyAsync(List<OrderItem> items)
        {
            await _context.OrderItems.InsertManyAsync(items);
        }
    }
}
