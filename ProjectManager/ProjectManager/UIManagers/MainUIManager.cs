using BLL.Interfaces;
using Core.Helpers;
using Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UI.Interfaces;

namespace UI.ConsoleManagers
{
    public abstract class MainUIManager<TEntity, TService> : IConsoleManager<TEntity>
        where TEntity : BaseEntity
        where TService : IGenericService<TEntity>
    {
        protected readonly TService Service;

        protected MainUIManager(TService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected async Task<TEntity> CreateObject(object[] arr)
        {
            TEntity newEntity;
            try
            {
                newEntity = (TEntity)Activator.CreateInstance(typeof(TEntity), arr);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return newEntity;
        }


        public async Task<IEnumerable<TEntity>> GetAll()
        {
            try
            {
                return await Service.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fail to get all {typeof(TEntity)}: {ex.Message}");

                return Enumerable.Empty<TEntity>();
            }
        }

        public async Task<TEntity> GetByPredicate(Func<TEntity, bool> predicate)
        {
            try
            {
                return await Service.GetByPredicate(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fail to get by predicate {typeof(TEntity)}: {ex.Message}");
            }
        }
        protected async Task ShowObjects(IList listOfObjects, Type type)
        {
            var properties = type.GetProperties();

            foreach (var obj in listOfObjects)
            {
                Console.WriteLine("--------------------------------");
                foreach (var property in properties)
                {
                    await ShowObjectHelper(property, obj);
                }
                Console.WriteLine("--------------------------------");
                Console.WriteLine();                         
            }
        }
        private async Task ShowObjectHelper(PropertyInfo property, object obj)
        {
            var propValue = property.GetValue(obj);

            switch (property.Name)
            {
                case "Id":
                    break;
                case "Executors":
                    Console.WriteLine();
                    Console.WriteLine("Task Executors: ");
                    var task = (Assigment)obj;
                    if (task != null)
                    {
                        await ShowObjects(task.Executors.ToList(), typeof(User));
                    }
                    break;
                case "ProjectOwner":
                    Console.WriteLine("Project owner: ");
                    if(propValue  != null)
                    {
                        await ShowObjects(new ArrayList() { propValue }, typeof(User));
                    }
                    break;
                case "Password":
                    break;
                case "ProjectId":
                    break;
                case "EmailIsSent":
                    break;
                case "Files":
                    Console.WriteLine();
                    Console.WriteLine("Assigment files: ");
                    var taskFiless = (Assigment)obj;
                    if (taskFiless != null && taskFiless.Files != null)
                    {
                        foreach(var file in taskFiless.Files)
                        {
                            Console.WriteLine($"File path: {file}");
                        }
                    }
                    break;
                default:
                    Console.WriteLine($"{obj.GetType().Name} {property.Name}: {property.GetValue(obj)}");
                    break;

            }
        }

        protected async Task ReinvokeMethodHelper(Func<Task> action, Func<Task> exitAction)
        {
            Console.WriteLine("If you want try again enter 1, else another number");
            int input = InputValidator.IntegerValidator();

            if(input != 1)
            {
                await exitAction();
            }
            else
            {
                await action();
            }            
        }

    }
}
