using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Models.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }

        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
    }
}
