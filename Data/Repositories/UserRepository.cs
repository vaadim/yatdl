using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace YATDL.Repositories
{
    public class UserRepository : EfRepository<User>
    {
        public UserRepository()
            : base(new YATDLContext())
        {
        }

        public User GetUser(string userName)
        {
            return Query.Where(f => f.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).Include(u => u.Roles).Include(p => p.UserProfile).FirstOrDefault();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return Query.Include(u => u.Roles).Include(p => p.UserProfile).ToList();
        }

        public User GetUserById(Guid userId)
        {
            return Query.Where(f => f.Id == userId).Include(u => u.Roles).Include(p => p.UserProfile).FirstOrDefault();
        }
    }
}