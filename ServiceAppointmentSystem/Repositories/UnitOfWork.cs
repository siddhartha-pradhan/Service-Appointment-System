using ServiceAppointmentSystem.Data;
using ServiceAppointmentSystem.Models.Entities;
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
            Order = new OrderRepository(_dbContext);
            OrderDetail = new OrderDetailRepository(_dbContext);
            Professional = new ProfessionalRepository(_dbContext);
            Service = new ServiceRepository(_dbContext);
        }

        public IAppUserRepository AppUser { get; set; }
        
        public IOrderDetailRepository OrderDetail { get; set; }
        
        public IOrderRepository Order { get; set; }
        
        public IProfessionalRepository Professional { get; set; }
        
        public IServiceRepository Service { get; set; }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
