using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader order);

        void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null);

        void UpdatePaymentStatus(int Id, string sessionId, string paymentIntentId);
    }
}
