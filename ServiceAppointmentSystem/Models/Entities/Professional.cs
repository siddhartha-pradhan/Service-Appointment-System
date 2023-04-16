using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class Professional 
    {
        [Key]
        public Guid Id { get; set; }
        
        public string UserId { get; set; }  

        public double Rate { get; set; }

        public string Certification { get; set; }

        public string Resume { get; set; }

        public int ServiceId { get; set; }

        public bool IsApproved { get; set; } = false; 

        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }
    }
}
