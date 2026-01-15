using System;
using System.Threading.Tasks;
using VeniceOrders.Domain.Entities;

namespace VeniceOrders.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order> CreateAsync(Order order);
        Task UpdateAsync(Order order);
    }
}
