using System.Data.Entity;

namespace YATDL
{
    public class DbContextBase : DbContext, IDbContext
    {
        public DbContextBase(string nameOrConnectionString) : 
            base(nameOrConnectionString)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
    }
}