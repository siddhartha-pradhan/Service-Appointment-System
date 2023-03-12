using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);

        void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null);

        void UpdatePaymentStatus(int Id, string sessionId, string paymentIntentId);
    }
}
