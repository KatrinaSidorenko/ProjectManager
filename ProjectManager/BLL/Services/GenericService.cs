using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public abstract class GenericService<T> : IGenericService<T> where T : BaseEntity
    {
        protected IRepository<T> _repository;
        protected IEnumerable<PropertyInfo> _properties;     
        public GenericService(IRepository<T> repository)
        {
            _repository = repository;
        }
        protected async Task GenericEdit(Guid id, object propertyForEdit, string propertyName)
        {
            if (id != Guid.Empty && _properties.Any(p => p.Name == propertyName))
            {
                var entity = await GetById(id);
                var property = _properties.FirstOrDefault(prop => prop.Name == propertyName);
                property.SetValue(entity, propertyForEdit);

                await Update(id, entity);
            }
        }

        public async Task Add(T entity)
        {
            try
            {
                var result = await _repository.Add(entity);
                if (!result.IsSuccessful)
                {
                    throw new InvalidOperationException($"Faild to add {typeof(T).Name}");
                }
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Faild to add {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                var result = await _repository.Delete(id);
                if (!result.IsSuccessful)
                {
                    throw new InvalidOperationException($"Faild to delete {typeof(T).Name}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Faild to delete {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task<List<T>> GetAll()
        {
            try
            {
                var result = await _repository.GetAll();

                if (!result.IsSuccessful)
                {
                    throw new InvalidOperationException($"Faild to get all {typeof(T).Name}");
                }

                return result.Data;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Faild to add {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task<T> GetById(Guid id)
        {
            try
            {
                var entity = await _repository.GetById(id);

                if (!entity.IsSuccessful)
                {
                    throw new InvalidOperationException($"Faild to get by id {typeof(T).Name}");
                }

                return entity.Data;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Faild to get by id {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task<T> GetByPredicate(Func<T, bool> predicate)
        {
            try
            {
                var entity = await _repository.GetByPredicate(predicate);

                if (!entity.IsSuccessful)
                {
                    throw new InvalidOperationException($"Faild to get by id {typeof(T).Name}");
                }

                return entity.Data;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Faild to get by id {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task Update(Guid id, T entity)
        {
            try
            {
                var result = await _repository.Update(id, entity);
                if (!result.IsSuccessful)
                {
                    throw new InvalidOperationException($"Faild to update {typeof(T).Name}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Faild to update {typeof(T).Name}: {ex.Message}");
            }
        }
    }
}
