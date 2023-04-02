using ServiceAppointmentSystem.Data;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Repositories
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public int DecrementCount(ShoppingCart cart, int count)
		{
			cart.Count -= count;
			return cart.Count;
		}

		public int IncrementCount(ShoppingCart cart, int count)
		{
			cart.Count += count;
			return cart.Count;
		}
	}
}
