namespace ServiceAppointmentSystem.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAppUserRepository AppUser { get; set; }

        IOrderDetailRepository OrderDetail { get; set; }

        IOrderRepository Order { get; set; }

        IProfessionalRepository Professional { get; set; }

        IServiceRepository Service { get; set; }

        void Save();
    }
}
