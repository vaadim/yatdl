using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace YATDL
{
    public interface IRepository<T> where T : class
    {
        DbContext DbContext { get; }
        IQueryable<T> Query { get; }
        //bool DoNotTrackChanges { get; set; }
        T Find(params object[] keyValues);
        
        T MarkToAdd(T entity);
        ICollection<T> MarkToAdd(ICollection<T> entities);

        T MarkToUpdate(T entity);
        
        void MarkToDelete(T entity);
        void MarkToDelete(ICollection<T> entities);

        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
        int SaveMarkedChanges();
        Exception TrySaveMarkedChanges(int countTries = 1, int intervalBetweenTriesMs = 500);
    }
}