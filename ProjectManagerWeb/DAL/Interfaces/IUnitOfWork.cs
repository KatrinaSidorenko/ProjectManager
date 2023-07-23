using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using DAL.Interfaces;
using DAL.Services;

namespace DAL.Interfaces
{ 
    public interface IUnitOfWork : IDisposable
    {
        Repository<Project> Projects { get; }
        Repository<Assigment> Assigments { get; }
        Repository<UserAssigment> UserAssigments { get; }
        Repository<AssigmentFile> AssigmentFiles { get; }
        int Complete();
    }
}
