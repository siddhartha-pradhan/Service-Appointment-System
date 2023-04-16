using ServiceAppointmentSystem.Data;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Repositories
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AppUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public AppUser GetById(string id)
        {
            var user = _dbContext.AppUsers.FirstOrDefault(x => x.Id == id);
            
            return user;
        }
    }
}
