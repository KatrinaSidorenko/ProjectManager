using Core.Models;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DAL.Services
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ProjectManagerContext _context;
        private DbSet<T> _entities;
        //protected IEnumerable<PropertyInfo> _properties;
        public Repository(ProjectManagerContext context)
        {
            _context = context;
            _entities = context.Set<T>();   
            //_properties = typeof(T).GetProperties();
        }

        public async Task<Result<T>> Add(T entity)
        {
            try
            {
                if (entity == null)
                {
                    return new Result<T>(false, $"{typeof(T)} entity is null");
                }

                await _entities.AddAsync(entity);

                return new Result<T>(true);
            }
            catch (Exception ex)
            {
                return new Result<T>(false, $"{typeof(T)} entity is null. Message: {ex.Message}");
            }
        }

        public async Task<Result<T>> AddRange(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                {
                    return new Result<T>(false, $"{typeof(T)} entity is null");
                }

                await _entities.AddRangeAsync(entities);

                return new Result<T>(true);
            }
            catch (Exception ex)
            {
                return new Result<T>(false, $"Fail to add renage of entities {ex.Message}");
            }
            
        }

        public async Task<Result<T>> Find(Func<T, bool> expression)
        {
            try
            {
                if (expression == null)
                    return new Result<T>(false, "Experssion is null");

                var entity = await _entities.FindAsync(expression);

                if (entity == null)
                    return new Result<T>(false, $"Fail to find entitie");

                return new Result<T>(true, entity);
            }
            catch (Exception ex)
            {
                return new Result<T>(false, $"Fail to find entitie  {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<T>>> GetAll()
        {
            try
            {
                var entities = await _entities.ToListAsync();

                return new Result<IEnumerable<T>>(true, entities);
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<T>>(false, $"Fail to find entities  {ex.Message}");
            }
        }

        public async Task<Result<T>> GetById(Guid id)
        {
            try
            {
                var entity = await _entities.FindAsync(id);

                if (entity == null)
                    return new Result<T>(false, "Fail to find entity bu id");

                return new Result<T>(true, entity);
            }
            catch (Exception ex)
            {
                return new Result<T>(false, $"Fail to find entity bu id : {ex.Message}");
            }
        }

        public async Task<Result<T>> Remove(T entity)
        {
            try
            {
                if (entity == null)
                {
                    return new Result<T>(false, "Entity is null");
                }

                _entities.Remove(entity);
                
                return new Result<T>(true);
            }
            catch (Exception ex)
            {
                return new Result<T>(false, $"Fail to remove entity {ex.Message}");
            }
        }

        public async Task<Result<T>> RemoveRange(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    return new Result<T>(false, "Fail to remove range of entities");

                _entities.RemoveRange(entities);

                return new Result<T>(true);
            }
            catch(Exception ex)
            {
                return new Result<T>(false, $"Fail to remove range of entities {ex.Message}");
            }
            
        }

        public async Task<Result<T>> Update(T entity)
        {
            try
            {
                if (entity == null)
                    return new Result<T>(false, "Fail to update, entity is null");

                _entities.Update(entity);

                return new Result<T>(true);
            }
            catch (Exception ex)
            {
                return new Result<T>(false, $"Fail to update,{ex.Message}");
            }

        }

        public async Task<Result<T>> GetByIdAsNoTracking(Guid id)
        {
            try
            {
                var entity = await _entities.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

                if (entity == null)
                    return new Result<T>(false, "Fail to get with no tracking entity");

                return new Result<T>(true, entity);
            }
            catch (Exception ex)
            {
                return new Result<T>(false, $"Fail to get with no tracking entity: {ex.Message}");
            }
           
        }

        public async Task<Result<Assigment>> GetByIdAsNoTrackingAssigemnt(Guid id)
        {
            try
            {
                var entity =  await _context.Assigments.AsNoTracking().Include(a => a.UserAssigments).FirstOrDefaultAsync(e => e.Id == id);

                if (entity == null)
                    return new Result<Assigment>(false, "Fail to get with no tracking entity");

                return new Result<Assigment>(true, entity);
            }
            catch (Exception ex)
            {
                return new Result<Assigment>(false, $"Fail to get with no tracking entity: {ex.Message}");
            }

        }
    }
}
