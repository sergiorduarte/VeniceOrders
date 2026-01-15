using System;
using System.Threading.Tasks;
using VeniceOrders.Domain.Entities;
using VeniceOrders.Domain.Interfaces;

namespace VeniceOrders.Infrastructure.Perssitense.SqlServer
{
    public class OrderRepository : IOrderRepository
    {
        private readonly VeniceDbContext _context;

        public OrderRepository(VeniceDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
