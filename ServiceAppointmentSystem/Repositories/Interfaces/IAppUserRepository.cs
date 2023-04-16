using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        AppUser GetById(string? id);
    }
}
