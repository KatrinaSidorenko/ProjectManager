using System;
using BLL.Interfaces;
using BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public class DependencyRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAssigmentService, AssigmentService>();
            services.AddScoped<IProjectService, ProjectService>();

            DAL.DependencyRegistration.RegisterRepositories(services);
        }
    }
}
