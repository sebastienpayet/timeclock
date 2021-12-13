using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeClock.business.port.repository
{
    interface IRepository<T>
    {
        T save(T entity);
        List<T> findAll();
        T findById(string id);
    }
}
