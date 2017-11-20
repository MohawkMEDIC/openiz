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
 * Date: 2017-3-13
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Services;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Configuration;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;

namespace OpenIZ.Persistence.Data.ADO.Services
{
	/// <summary>
	/// Represents a device identity provider.
	/// </summary>
	public class AdoDeviceIdentityProvider : IDeviceIdentityProviderService
	{
		/// <summary>
		/// The trace source.
		/// </summary>
		private readonly TraceSource traceSource = new TraceSource(AdoDataConstants.IdentityTraceSourceName);

		/// <summary>
		/// The configuration.
		/// </summary>
		private readonly AdoConfiguration configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(AdoDataConstants.ConfigurationSectionName) as AdoConfiguration;

		/// <summary>
		/// Fired after an authentication request has been made.
		/// </summary>
		public event EventHandler<AuthenticatedEventArgs> Authenticated;
		/// <summary>
		/// Fired prior to an authentication request being made.
		/// </summary>
		public event EventHandler<AuthenticatingEventArgs> Authenticating;

		/// <summary>
		/// Authenticates the specified device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="deviceSecret">The device secret.</param>
		/// <returns>Returns the authenticated device principal.</returns>
		public IPrincipal Authenticate(string deviceId, string deviceSecret)
		{
			using (var dataContext = this.configuration.Provider.GetWriteConnection())
			{
				try
				{
					dataContext.Open();

					var hashService = ApplicationContext.Current.GetService<IPasswordHashingService>();

					var client = dataContext.FirstOrDefault<DbSecurityDevice>("auth_dev", deviceId, hashService.EncodePassword(deviceSecret));

					if (client == null)
					{
						throw new SecurityException("Invalid device credentials");
					}

					IPrincipal devicePrincipal = new DevicePrincipal(new DeviceIdentity(client.Key, client.PublicId, true));

					new PolicyPermission(System.Security.Permissions.PermissionState.None, PermissionPolicyIdentifiers.Login, devicePrincipal).Demand();

					return devicePrincipal;
				}
				catch (Exception e)
				{
					this.traceSource.TraceEvent(TraceEventType.Error, e.HResult, "Error authenticating {0} : {1}", deviceId, e);
					throw new AuthenticationException("Error authenticating application", e);
				}
			}
		}

		/// <summary>
		/// Authenticate the device based on certificate provided
		/// </summary>
		/// <param name="deviceCertificate">The certificate of the device used to authenticate the device.</param>
		/// <returns>Returns the authenticated device principal.</returns>
		public IPrincipal Authenticate(X509Certificate2 deviceCertificate)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the identity of the device using a given device name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>Returns the identity of the device.</returns>
		public IIdentity GetIdentity(string name)
		{
			using (var dataContext = this.configuration.Provider.GetReadonlyConnection())
			{
				try
				{
					dataContext.Open();

					var client = dataContext.FirstOrDefault<DbSecurityDevice>(o => o.PublicId == name);

					return new DeviceIdentity(client.Key, client.PublicId, false);

				}
				catch (Exception e)
				{
					this.traceSource.TraceEvent(TraceEventType.Error, e.HResult, "Error getting identity data for {0} : {1}", name, e);
					throw;
				}
			}
		}
	}
}
