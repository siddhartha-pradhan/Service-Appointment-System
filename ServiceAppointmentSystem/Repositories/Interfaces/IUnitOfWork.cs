namespace ServiceAppointmentSystem.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAppUserRepository AppUser { get; set; }

        IAppointmentRepository Appointment { get; set; }

        IItemRepository Item { get; set; }

        IOrderDetailRepository OrderDetail { get; set; }

        IOrderRepository Order { get; set; }

        IProfessionalRepository Professional { get; set; }

        IServiceRepository Service { get; set; }

		IShoppingCartRepository ShoppingCart { get; set; }

		void Save();
    }
}
