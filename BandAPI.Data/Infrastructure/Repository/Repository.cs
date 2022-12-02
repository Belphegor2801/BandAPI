using BandAPI.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BandAPI.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbset;
        private DbContext _dataContext;

        public List<Task> ListEsTask = new List<Task>();

        public Repository(DbContext dataContext)
        {
            _dataContext = dataContext;
            _dbset = _dataContext.Set<T>();
        }

        protected IDatabaseFactory DatabaseFactory { get; }

        protected DbContext DataContext => _dataContext ?? (_dataContext = DatabaseFactory.GetDbContext());

        public List<Task> GetAllTask()
        {
            return ListEsTask;
        }

        public virtual IQueryable<T> SqlQuery(string sql, params object[] parameters)
        {
            return _dbset.FromSqlRaw(sql, parameters);
        }

        public virtual int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _dataContext.Database.ExecuteSqlRaw(sql, parameters);
        }

        public virtual Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return _dataContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public virtual T Find(params object[] id)
        {
            return _dbset.Find(id);
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate).FirstOrDefault();
        }

        public virtual Task<T> FindAsync(params object[] id)
        {
            return _dbset.FindAsync(id).AsTask();
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbset.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual bool Any(params object[] id)
        {
            var entity = _dbset.Find(id);
            if (entity == null) return false;
            _dataContext.Entry(entity).State = EntityState.Detached;
            return true;
        }

        public virtual async Task<bool> AnyAsync(params object[] id)
        {
            var entity = await _dbset.FindAsync(id);
            if (entity == null) return false;
            _dataContext.Entry(entity).State = EntityState.Detached;
            return true;
        }

        public virtual int Count()
        {
            return _dbset.Count();
        }

        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.AsNoTracking().Where(predicate);
            return objects.Count();
        }

        public virtual Task<int> CountAsync()
        {
            return _dbset.CountAsync();
        }

        public virtual Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.AsNoTracking().Where(predicate);
            return objects.CountAsync();
        }

        public virtual long LongCount()
        {
            return _dbset.LongCount();
        }

        public virtual long LongCount(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.AsNoTracking().Where(predicate);
            return objects.LongCount();
        }

        public virtual Task<long> LongCountAsync()
        {
            return _dbset.LongCountAsync();
        }

        public virtual Task<long> LongCountAsync(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.AsNoTracking().Where(predicate);
            return objects.LongCountAsync();
        }

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            _dbset.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.Where(predicate).FirstOrDefault();
            if (objects != null)
            {
                _dbset.Remove(objects);
            }
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            //foreach (var entity in entities)
            //{
            //    _dbset.Add(entity);
            //}
            var enumerable = entities as T[] ?? entities.ToArray();
            _dbset.AddRange(enumerable);
        }

        public virtual void AddRange(params T[] entities)
        {
            _dbset.AddRange(entities);
        }

        public virtual void DeleteRange(params T[] entities)
        {
            _dbset.RemoveRange(entities);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            var enumerable = entities as T[] ?? entities.ToArray();
            _dbset.RemoveRange(enumerable);
        }

        public virtual void DeleteRange(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.Where(predicate);
            if (objects.Any())
            {
                _dbset.RemoveRange(objects);
            }
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbset.AsQueryable();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate, string sort = "")
        {
            return _dbset.Where(predicate).ApplySorting(sort);
        }

        public virtual IQueryable<T> GetInclude(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbset;

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }

        public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string sort = "")
        {
            if (predicate == null) return _dbset.ApplySorting(sort).ToListAsync();
            return _dbset.Where(predicate).ApplySorting(sort).ToListAsync();
        }

        public virtual Task<List<T>> GetListAsync(IQueryable<T> iqueryable, string sort = "")
        {
            return _dbset.ApplySorting(sort).ToListAsync();
        }

        public virtual DbSet<T> DbSet()
        {
            return _dbset;
        }

        #region GetPage

        public virtual Pagination<T> GetPage(PaginationRequest query)
        {
            var dataSet = _dbset.AsNoTracking().AsQueryable();
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {
                dataSet = dataSet.ApplySorting(query.Sort);
                var totals = dataSet.Count();
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                var items = dataSet.Skip(excludedRows).Take(query.Size.Value).ToList();
                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count,
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var items = dataSet.ToList();

                var totals = dataSet.Count();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }

        public virtual Pagination<T> GetPage()
        {
            return GetPage(new PaginationRequest { Size = null });
        }

        public virtual Pagination<T> GetPage(Expression<Func<T, bool>> predicate, PaginationRequest query)
        {
            var dataSet = _dbset.AsQueryable().AsNoTracking().Where(predicate);
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {
                dataSet = dataSet.ApplySorting(query.Sort);
                var totals = dataSet.Count();
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                var items = dataSet.Skip(excludedRows).Take(query.Size.Value).ToList();
                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count,
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var totals = dataSet.Count();
                var items = dataSet.ToList();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }

        public virtual async Task<Pagination<T>> GetPageAsync(PaginationRequest query)
        {
            var dataSet = _dbset.AsNoTracking().AsQueryable();
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {
                dataSet = dataSet.ApplySorting(query.Sort);
                var totals = await dataSet.CountAsync();
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                var items = await dataSet.Skip(excludedRows).Take(query.Size.Value).ToListAsync();
                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count,
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var items = await dataSet.ToListAsync();
                var totals = await dataSet.CountAsync();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }

        public virtual async Task<Pagination<T>> GetPageAsync()
        {
            return await GetPageAsync(new PaginationRequest { Size = null });
        }

        public virtual async Task<Pagination<T>> GetPageAsync(Expression<Func<T, bool>> predicate,
            PaginationRequest query)
        {
            var dataSet = _dbset.AsQueryable().AsNoTracking().Where(predicate);
            //var dataSet = _dataContext.Set<T>();
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {
                dataSet = dataSet.ApplySorting(query.Sort);
                var totals = await dataSet.CountAsync();
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                dataSet = dataSet.Skip(excludedRows).Take(query.Size.Value);
                var items = dataSet.ToList();
                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count(),
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var totals = await dataSet.CountAsync();
                var items = await dataSet.ToListAsync();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }

        public async Task<Pagination<T>> GetPageAsync(IQueryable<T> iqueryable, PaginationRequest query)
        {
            iqueryable = iqueryable.AsNoTracking();
            //var dataSet = _dataContext.Set<T>();
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {
                iqueryable = iqueryable.ApplySorting(query.Sort);
                var totals = await iqueryable.CountAsync();
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                iqueryable = iqueryable.Skip(excludedRows).Take(query.Size.Value);
                var items = iqueryable.ToList();

                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count(),
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var totals = await iqueryable.CountAsync();
                var items = await iqueryable.ToListAsync();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }

        #endregion GetPage

        #region custom

        public virtual Task<List<T>> GetListAsyncCustom(IQueryable<T> iqueryable, string sort = "")
        {
            return iqueryable.ApplySorting(sort).ToListAsync();
        }

        #endregion custom
    }
}