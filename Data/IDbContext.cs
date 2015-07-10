using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace YATDL
{
    public interface IDbContext : IDisposable
    {
        IDbSet<T> Set<T>() where T : class;
        int SaveChanges();

        DbEntityEntry Entry(object o);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}