using InsightRESTAPI.Model.Data;
using InsightRESTAPI.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace InsightRESTAPI.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository<Notes> Notes { get; private set; }
        public GenericRepository<RefreshToken> RefreshToken { get; private set; }
        public GenericRepository<ApplicationUser> ApplicationUser { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Notes = new GenericRepository<Notes>(_context);
            ApplicationUser = new GenericRepository<ApplicationUser>(_context);
            RefreshToken = new GenericRepository<RefreshToken>(_context);
            
        }

        public int Save()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // Garbage Collector
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
