using System;

namespace YATDL
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        void Dispose(bool disposing);
        IRepository<T> Repository<T>() where T : class;
    }
}