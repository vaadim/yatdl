using System.Data.Entity;

namespace YATDL
{
    public class DontDoNothingDatabaseInitializer<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
        }
    }
}