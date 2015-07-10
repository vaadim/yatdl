using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using YATDL.Repositories;

namespace YATDL.Security
{
    public class CodeFirstRoleProvider : RoleProvider
    {
        public override string ApplicationName { get; set; }


        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "CodeFirstRoleProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "CodeFirst Extended Role Provider");
            }
            base.Initialize(name, config);

            ApplicationName = GetValueOrDefault(config, "applicationName", o => o.ToString(), "YATDL");

            config.Remove("name");
            config.Remove("description");
            config.Remove("applicationName");
            config.Remove("connectionStringName");

            if (config.Count <= 0)
                return;

            string key = config.GetKey(0);
            if (string.IsNullOrEmpty(key))
                return;

            throw new ProviderException(string.Format(CultureInfo.CurrentCulture,
                                                      "The role provider does not recognize the configuration attribute {0}.",
                                                      key));
        }

        public override bool RoleExists(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return false;

            using (var context = new YATDLContext())
            {
                bool isExists = context.Roles.Any(rl => rl.Name == roleName);
                return isExists;
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleName))
                return false;

            using (var context = new YATDLContext())
            {
                User user = context.Users.Include(r => r.Roles).FirstOrDefault(usr => usr.UserName == username);
                if (user == null)
                    return false;

                Role role = context.Roles.FirstOrDefault(rl => rl.Name == roleName);
                if (role == null)
                    return false;

                return user.Roles.Contains(role);
            }
        }

        public override string[] GetAllRoles()
        {
            using (var context = new YATDLContext())
            {
                return context.Roles.Select(rl => rl.Name).ToArray();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return null;

            using (var context = new YATDLContext())
            {
                Role role = context.Roles.Include(p => p.Users).FirstOrDefault(rl => rl.Name == roleName);
                return role != null
                           ? role.Users.Select(usr => usr.UserName).ToArray()
                           : null;
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var repo = new UserRepository();

            User user = repo.GetUser(username);
            return user != null
                        ? user.Roles.Select(rl => rl.Name).ToArray()
                        : null;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(usernameToMatch))
                return null;

            using (var context = new YATDLContext())
            {
                return (from rl in context.Roles
                        from usr in rl.Users
                        where rl.Name == roleName && usr.UserName.Contains(usernameToMatch)
                        select usr.UserName).ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return;

            using (var context = new YATDLContext())
            {
                Role role = context.Roles.FirstOrDefault(rl => rl.Name == roleName);
                if (role != null)
                    return;

                var newRole = new Role
                                  {
                                      Id = Guid.NewGuid(),
                                      Name = roleName
                                  };
                context.Roles.Add(newRole);
                context.SaveChanges();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
                return false;

            using (var context = new YATDLContext())
            {
                Role role = context.Roles.Include(p => p.Users).FirstOrDefault(rl => rl.Name == roleName);
                if (role == null)
                    return false;

                if (throwOnPopulatedRole)
                {
                    if (role.Users.Any())
                        return false;
                }
                else
                    role.Users.Clear();

                context.Roles.Remove(role);
                context.SaveChanges();
                return true;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var context = new YATDLContext())
            {
                var users = context.Users.Include(p => p.Roles).Where(usr => usernames.Contains(usr.UserName)).ToList();
                var roles = context.Roles.Where(rl => roleNames.Contains(rl.Name)).ToList();
                foreach (User user in users)
                {
                    foreach (Role role in roles)
                    {
                        if (user.Roles.All(p => !String.Equals(p.Name, role.Name, StringComparison.OrdinalIgnoreCase)))
                            user.Roles.Add(role);
                    }
                }
                context.SaveChanges();
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var repo = new UserRepository();
            foreach (string username in usernames)
            {
                string us = username;
                User user = repo.GetUser(us);
                if (user != null)
                {
                    foreach (string roleName in roleNames)
                    {
                        Role role = user.Roles.FirstOrDefault(r => String.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
                        if (role != null)
                            user.Roles.Remove(role);
                    }
                }
                repo.Update(user);
            }
        }

        private static T GetValueOrDefault<T>(NameValueCollection nvc, string key, Func<object, T> converter, T defaultIfNull)
        {
            string val = nvc[key];

            if (val == null)
                return defaultIfNull;

            try
            {
                return converter(val);
            }
            catch
            {
                return defaultIfNull;
            }
        }
    }
}