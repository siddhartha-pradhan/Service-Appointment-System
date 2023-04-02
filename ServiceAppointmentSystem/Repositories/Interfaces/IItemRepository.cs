using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
	public interface IItemRepository : IRepository<Item>
	{
		void Update(Item item);

		void Delete(Item item);
	}
}
