using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ServiceId { get; set; }

        public Guid? ProfessionalId { get; set; } = null;

        public string Request { get; set; }

        public DateTime BookedDate { get; set; } = DateTime.Now;

        public DateTime AppointmentDate { get; set; }

        public DateTime? FinalizedDate { get; set;}

        public string? AdminRemarks { get; set; }

        public string? ProfessionalRemarks { get; set; } = "";

        public string ActionStatus { get; set; } = Constants.Constants.Booked;

        public string PaymentStatus { get; set; } = Constants.Constants.Pending;

        [ForeignKey("UserId")]
        public virtual AppUser? AppUser { get; set; }

        [ForeignKey("ProfessionalId")]
        public virtual Professional? Professional { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }
    }
}
