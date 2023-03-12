using ServiceAppointmentSystem.Data;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Repositories
{
    public class ProfessionalRepository : Repository<Professional>, IProfessionalRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfessionalRepository(ApplicationDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public void Approve(Professional professional)
        {
            var user = _dbContext.AppUsers.FirstOrDefault(x => x.Id == professional.UserId);
            
            professional.IsApproved = true;

            user.EmailConfirmed = true;
        }
    }
}
