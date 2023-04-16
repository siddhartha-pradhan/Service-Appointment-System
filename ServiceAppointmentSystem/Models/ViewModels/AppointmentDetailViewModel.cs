using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ServiceAppointmentSystem.Models.ViewModels;

public class AppointmentDetailViewModel
{
	public int AppointmentId { get; set; }

	public string Request { get; set; }

	public string? UserId { get; set; }

	public string? UserName { get; set; }

	public byte[]? UserProfile { get; set; }

	[Display(Name = "Service")]
	public int ServiceId { get; set; }

	public string Service { get; set; }

	public Guid? ProfessionalId { get; set; }

	public List<SelectListItem>? Professionals { get; set; }

	[Display(Name = "Booked Date")]
	public string BookedDate { get; set; }

	[Display(Name = "Appointment Date")]
	public string AppointedDate { get; set; }

	public string? FinalizedDate { get; set; }
}
