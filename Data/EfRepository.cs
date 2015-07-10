using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace YATDL
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly IDbSet<T> _dbSet;

        public EfRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public DbContext DbContext
        {
            get { return _dbContext; }
        }

        public IQueryable<T> Query { get { return _dbSet; } }

        public T Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public T MarkToAdd(T entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public ICollection<T> MarkToAdd(ICollection<T> entities)
        {
            bool isAutoDetectWasEnabled = _dbContext.Configuration.AutoDetectChangesEnabled;
            if (isAutoDetectWasEnabled)
                _dbContext.Configuration.AutoDetectChangesEnabled = false;

            foreach (var entity in entities)
                _dbSet.Add(entity);

            _dbContext.Configuration.AutoDetectChangesEnabled = isAutoDetectWasEnabled;
            if (isAutoDetectWasEnabled)
                _dbContext.ChangeTracker.DetectChanges();

            return entities;
        }
        
        public T MarkToUpdate(T entity)
        {
            if (!_dbSet.Local.Contains(entity))
            {
                _dbSet.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }

            return entity;
        }

        public void MarkToDelete(T entity)
        {
            if (!_dbSet.Local.Contains(entity))
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);
        }

        public void MarkToDelete(ICollection<T> entities)
        {
            bool isAutoDetectWasEnabled = _dbContext.Configuration.AutoDetectChangesEnabled;
            if (isAutoDetectWasEnabled)
                _dbContext.Configuration.AutoDetectChangesEnabled = false;

            foreach (var entity in entities)
                MarkToDelete(entity);

            _dbContext.Configuration.AutoDetectChangesEnabled = isAutoDetectWasEnabled;
            if (isAutoDetectWasEnabled)
                _dbContext.ChangeTracker.DetectChanges();
        }

        public T Add(T entity)
        {
            MarkToAdd(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public T Update(T entity)
        {
            MarkToUpdate(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Delete(T entity)
        {
            MarkToDelete(entity);
            _dbContext.SaveChanges();
        }

        public int SaveMarkedChanges()
        {
            return _dbContext.SaveChanges();
        }

        public Exception TrySaveMarkedChanges(int countTries = 1, int intervalBetweenTriesMs = 500)
        {
            if (countTries <= 0 || countTries >= 100)
                throw new ArgumentException("countTries должен быть от 1 до 100");

            Exception lastException = null;

            for (int i = 0; i < countTries; i++)
            {
                try
                {
                    this.DbContext.SaveChanges();
                    return null;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (intervalBetweenTriesMs > 0)
                        Thread.Sleep(intervalBetweenTriesMs);
                }
            }
            return lastException;
        }
    }   
}