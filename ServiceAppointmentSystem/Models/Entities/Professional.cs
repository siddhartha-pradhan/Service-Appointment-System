using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class Professional 
    {
        [Key]
        public int Id { get; set; }
        
        public string UserId { get; set; }  

        public string Role { get; set; }

        public double Rate { get; set; }

        public string Certification { get; set; }

        public int ServiceId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }
    }
}
