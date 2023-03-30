using ServiceAppointmentSystem.Data;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Repositories
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ServiceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(Service service)
        {
            _dbContext.Services.Remove(service);
        }

        public string Role(int Id)
        {
            return _dbContext.Services.FirstOrDefault(x => x.Id == Id).Role;
        }

        public void Update(Service service)
        {
            _dbContext.Services.Update(service);    
        }
    }
}
