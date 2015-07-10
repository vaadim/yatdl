using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace YATDL
{
    public class YATDLContext : DbContextBase
    {
        static YATDLContext()
        {
            Database.SetInitializer(new YATDLDatabaseInitializer());
        }

        public YATDLContext()
            : base("YATDLConnection")
        {
        }

        public static YATDLContext CreateReadOnly()
        {
            var context = new YATDLContext();
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;

            return context;
        }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<User>()
                .HasMany<Role>(r => r.Roles)
                .WithMany(s => s.Users)
                .Map(p =>
                {
                    p.MapLeftKey(new string [] { });
                    p.MapRightKey(new string[] { });
                    p.ToTable("UsersInRoles");
                });
        }
    }
}