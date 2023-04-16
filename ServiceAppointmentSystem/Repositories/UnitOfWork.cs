using ServiceAppointmentSystem.Data;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            AppUser = new AppUserRepository(_dbContext);
            Item = new ItemRepository(_dbContext);
			Order = new OrderRepository(_dbContext);
            OrderDetail = new OrderDetailRepository(_dbContext);
            Professional = new ProfessionalRepository(_dbContext);
            Service = new ServiceRepository(_dbContext);
			ShoppingCart = new ShoppingCartRepository(_dbContext);
            Appointment = new AppointmentRepository(_dbContext);
        }

        public IAppUserRepository AppUser { get; set; }

        public IAppointmentRepository Appointment { get; set; }

        public IItemRepository Item { get; set; }

		public IOrderDetailRepository OrderDetail { get; set; }
        
        public IOrderRepository Order { get; set; }
        
        public IProfessionalRepository Professional { get; set; }
        
        public IServiceRepository Service { get; set; }

		public IShoppingCartRepository ShoppingCart { get; set; }

		public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
