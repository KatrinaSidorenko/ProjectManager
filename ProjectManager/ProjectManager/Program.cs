using System;
using System.Collections.Generic;
using BLL.Services;
using DAL.Services;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using UI;
using UI.ConsoleManagers;
using System.Threading.Tasks;

namespace ProjectManager
{
    internal class Program
    {
         static async Task Main(string[] args)
         {
            var serviceProvider = DependencyRegistration.Register();

            using (var scope = serviceProvider.CreateScope())
            {
                var appManager = scope.ServiceProvider.GetService<AppManager>();
                await appManager.StartAsync();
            }
         }
    }
}
