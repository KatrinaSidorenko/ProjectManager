using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<Result<T>> GetById(Guid id);
        Task<Result<IEnumerable<T>>> GetAll();
        Task<Result<T>> Find(Func<T, bool> expression);
        Task<Result<T>> Add(T entity);
        Task<Result<T>> AddRange(IEnumerable<T> entities);
        Task<Result<T>> Remove(T entity);
        Task<Result<T>> RemoveRange(IEnumerable<T> entities);
        Task<Result<T>> Update(T entity);
        Task<Result<T>> GetByIdAsNoTracking(Guid id);
        Task<Result<Assigment>> GetByIdAsNoTrackingAssigemnt(Guid id);
    }
}
