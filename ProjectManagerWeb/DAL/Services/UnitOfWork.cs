using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using DAL.Interfaces;

namespace DAL.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectManagerContext _context;
        public UnitOfWork(ProjectManagerContext context)
        {
            _context = context;
            Projects = new Repository<Project>(_context);
            Assigments = new Repository<Assigment>(_context);
            UserAssigments = new Repository<UserAssigment>(_context);
            AssigmentFiles = new Repository<AssigmentFile>(_context);
        }
        public Repository<Project> Projects { get; private set; }
        public Repository<Assigment> Assigments { get; private set; }
        public Repository<UserAssigment> UserAssigments { get; private set; }
        public Repository<AssigmentFile> AssigmentFiles { get; private set; }

        public int Complete()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fail to save changes to database: {ex.Message}");
            }
        }
        public void Dispose()
        {
            if(_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
