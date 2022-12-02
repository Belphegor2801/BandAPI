using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace BandAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseFactory _databaseFactory;
        private DbContext _dataContext;
        private bool _disposed;
        public List<IRepositoryBase> ListRepository = new List<IRepositoryBase>();

        public UnitOfWork(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
            _dataContext = _databaseFactory.GetDbContext();
        }

        public DbContext DataContext => _dataContext ?? (_dataContext = _databaseFactory.GetDbContext());

        public IRepository<T> GetRepository<T>() where T : class
        {
            var repository = new Repository<T>(_dataContext);
            ListRepository.Add(repository);
            return repository;
        }

        public void Migrate()
        {
            DataContext.Database.Migrate();
        }

        public bool EnsureCreated()
        {
            return DataContext.Database.EnsureCreated();
        }

        public Task MigrateAsync()
        {
            return DataContext.Database.MigrateAsync();
        }

        public Task<bool> EnsureCreatedAsync()
        {
            return DataContext.Database.EnsureCreatedAsync();
        }

        public int Save()
        {
            var listTask = new List<Task>();
            foreach (var repository in ListRepository)
            {
                listTask.AddRange(repository.GetAllTask());
            }
            if (listTask.Count > 0)
            {
                Task.WaitAll(listTask.ToArray());
            }
            return DataContext.SaveChanges();
        }

        public bool CheckConnection()
        {
            return DataContext.Database.GetDbConnection().State == System.Data.ConnectionState.Connecting;
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var listTask = new List<Task>();
            foreach (var repository in ListRepository)
            {
                listTask.AddRange(repository.GetAllTask());
            }
            if (listTask.Count > 0)
            {
                await Task.WhenAll(listTask);
            }
            return await DataContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    _dataContext.Dispose();
                    _disposed = true;
                }

            _disposed = false;
        }

        public DbContext GetDbContext()
        {
            return _dataContext;
        }
    }
}