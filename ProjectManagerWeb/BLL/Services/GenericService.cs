using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public abstract class GenericService<T> : IGenericService<T> where T : BaseEntity
    {
        protected IUnitOfWork _unitOfWork;
        protected IRepository<T> _repository;     
        public GenericService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Add(T entity)
        {
            try
            {
                var result = await _repository.Add(entity);

                if (!result.IsSuccessful)
                    throw new InvalidOperationException(result.Message);

                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Faild to add {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                var entity = await GetById(id);

                var result = await _repository.Remove(entity);

                if (!result.IsSuccessful)
                    throw new InvalidOperationException(result.Message);

                _unitOfWork.Complete();
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

                if(!result.IsSuccessful)
                    throw new InvalidOperationException(result.Message);

                return result.Data.ToList();
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
                
                var result = await _repository.GetById(id);

                if(!result.IsSuccessful)
                    throw new InvalidOperationException(result.Message);

                return result.Data;
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
                var result = await _repository.Find(predicate);

                if(!result.IsSuccessful)
                    throw new InvalidOperationException(result.Message);

                return result.Data;
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
                var result = await _repository.Update(entity);

                if(!result.IsSuccessful)
                    throw new InvalidOperationException(result.Message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Faild to update {typeof(T).Name}: {ex.Message}");
            }
        }
    }
}
