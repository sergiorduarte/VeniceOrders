using System.Threading.Tasks;

namespace VeniceOrders.Domain.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string queue);
    }
}
