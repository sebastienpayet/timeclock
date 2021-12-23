using System.Collections.Generic;

namespace TimeClock.business.port.repository
{
    public interface IRepository<T>
    {
        T Save(T entity);
        List<T> FindAll();
        T FindById(string id);
    }
}
