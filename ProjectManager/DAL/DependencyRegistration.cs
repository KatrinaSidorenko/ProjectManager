using System;
using Microsoft.Extensions.DependencyInjection;
using DAL.Interfaces;
using DAL.Services;


namespace DAL
{
    public class DependencyRegistration
    {
        public static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
