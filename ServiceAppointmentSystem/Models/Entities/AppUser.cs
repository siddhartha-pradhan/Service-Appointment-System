using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }    

        [Required]
        public byte[] ProfileImage { get; set; }

        [Display(Name = "City")]
        public string CityAddress { get; set; }

        [Display(Name = "Region")]
        public string RegionName { get; set; }

        public string? Professionalism { get; set; }

        [Display(Name = "Certification")]
        public string? CertificationURL { get; set; }

        [Display(Name = "Resume")]
        public string? ResumeURL { get; set; }
    }
}
