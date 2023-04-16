using System.Linq.Expressions;

namespace ServiceAppointmentSystem.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null);

        T Get(int Id);

        T GetById(Guid? Id);


        List<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null);

        void Add(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}
