using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
	public interface IOrderHeaderRepository
	{
		void Update(OrderHeader orderHeader);

		void UpdateStatus(int ID, string orderStatus, string? paymentStatus = null);

		void UpdatePaymentStatus(int ID, string sessionID, string paymentIntentID);
	}
}
