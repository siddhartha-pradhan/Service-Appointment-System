using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ServiceId { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
    }
}
