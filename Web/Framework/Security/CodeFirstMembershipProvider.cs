using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using YATDL.Repositories;

namespace YATDL.Security
{
    public class CodeFirstMembershipProvider : MembershipProvider
    {
        private bool _enablePasswordRetrieval;
        private bool _enablePasswordReset;
        private bool _requiresQuestionAndAnswer;
        private string _hashAlgorithmType;

        #region Properties

        public override string ApplicationName { get; set; }

        private int _maxInvalidPasswordAttempts;
        public override int MaxInvalidPasswordAttempts { get { return _maxInvalidPasswordAttempts; } }

        private int _minRequiredNonAlphanumericCharacters;
        public override int MinRequiredNonAlphanumericCharacters { get { return _minRequiredNonAlphanumericCharacters; } }

        private int _minRequiredPasswordLength;
        public override int MinRequiredPasswordLength { get { return _minRequiredPasswordLength; } }

        private int _passwordAttemptWindow;
        public override int PasswordAttemptWindow { get { return _passwordAttemptWindow; } }

        private MembershipPasswordFormat _passwordFormat;
        public override MembershipPasswordFormat PasswordFormat { get { return _passwordFormat; } }

        private string _passwordStrengthRegularExpression;
        public override string PasswordStrengthRegularExpression { get { return _passwordStrengthRegularExpression; } }

        private bool _requiresUniqueEmail;
        public override bool RequiresUniqueEmail { get { return _requiresUniqueEmail; } }

        #endregion

