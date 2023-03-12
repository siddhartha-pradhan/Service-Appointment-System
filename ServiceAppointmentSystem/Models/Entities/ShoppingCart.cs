using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public string UserId { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }

        [NotMapped]
        public double TotalAmount { get; set; }
    }
}
