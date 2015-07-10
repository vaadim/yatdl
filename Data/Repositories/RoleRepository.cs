using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace YATDL.Repositories
{
    public class RoleRepository : EfRepository<Role>
    {
        public RoleRepository()
            : base(new YATDLContext())
        {
        }

        public Role[] GetRolesForUser(string userName)
        {
            return Query.Include(u => u.Users).Where(f => f.Users.Any(a => string.Equals(userName, a.UserName))).ToArray();
        }

        public Role GetRole(string roleName)
        {
            return Query.Where(f => f.Name == roleName).Include(u => u.Users).FirstOrDefault();
        }

        public Role GetRoleByDescr(string roleDescr)
        {
            return Query.Where(f => f.Description == roleDescr).Include(u => u.Users).FirstOrDefault();
        }

        public IEnumerable<Role> GetAllRoles()
        {
            return Query.Include(u => u.Users).ToList();
        }
    }
}