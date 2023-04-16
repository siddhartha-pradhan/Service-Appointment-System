namespace ServiceAppointmentSystem.Models.ViewModels
{
	public class ApprovalViewModel
	{
		public string UserId { get; set; }

		public Guid ProfessionalId { get; set; }

		public string FullName { get; set; }

		public string PhoneNumber { get; set; }

		public string EmailAddress { get; set; }

		public byte[] ProfileImage { get; set; }

		public string Specialist { get; set; }

		public string Certification { get; set; }

		public string Resume { get; set; }
	}
}
