using System.ComponentModel.DataAnnotations;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Base Price")]
        public double BasePrice { get; set; }
    }
}
