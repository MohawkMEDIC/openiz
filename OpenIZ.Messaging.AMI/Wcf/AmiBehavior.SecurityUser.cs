/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: khannan
 * Date: 2016-11-22
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using MARC.Util.CertificateTools;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.AMI.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Core.Security.Attribute;
using System.Security.Permissions;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZ.Core.Security.Claims;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	public partial class AmiBehavior
	{
		/// <summary>
		/// Creates a security user.
		/// </summary>
		/// <param name="user">The security user to be created.</param>
		/// <returns>Returns the newly created security user.</returns>
		public SecurityUserInfo CreateUser(SecurityUserInfo user)
		{
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			var roleProviderService = ApplicationContext.Current.GetService<IRoleProviderService>();

			var userToCreate = new Core.Model.Security.SecurityUser()
			{
				UserName = user.UserName,
				Email = user.Email,
			};

			if (user.User == null)
			{
				userToCreate.UserClass = UserClassKeys.HumanUser;
			}
			else
			{
				userToCreate.UserClass = user.User.UserClass == Guid.Empty ? UserClassKeys.HumanUser : user.User.UserClass;
			}

			if (user.Lockout.HasValue && user.Lockout.Value)
			{
				userToCreate.Lockout = DateTime.MaxValue;
			}

			var securityUser = userRepository.CreateUser(userToCreate, user.Password);

			if (user.Roles != null)
				roleProviderService.AddUsersToRoles(new String[] { user.UserName }, user.Roles.Select(o => o.Name).ToArray(), AuthenticationContext.Current.Principal);

			return new SecurityUserInfo(securityUser);
		}

		/// <summary>
		/// Deletes a security user.
		/// </summary>
		/// <param name="rawUserId">The id of the user to be deleted.</param>
		/// <returns>Returns the deleted user.</returns>
		public SecurityUserInfo DeleteUser(string rawUserId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(rawUserId, out key))
			{
				throw new ArgumentException($"{nameof(rawUserId)} must be a valid GUID");
			}

			var securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			return new SecurityUserInfo(securityRepositoryService.ObsoleteUser(key));
		}

		/// <summary>
		/// Gets a specific security user.
		/// </summary>
		/// <param name="userId">The id of the security user to be retrieved.</param>
		/// <returns>Returns the security user.</returns>
		public SecurityUserInfo GetUser(string rawUserId)
		{
			Guid userId = Guid.Empty;
			if (!Guid.TryParse(rawUserId, out userId))
				throw new ArgumentException(nameof(rawUserId));
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new SecurityUserInfo(userRepository.GetUser(userId));
		}

		/// <summary>
		/// Gets a list of security users.
		/// </summary>
		/// <returns>Returns a list of security users.</returns>
		public AmiCollection<SecurityUserInfo> GetUsers()
		{
			var expression = QueryExpressionParser.BuildLinqExpression<SecurityUser>(this.CreateQuery(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters));
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new AmiCollection<SecurityUserInfo>() { CollectionItem = userRepository.FindUsers(expression).Select(o => new SecurityUserInfo(o)).ToList() };
		}

		/// <summary>
		/// Updates a security user.
		/// </summary>
		/// <param name="userId">The id of the security user to be updated.</param>
		/// <param name="info">The security user containing the updated information.</param>
		/// <returns>Returns the updated security user.</returns>
		public SecurityUserInfo UpdateUser(string rawUserId, SecurityUserInfo info)
		{
			Guid userId = Guid.Parse(rawUserId);
			// First change password if needed
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			var idpService = ApplicationContext.Current.GetService<IIdentityProviderService>();
			if (!String.IsNullOrEmpty(info.Password))
			{
				var user = userRepository.ChangePassword(userId, info.Password);
				idpService.RemoveClaim(user.UserName, OpenIzClaimTypes.OpenIZPasswordlessAuth);
			}

			if (info.Email != null)
			{
				SecurityUserInfo userInfo = new SecurityUserInfo(userRepository.SaveUser(new Core.Model.Security.SecurityUser()
				{
					Key = userId,
					Email = info.Email
				}));
			}

			if (info.Lockout.HasValue)
			{
				if (info.Lockout.Value)
					userRepository.LockUser(userId);
				else
					userRepository.UnlockUser(userId);
			}

			// First, we remove the roles
			if (info.Roles != null && info.Roles.Count > 0)
			{
				var irps = ApplicationContext.Current.GetService<IRoleProviderService>();

				var roles = irps.GetAllRoles(info.UserName);

				// if the roles provided are not equal to the current roles of the user, only then change the roles of the user
				if (roles != info.Roles.Select(r => r.Name).ToArray())
				{
					irps.RemoveUsersFromRoles(new String[] { info.UserName }, info.Roles.Select(o => o.Name).ToArray(), AuthenticationContext.Current.Principal);
					irps.AddUsersToRoles(new String[] { info.UserName }, info.Roles.Select(o => o.Name).ToArray(), AuthenticationContext.Current.Principal);
				}
			}

			return info;
		}
	}
}
