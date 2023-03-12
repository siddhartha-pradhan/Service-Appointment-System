using ServiceAppointmentSystem.Models.Entities;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
    public interface IProfessionalRepository : IRepository<Professional>
    {
        void Approve (Professional professional);
    }
}
