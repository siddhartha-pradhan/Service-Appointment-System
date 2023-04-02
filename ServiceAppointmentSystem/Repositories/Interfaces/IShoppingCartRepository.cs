using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
	public interface IShoppingCartRepository : IRepository<ShoppingCart>
	{
		int IncrementCount(ShoppingCart cart, int count);

		int DecrementCount(ShoppingCart cart, int count);
	}
}
