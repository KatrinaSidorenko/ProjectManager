using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<Result<List<T>>> GetAll();
        Task<Result<T>> GetById(Guid id);
        Task<Result<bool>> Update(Guid id,  T entity);
        Task<Result<bool>> Add(T entity);
        Task<Result<bool>> Delete(Guid id);
        Task<Result<T>> GetByPredicate(Func<T, bool> predicate);
    }
}
