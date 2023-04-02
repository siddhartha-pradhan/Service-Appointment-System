using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Models.ViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }
        
        public IEnumerable<SelectListItem>? ServiceList { get; set; }
    }
}
