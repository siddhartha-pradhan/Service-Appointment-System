using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public Order Order { get; set; }

        public IEnumerable<ShoppingCart>? CartList { get; set; }
    }
}
