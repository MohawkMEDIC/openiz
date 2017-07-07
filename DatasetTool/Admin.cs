/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-6-23
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OizDevTool
{
    /// <summary>
    /// Represents administrative functions
    /// </summary>
    [Description("Administrative functions for creating users, groups, etc.")]
    public static class Admin
    {

        /// <summary>
        /// Administrative shell
        /// </summary>
        public class AdminShell : InteractiveBase
        {


            /// <summary>
            /// Add a user
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            [Command("lock", "Locks the specified user")]
            void LockUser(string userName)
            {
                ApplicationContext.Current.GetService<IIdentityProviderService>().SetLockout(userName, true, AuthenticationContext.SystemPrincipal);
            }

            /// <summary>
            /// Add a user
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            [Command("unlock", "Unlocks the specified user")]
            void UnlockUser(string userName)
            {
                ApplicationContext.Current.GetService<IIdentityProviderService>().SetLockout(userName, false, AuthenticationContext.SystemPrincipal);
            }

            /// <summary>
            /// Add a user
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            [Command("passwd", "Sets a user's password")]
            void SetPassword(string userName, string password)
            {
                ApplicationContext.Current.GetService<IIdentityProviderService>().ChangePassword(userName, password, AuthenticationContext.SystemPrincipal);
            }

            /// <summary>
            /// Add a user
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            [Command("useradd", "Adds a user to OpenIZ")]
            void UserAdd(string userName, string password)
            {
                ApplicationContext.Current.GetService<IIdentityProviderService>().CreateIdentity(userName, password, AuthenticationContext.SystemPrincipal);
            }
        }

        /// <summary>
        /// Business rule
        /// </summary>
        [Description("Drops into the administrative shell")]
        [Interactive]
        public static void Shell(String[] args)
        {
            AdminShell shell = new AdminShell();
            shell.Exec();
        }

    }
}
