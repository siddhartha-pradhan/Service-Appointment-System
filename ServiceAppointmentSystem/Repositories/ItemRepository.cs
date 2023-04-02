using ServiceAppointmentSystem.Data;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Repositories
{
	public class ItemRepository : Repository<Item>, IItemRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public ItemRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public void Delete(Item item)
		{
			_dbContext.Items.Remove(item);
		}

		public void Update(Item item)
		{
			_dbContext.Items.Update(item);
		}
	}
}