        #region Functions

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "CodeFirstMembershipProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Simple Security Membership Provider");
            }

            base.Initialize(name, config);

            ApplicationName = GetValueOrDefault(config, "applicationName", o => o.ToString(), "YATDL");

            _enablePasswordRetrieval = GetValueOrDefault(config, "enablePasswordRetrieval", Convert.ToBoolean, false);
            _enablePasswordReset = GetValueOrDefault(config, "enablePasswordReset", Convert.ToBoolean, true);
            _requiresQuestionAndAnswer = GetValueOrDefault(config, "requiresQuestionAndAnswer", Convert.ToBoolean, false);
            _requiresUniqueEmail = GetValueOrDefault(config, "requiresUniqueEmail", Convert.ToBoolean, true);
            _maxInvalidPasswordAttempts = GetValueOrDefault(config, "maxInvalidPasswordAttempts", Convert.ToInt32, 3);
            _passwordAttemptWindow = GetValueOrDefault(config, "passwordAttemptWindow", Convert.ToInt32, 10);
            _passwordFormat = GetValueOrDefault(config, "passwordFormat", o =>
                                                                          {
                                                                              MembershipPasswordFormat format;
                                                                              return Enum.TryParse(o.ToString(), true, out format) ? format : MembershipPasswordFormat.Hashed;
                                                                          },
                                                                          MembershipPasswordFormat.Hashed);

            _minRequiredPasswordLength = GetValueOrDefault(config, "minRequiredPasswordLength", Convert.ToInt32, 6);
            _minRequiredNonAlphanumericCharacters = GetValueOrDefault(config, "minRequiredNonalphanumericCharacters", Convert.ToInt32, 1);
            _passwordStrengthRegularExpression = GetValueOrDefault(config, "passwordStrengthRegularExpression", o => o.ToString(), string.Empty);
            _hashAlgorithmType = GetValueOrDefault(config, "hashAlgorithmType", o => o.ToString(), "SHA1");

            config.Remove("name");
            config.Remove("description");
            config.Remove("applicationName");
            config.Remove("connectionStringName");
            config.Remove("enablePasswordRetrieval");
            config.Remove("enablePasswordReset");
            config.Remove("requiresQuestionAndAnswer");
            config.Remove("requiresUniqueEmail");
            config.Remove("maxInvalidPasswordAttempts");
            config.Remove("passwordAttemptWindow");
            config.Remove("passwordFormat");
            config.Remove("minRequiredPasswordLength");
            config.Remove("minRequiredNonalphanumericCharacters");
            config.Remove("passwordStrengthRegularExpression");
            config.Remove("hashAlgorithmType");

            if (config.Count <= 0)
                return;

            string key = config.GetKey(0);
            if (string.IsNullOrEmpty(key))
                return;

            throw new ProviderException(string.Format(CultureInfo.CurrentCulture,
                                                      "The membership provider does not recognize the configuration attribute {0}.",
                                                      key));
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (string.IsNullOrEmpty(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (string.IsNullOrEmpty(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (string.IsNullOrEmpty(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            string hashedPassword = Crypto.HashPassword(password);
            if (hashedPassword.Length > 128)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            using (var context = new YATDLContext())
            {
                if (context.Users.Any(u => u.UserName == username))
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }

                if (context.Users.Any(u => u.Email == email))
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = username,
                    Password = hashedPassword,
                    IsApproved = isApproved,
                    Email = email,
                    CreateDate = DateTime.UtcNow,
                    LastPasswordChangedDate = DateTime.UtcNow,
                    PasswordFailuresSinceLastSuccess = 0,
                    LastLoginDate = DateTime.UtcNow,
                    LastActivityDate = DateTime.UtcNow,
                    LastLockoutDate = DateTime.UtcNow,
                    IsLockedOut = false,
                    LastPasswordFailureDate = DateTime.UtcNow,
                    UserProfile = new UserProfile()
                };

                context.Users.Add(newUser);
                context.SaveChanges();
                status = MembershipCreateStatus.Success;
                return new MembershipUser(Membership.Provider.Name, newUser.UserName, newUser.Id, newUser.Email, null, null, newUser.IsApproved, newUser.IsLockedOut, newUser.CreateDate.Value, newUser.LastLoginDate.Value, newUser.LastActivityDate.Value, newUser.LastPasswordChangedDate.Value, newUser.LastLockoutDate.Value);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            var repo = new UserRepository();
            var user = repo.GetUser(username);
            if (user == null)
            {
                return false;
            }
            if (!user.IsApproved)
            {
                return false;
            }
            if (user.IsLockedOut)
            {
                return false;
            }
            string hashedPassword = user.Password;
            bool verificationSucceeded = (hashedPassword != null && Crypto.VerifyHashedPassword(hashedPassword, password));
            if (verificationSucceeded)
            {
                user.PasswordFailuresSinceLastSuccess = 0;
                user.LastLoginDate = DateTime.UtcNow;
                user.LastActivityDate = DateTime.UtcNow;
            }
            else
            {
                int failures = user.PasswordFailuresSinceLastSuccess;
                if (failures < MaxInvalidPasswordAttempts)
                {
                    user.PasswordFailuresSinceLastSuccess += 1;
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                }
                else if (failures >= MaxInvalidPasswordAttempts)
                {
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                    user.LastLockoutDate = DateTime.UtcNow;
                    user.IsLockedOut = true;
                }
            }
            repo.Update(user);

            return verificationSucceeded;

        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            var repo = new UserRepository();

            User user = repo.GetUser(username);
            if (user == null)
                return null;

            if (userIsOnline)
            {
                user.LastActivityDate = DateTime.UtcNow;
                repo.Update(user);
            }
            return new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null,
                                        null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value,
                                        user.LastLoginDate.Value, user.LastActivityDate.Value,
                                        user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey is Guid) { }
            else
            {
                return null;
            }

            using (var context = new YATDLContext())
            {
                User user = context.Users.Find(providerUserKey);
                if (user == null)
                    return null;

                if (userIsOnline)
                {
                    user.LastActivityDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
                return new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null,
                                          null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value,
                                          user.LastLoginDate.Value, user.LastActivityDate.Value,
                                          user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value);
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                return false;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                return false;
            }

            var repo = new UserRepository();
            var user = repo.GetUser(username);
            if (user == null)
                return false;

            string hashedPassword = user.Password;
            bool verificationSucceeded = (hashedPassword != null && Crypto.VerifyHashedPassword(hashedPassword, oldPassword));
            if (verificationSucceeded)
            {
                user.PasswordFailuresSinceLastSuccess = 0;
            }
            else
            {
                int failures = user.PasswordFailuresSinceLastSuccess;
                if (failures < MaxInvalidPasswordAttempts)
                {
                    user.PasswordFailuresSinceLastSuccess += 1;
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                }
                else if (failures >= MaxInvalidPasswordAttempts)
                {
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                    user.LastLockoutDate = DateTime.UtcNow;
                    user.IsLockedOut = true;
                }
                repo.Update(user);
                return false;
            }

            string newHashedPassword = Crypto.HashPassword(newPassword);
            if (newHashedPassword.Length > 128)
                return false;

            user.Password = newHashedPassword;
            user.LastPasswordChangedDate = DateTime.UtcNow;
            repo.Update(user);

            return true;

        }

        public override bool UnlockUser(string userName)
        {
            using (var context = new YATDLContext())
            {
                User user = context.Users.FirstOrDefault(usr => usr.UserName == userName);
                if (user == null)
                    return false;

                user.IsLockedOut = false;
                user.PasswordFailuresSinceLastSuccess = 0;
                context.SaveChanges();
                return true;
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            DateTime dateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(Convert.ToDouble(Membership.UserIsOnlineTimeWindow)));
            var repo = new UserRepository();
            return repo.GetAllUsers().Count(usr => usr.LastActivityDate > dateActive);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            var repo = new UserRepository();
            User user = repo.GetUser(username);
            if (user == null)
                return false;

            repo.Delete(user);
            return true;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();
            using (var context = new YATDLContext())
            {
                totalRecords = context.Users.Count(u => u.UserName == usernameToMatch);
                IQueryable<User> users = context.Users.Where(usr => usr.UserName == usernameToMatch).OrderBy(usrn => usrn.UserName).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (User user in users)
                    membershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value, user.LastLoginDate.Value, user.LastActivityDate.Value, user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value));

            }
            return membershipUsers;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();
            using (var context = new YATDLContext())
            {
                totalRecords = context.Users.Count();
                IQueryable<User> users = context.Users.OrderBy(usrn => usrn.UserName).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (User user in users)
                {
                    membershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.Id, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value, user.LastLoginDate.Value, user.LastActivityDate.Value, user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value));
                }
            }
            return membershipUsers;
        }

        #endregion

        public Guid GetUserIdFromPasswordResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public bool ResetPasswordWithToken(string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        #region Not Supported

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        //CodeFirstMembershipProvider does not support password retrieval scenarios.
        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CodeFirstMembershipProvider does not support password reset scenarios.
        public override bool EnablePasswordReset
        {
            get { return false; }
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CodeFirstMembershipProvider does not support question and answer scenarios.
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CodeFirstMembershipProvider does not support UpdateUser because this method is useless.
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }

        #endregion

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