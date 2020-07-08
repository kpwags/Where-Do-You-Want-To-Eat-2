using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WhereDoYouWantToEat2.Respositories.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        IQueryable<T> Get(Expression<Func<T, bool>> expression);
        T GetById(object id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task DeleteRange(List<T> entity);
    }
}