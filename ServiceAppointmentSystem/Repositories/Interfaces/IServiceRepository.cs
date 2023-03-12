using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        void Update(Service service);
        
        void Delete(Service service);
    }
}
