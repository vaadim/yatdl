using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using YATDL.Repositories;

namespace YATDL.Security
{
    public sealed class WebSecurity
    {
        public static HttpContextBase Context
        {
            get { return new HttpContextWrapper(HttpContext.Current); }
        }

        public static HttpRequestBase Request
        {
            get { return Context.Request; }
        }

        public static bool RequireEmailConfirmation
        {
            get { return false; }
        }

        public static HttpResponseBase Response
        {
            get { return Context.Response; }
        }

        public static System.Security.Principal.IPrincipal User
        {
            get { return Context.User; }
        }

        public static bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        public static MembershipCreateStatus Register(string username, string password, string email, bool isApproved, string firstName, string lastName)
        {
            MembershipCreateStatus createStatus;
            Membership.CreateUser(username, password, email, null, null, isApproved, null, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                InitUserProfile(username, firstName, lastName, email, false);
            }

            return createStatus;
        }

        static string InitUserProfile(string username, string firstname, string lastname, string email, bool createToken)
        {
            var repo = new UserRepository();
            var token = string.Empty;
            var user = repo.GetUser(username);
            if (user != null)
            {
                user.UserProfile.FirstName = firstname;
                user.UserProfile.LastName = lastname;

                //create token for email confirmation
                if (createToken)
                {
                    token = GenerateToken();
                    user.ConfirmationToken = token;
                }
                //otherwise - confirmed
                else
                {
                    user.IsConfirmed = true;
                }

                repo.Update(user);
            }
            return token;
        }

        public static string RegisterWithToken(string username, string password, string email, string firstName, string lastName)
        {
            if (Register(username, password, email, false, firstName, lastName) == MembershipCreateStatus.Success)
                return InitUserProfile(username, firstName, lastName, email, true);

            return null;
        }

        public enum MembershipLoginStatus
        {
            Success, Failure
        }

        public static MembershipLoginStatus Login(string userName, string password, bool rememberMe)
        {
            if (Membership.ValidateUser(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, rememberMe);
                return MembershipLoginStatus.Success;
            }

            return MembershipLoginStatus.Failure;
        }

        public static void Logout()
        {
            FormsAuthentication.SignOut();
        }

        public static MembershipUser GetUser(string userName)
        {
            return Membership.GetUser(userName);
        }

        public static bool UserInRole(string username, string rolename = "")
        {
            rolename = string.IsNullOrEmpty(rolename)
                ? "Administrator"
                : rolename;

            var roles = (CodeFirstRoleProvider)Roles.Provider;

            if (roles == null)
                return false;

            return roles.IsUserInRole(username, rolename);
        }

        public static void AddToRole(string username, string rolename = "")
        {
            rolename = string.IsNullOrEmpty(rolename)
                ? "Administrator"
                : rolename;

            if (UserInRole(username, rolename))
                return;

            var roles = (CodeFirstRoleProvider)Roles.Provider;

            if (roles == null)
                return;

            roles.AddUsersToRoles(new [] { username }, new [] { rolename });
        }

        public static void RemoveFromRole(string username, string rolename = "")
        {
            rolename = string.IsNullOrEmpty(rolename)
                ? "Administrator"
                : rolename;

            if (!UserInRole(username, rolename))
                return;

            var roles = (CodeFirstRoleProvider)Roles.Provider;

            if (roles == null)
                return;

            roles.RemoveUsersFromRoles(new[] { username }, new[] { rolename });
        }

        public static bool ChangePassword(string oldPassword, string newPassword)
        {
            var currentUser = Membership.GetUser(User.Identity.Name);
            if (currentUser == null)
                return false;
            return currentUser.ChangePassword(oldPassword, newPassword);
        }

        public static bool DeleteUser(string userName)
        {
            return Membership.DeleteUser(userName);
        }

        public static List<MembershipUser> FindUsersByEmail(string email, int pageIndex, int pageSize)
        {
            int totalRecords;
            return Membership.FindUsersByEmail(email, pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList();
        }

        public static List<MembershipUser> FindUsersByName(string userName, int pageIndex, int pageSize)
        {
            int totalRecords;
            return Membership.FindUsersByName(userName, pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList();
        }

        public static List<MembershipUser> GetAllUsers(int pageIndex, int pageSize)
        {
            int totalRecords;
            return Membership.GetAllUsers(pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList();
        }

        private static Guid VerifyUserNameHasConfirmedAccount(string userName, bool throwException)
        {
            var repo = new UserRepository();

            var user = repo.GetUser(userName);
            if (user == null)
            {
                if (throwException)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                        "User {0} does not exist!",
                                                                        new object[] {userName}));
                }
            }

            return user == null || !user.IsConfirmed ? Guid.Empty : user.Id;
        }

        private static string GenerateToken()
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                return GenerateToken(provider);
            }
        }

        internal static string GenerateToken(RandomNumberGenerator generator)
        {
            var data = new byte[0x10];
            generator.GetBytes(data);
            return HttpServerUtility.UrlTokenEncode(data);
        }

        const int TokenExpirationInMinutesFromNow = 15;

        public static string GeneratePasswordResetToken(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Username cannot be empty");
            }

            var repo = new UserRepository();
            var userId = VerifyUserNameHasConfirmedAccount(userName, false);

            var user = repo.GetUserById(userId);

            if (user == null)
                return null;

            if (user.PasswordVerificationTokenExpirationDate.HasValue &&
                user.PasswordVerificationTokenExpirationDate.Value > DateTime.UtcNow)
            {
                return user.PasswordVerificationToken;
            }

            var token = GenerateToken();
            user.PasswordVerificationToken = token;
            user.PasswordVerificationTokenExpirationDate =
                DateTime.UtcNow.AddMinutes(TokenExpirationInMinutesFromNow);

            repo.Update(user);

            return token;
        }

        public static bool UserCheckResetPasswordToken(string userId, string token)
        {
            var membership = (CodeFirstMembershipProvider)Membership.Provider;

            return string.Equals(userId, membership.GetUserIdFromPasswordResetToken(token).ToString());
        }

        public static bool ResetPassword(string token, string password, out string errorMsg)
        {
            errorMsg = string.Empty;
            var membership = (CodeFirstMembershipProvider)Membership.Provider;
            try
            {
                return membership.ResetPasswordWithToken(token, password);
            }
            catch (Exception exc)
            {
                errorMsg = exc.Message;
                return false;
            }
        }

        static Random _random = new Random((int)DateTime.Now.Ticks);

        public static string GeneratePassword(int size)
        {
            var builder = new StringBuilder();
            char ch;
            lock(_random)
                for (int i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));
                    builder.Append(ch);
                }

            return builder.ToString();
        }

        public static bool ConfirmAccount(string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }
    }
}