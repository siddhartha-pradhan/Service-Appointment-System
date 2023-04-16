using System.ComponentModel.DataAnnotations;

namespace ServiceAppointmentSystem.Models.ViewModels
{
    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }

        public string Request { get; set; }

        public string? UserId { get; set; }  

        public string? UserName { get; set; }    

        public byte[]? UserProfile { get; set; } 

        public string ActionStatus { get; set; }

        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        public string Service { get; set; }

        public Guid? ProfessionalId { get; set; }

        public string? ProfessionalName { get; set; }

        public string? Professionalism { get; set; }

        [Display(Name = "Booked Date")]
        public string BookedDate { get; set; }

		[Display(Name = "Appointment Date")]
        public string AppointedDate { get; set; }

        public string? FinalizedDate { get; set; }

        public string? ProfessionalRemarks { get; set; }

    }
}
