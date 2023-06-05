using Core.Models;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;

namespace DAL.Services
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly string _filePath;
        public Repository(string filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{typeof(T).Name}.json");
            EnsureFileExists();
        }
        public async Task<Result<bool>> Add(T entity)
        {
            try
            {
                var entites = await GetAllItems();

                if(entites != null)
                {
                    entites.Add(entity);
                }
                else
                {
                    entites = new List<T>();
                    entites.Add(entity);
                }
                
                await WriteToFile(entites);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed add entity to file: {ex.Message}");
            }
        }

        public async Task<Result<bool>> Delete(Guid id)
        {
            try
            {
                var entities = await GetAllItems();
                int index = entities.FindIndex(entity => entity.Id.Equals(id));

                if(index != -1)
                {
                    entities.RemoveAt(index);

                    await WriteToFile(entities);

                    return new Result<bool>(isSuccessful: true);
                } 
                
                return new Result<bool>(isSuccessful: false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed delete entity to file: {ex.Message}");
            }
        }

        public async Task<Result<List<T>>> GetAll()
        {
            try
            {
                var allEntities = await GetAllItems();
                if(allEntities != null)
                {
                    return new Result<List<T>>(isSuccessful: true, data: allEntities);
                }

                return new Result<List<T>>(isSuccessful: false, message: "Fail to get all entities");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed get all entities: {ex.Message}");
            }
        }

        public async Task<Result<T>> GetById(Guid id)
        {
            try
            {
                var entityResult = await GetByPredicate(entity => entity.Id.Equals(id));

                if(entityResult !=  null)
                {
                    return entityResult;
                }

                return new Result<T>(isSuccessful: false, message: "Item not found.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed get entity by id: {ex.Message}");
            }
        }

        public async Task<Result<bool>> Update(Guid id, T entity)
        {
            try
            {
                var entities = await GetAllItems();
                int index = entities.FindIndex(entity => entity.Id.Equals(id));

                if(index != -1)
                {
                    entities[index] = entity;
                    await WriteToFile(entities);

                    return new Result<bool>(isSuccessful: true);
                }

                return new Result<bool>(isSuccessful: false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed update entity: {ex.Message}");
            }
        }

        private async Task WriteToFile(List<T> items)
        {
            try
            {
                using (StreamWriter file = File.CreateText(_filePath))
                {
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        writer.Formatting = Formatting.Indented;
                        JsonSerializer serializer = new JsonSerializer();
                        await Task.Run(() => serializer.Serialize(writer, items));
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed write to file: {ex.Message}");
            }

        }

        private async Task<List<T>> GetAllItems()
        {
            try
            {
                using(StreamReader file = File.OpenText(_filePath))
                {
                    using(JsonReader jsonReader = new JsonTextReader(file))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        return await Task.Run(() => serializer.Deserialize<List<T>>(jsonReader));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed read from file: {ex.Message}");
            }
        }

        public async Task<Result<T>> GetByPredicate(Func<T, bool> predicate)
        {
            try
            {
                var entities = await GetAllItems();
                var entity = entities.FirstOrDefault(predicate);

                if (entity == null)
                {
                    return new Result<T>(isSuccessful: false, message: "Item not found.");
                }

                return new Result<T>(isSuccessful: true, data: entity);
            }
            catch (Exception ex)
            {
                return new Result<T>(isSuccessful: false, message: $"Failed to get item. Exception: {ex.Message}");
            }
        }

        private void  EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                WriteToFile(new List<T>());
            }
        }

    }
}
