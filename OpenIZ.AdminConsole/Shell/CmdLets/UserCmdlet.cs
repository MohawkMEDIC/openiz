using MohawkCollege.Util.Console.Parameters;
using OpenIZ.AdminConsole.Attributes;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Messaging.AMI.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole.Shell.CmdLets
{
    /// <summary>
    /// Commandlet for user commands
    /// </summary>
    [AdminCommandlet]
    public static class UserCmdlet
    {

        /// <summary>
        /// Base class for user operations
        /// </summary>
        internal class GenericUserParms
        {

            /// <summary>
            /// Gets or sets the username
            /// </summary>
            [Description("The username of the user")]
            [Parameter("u")]
            public String UserName { get; set; }

        }
        
        // Ami client
        private static AmiServiceClient m_client = new AmiServiceClient(ApplicationContext.Current.GetRestClient(Core.Interop.ServiceEndpointType.AdministrationIntegrationService));

        #region User Add
        internal class UseraddParms : GenericUserParms
        {
            
            /// <summary>
            /// Gets or sets the password
            /// </summary>
            [Description("The password of the user")]
            [Parameter("p")]
            public String Password { get; set; }

            /// <summary>
            /// Gets or sets the roles
            /// </summary>
            [Description("Adds the user to the specified role")]
            [Parameter("r")]
            public StringCollection Roles { get; set; }

            /// <summary>
            /// The name of the user to add
            /// </summary>
            [Description("The email of the user to create")]
            [Parameter("e")]
            public string Email { get; set; }
        }

        /// <summary>
        /// Useradd parameters
        /// </summary>
        [AdminCommand("useradd", "Adds a user to the OpenIZ instance")]
        [Description("This command add the specified user to the OpenIZ IMS instance")]
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateIdentity)]
        internal static void Useradd(UseraddParms parms)
        {

            var roles = new List<SecurityRoleInfo>();

            if(parms.Roles?.Count > 0)
                roles = parms.Roles.OfType<String>().Select(o => m_client.GetRoles(r => r.Name == o).CollectionItem.FirstOrDefault()).ToList();

            m_client.CreateUser(new Core.Model.AMI.Auth.SecurityUserInfo()
            {
                UserName = parms.UserName,
                Password = parms.Password ?? "Mohawk123",
                Email = parms.Email,
                Roles = roles
            });

        }

        #endregion

        #region User Delete/Lock Commands

        /// <summary>
        /// User locking parms
        /// </summary>
        internal class UserLockParms : GenericUserParms
        {

            /// <summary>
            /// The time to set the lock util
            /// </summary>
            [Description("Whether or not to set the lock")]
            [Parameter("l")]
            public bool Locked { get; set; }
        }

        /// <summary>
        /// Useradd parameters
        /// </summary>
        [AdminCommand("userdel", "De-activates a user to the OpenIZ instance")]
        [Description("This command change the obsoletion time of the user effectively de-activating it")]
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        internal static void Userdel(GenericUserParms parms)
        {
            var user = m_client.GetUsers(o => o.UserName == parms.UserName).CollectionItem.FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException($"User {parms.UserName} not found");

            m_client.DeleteUser(user.UserId.ToString());

        }

        /// <summary>
        /// Useradd parameters
        /// </summary>
        [AdminCommand("userundel", "Re-activates a user to the OpenIZ instance")]
        [Description("This command will undo a de-activation and will reset the user's obsoletion time")]
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        internal static void Userudel(GenericUserParms parms)
        {
            var user = m_client.GetUsers(o => o.UserName == parms.UserName).CollectionItem.FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException($"User {parms.UserName} not found");

            user.Lockout = false;
            user.User.ObsoletionTime = null;
            user.User.ObsoletedBy = null;

            m_client.UpdateUser(user.UserId.Value, user);

        }

        /// <summary>
        /// Useradd parameters
        /// </summary>
        [AdminCommand("userlock", "Engages or disengages the user lock")]
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        [Description("This command will change lock status of the user, either setting it or un-setting it")]
        internal static void Userlock(UserLockParms parms)
        {
            var user = m_client.GetUsers(o => o.UserName == parms.UserName).CollectionItem.FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException($"User {parms.UserName} not found");

            user.Lockout = parms.Locked;

            m_client.UpdateUser(user.UserId.Value, user);

        }

        #endregion

        #region User List

        /// <summary>
        /// User list parameters
        /// </summary>
        internal class UserListParms : GenericUserParms
        {

            /// <summary>
            /// Locked
            /// </summary>
            [Description("Filter on locked status")]
            [Parameter("l")]
            public bool Locked { get; set; }

            /// <summary>
            /// Locked
            /// </summary>
            [Description("Filter on active status")]
            [Parameter("a")]
            public bool Active { get; set; }

            /// <summary>
            /// Locked
            /// </summary>
            [Description("Filter on human class only")]
            [Parameter("h")]
            public bool Human { get; set; }

            /// <summary>
            /// Locked
            /// </summary>
            [Description("Filter on system class only")]
            [Parameter("s")]
            public bool System { get; set; }
        }

        /// <summary>
        /// List users
        /// </summary>
        [AdminCommand("userlist", "Lists users in the OpenIZ instance")]
        [Description("This command lists all users in the user database regardless of their status, or class. To filter use the filter parameters listed.")]
        internal static void Userlist(UserListParms parms)
        {
            AmiCollection<SecurityUserInfo> users = null;
            if (parms.Locked && !String.IsNullOrEmpty(parms.UserName))
                users = m_client.GetUsers(o => o.UserName.Contains(parms.UserName) && o.Lockout.HasValue);
            else if (!String.IsNullOrEmpty(parms.UserName))
                users = m_client.GetUsers(o => o.UserName.Contains(parms.UserName));
            else
                users = m_client.GetUsers(o => o.UserName != null);

            if (parms.Active)
                users.CollectionItem = users.CollectionItem.Where(o => !o.User.ObsoletionTime.HasValue).ToList();
            if(parms.Human)
                users.CollectionItem = users.CollectionItem.Where(o => o.User.UserClass == UserClassKeys.HumanUser).ToList();
            else if(parms.System)
                users.CollectionItem = users.CollectionItem.Where(o => o.User.UserClass != UserClassKeys.HumanUser).ToList();

            Console.WriteLine("SID{0}UserName{1}Last Lgn{2}Lockout{2} ILA  A", new String(' ', 37), new String(' ', 32), new String(' ', 13));
            foreach (var usr in users.CollectionItem)
            {
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{5}{7}{8}{9}",
                    usr.User.Key.Value.ToString("B"),
                    new String(' ', 2),
                    usr.UserName.Length > 38 ? usr.UserName.Substring(0, 38) : usr.UserName,
                    new String(' ', usr.UserName.Length > 38 ? 2 : 40 - usr.UserName.Length),
                    usr.User.LastLoginTime.HasValue ? usr.User.LastLoginTime?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss") : new String(' ', 19),
                    "  ",
                    usr.Lockout.HasValue ? usr.User.Lockout?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss") : new string(' ', 19),
                    usr.User.InvalidLoginAttempts,
                    new String(' ', 4 - usr.User.InvalidLoginAttempts.ToString().Length),
                    usr.User.ObsoletionTime.HasValue ? "  " : " *"
                    );
            }
        }
        #endregion

        #region Change Roles

        internal class ChangeRoleParms : GenericUserParms
        {
            /// <summary>
            /// Gets or sets roles
            /// </summary>
            [Description("The roles to assign to the user")]
            [Parameter("r")]
            public StringCollection Roles { get; set; }
        }

        /// <summary>
        /// Gets or sets roles
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        [AdminCommand("chrole", "Change roles for a user")]
        [Description("This command is used to assign roles for the specified user to the specified roles. Note that the role list provided replaces the current role list of the user")]
        internal static void ChangeRoles(ChangeRoleParms parms)
        {
            var user = m_client.GetUsers(o => o.UserName == parms.UserName).CollectionItem.FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException($"User {parms.UserName} not found");

            var roles = new List<SecurityRoleInfo>();
            if (parms.Roles?.Count > 0)
                roles = parms.Roles.OfType<String>().Select(o => m_client.GetRoles(r => r.Name == o).CollectionItem.FirstOrDefault()).ToList();

            user.Roles = roles;
            m_client.UpdateUser(user.UserId.Value, user);

        }
        #endregion

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        internal class UserPasswordParms : GenericUserParms
        {
            /// <summary>
            /// Gets or sets the password
            /// </summary>
            [Description("The password to set on the user")]
            [Parameter("p")]
            public String Password { get; set; }
        }

        /// <summary>
        /// Set user password
        /// </summary>
        [AdminCommand("passwd", "Changes a users password")]
        [Description("This command will change the specified user's password the specified password. The server will reject this command if the password does not meet complixity requirements")]
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ChangePassword)]
        internal static void SetPassword(UserPasswordParms parms)
        {
            var user = m_client.GetUsers(o => o.UserName == parms.UserName).CollectionItem.FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException($"User {parms.UserName} not found");

            user.Password = parms.Password;
            
            m_client.UpdateUser(user.UserId.Value, user);
        }

        /// <summary>
        /// User information
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        [AdminCommand("userinfo", "Displays detailed information about the user")]
        [Description("This command will display detailed information about the specified security user account. It will show groups, status, and effective policies")]
        internal static void UserInfo(GenericUserParms parms)
        {

            var user = m_client.GetUsers(o => o.UserName == parms.UserName).CollectionItem.FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException($"User {parms.UserName} not found");

            Console.WriteLine("User: {0}", user.UserName);
            Console.WriteLine("\tSID: {0}", user.UserId);
            Console.WriteLine("\tEmail: {0}", user.Email);
            Console.WriteLine("\tPhone: {0}", user.User.PhoneNumber);
            Console.WriteLine("\tInvalid Logins: {0}", user.User.InvalidLoginAttempts);
            Console.WriteLine("\tLockout: {0}", user.User.Lockout);
            Console.WriteLine("\tLast Login: {0}", user.User.LastLoginTime);
            Console.WriteLine("\tCreated: {0} ({1})", user.User.CreationTime, m_client.GetUser(user.User.CreatedByKey.ToString()).UserName);
            if (user.User.UpdatedTime.HasValue)
                Console.WriteLine("\tLast Updated: {0} ({1})", user.User.UpdatedTime, m_client.GetUser(user.User.UpdatedByKey.ToString()).UserName);
            if (user.User.ObsoletionTime.HasValue)
                Console.WriteLine("\tDeActivated: {0} ({1})", user.User.ObsoletionTime, m_client.GetUser(user.User.ObsoletedByKey.ToString()).UserName);
            Console.WriteLine("\tGroups: {0}", String.Join(";", user.Roles.Select(o => o.Name)));

            List<SecurityPolicyInfo> policies = m_client.GetPolicies(o => o.ObsoletionTime == null).CollectionItem.OrderBy(o=>o.Oid).ToList();
            policies.ForEach(o => o.Grant = (PolicyGrantType)10);
            foreach(var rol in user.Roles)
                foreach(var pol in m_client.GetRole(rol.Id.ToString()).Policies)
                {
                    var existing = policies.FirstOrDefault(o => o.Oid == pol.Oid);
                    if (pol.Grant < existing.Grant)
                        existing.Grant = pol.Grant;
                }

            Console.WriteLine("\tEffective Policies:");
            foreach(var itm in policies)
            {
                Console.Write("\t\t{0} : ", itm.Name);
                if (itm.Grant == (PolicyGrantType)10) // Lookup parent
                {
                    var parent = policies.LastOrDefault(o => itm.Oid.StartsWith(o.Oid + ".") && itm.Oid != o.Oid);
                    if (parent != null && parent.Grant <= PolicyGrantType.Grant)
                        Console.WriteLine("{0} (inherited from {1})", parent.Grant, parent.Name);
                    else
                        Console.WriteLine("Deny (automatic)");
                }
                else
                    Console.WriteLine("{0} (explicit)", itm.Grant);
            }

        }

    }
}
