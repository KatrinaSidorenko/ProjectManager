using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Interfaces
{
    public interface IConsoleManager<TEntity> where TEntity : BaseEntity 
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetByPredicate(Func<TEntity, bool> predicate);
    }
}
