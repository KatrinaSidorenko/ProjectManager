using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IGenericService<T> where T : BaseEntity
    {
        Task Add(T entity);
        Task Update(Guid id, T entity);
        Task Delete(Guid id);
        Task<T> GetById(Guid id);
        Task<List<T>> GetAll();
        Task<T> GetByPredicate(Func<T, bool> predicate);
    }
}
