using ServiceAppointmentSystem.Data;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Repositories
{
    public class OrderRepository : Repository<OrderHeader>, IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(OrderHeader order)
        {
            _dbContext.Orders.Update(order);
        }

        public void UpdatePaymentStatus(int Id, string sessionId, string paymentIntentId)
        {
            var order = _dbContext.Orders.FirstOrDefault(u => u.Id == Id);

            if (order != null)
            {
                order.PaymentDate = DateTime.Now;
                order.SessionId = sessionId;
                order.PaymentIntentId = paymentIntentId;
            }

        }

        public void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null)
        {
            var order = _dbContext.Orders.FirstOrDefault(u => u.Id == Id);

            if (order != null)
            {
                order.OrderStatus = orderStatus;

                if (paymentStatus != null)
                {
                    order.PaymentStatus = paymentStatus;
                }
            }
        }
    }
}
