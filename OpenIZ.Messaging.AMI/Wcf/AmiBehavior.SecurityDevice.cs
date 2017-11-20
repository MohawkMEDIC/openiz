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
 * User: khannan
 * Date: 2016-11-30
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Services;
using System;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	public partial class AmiBehavior
	{
		/// <summary>
		/// Creates a device in the IMS.
		/// </summary>
		/// <param name="deviceInfo">The device to be created.</param>
		/// <returns>Returns the newly created device.</returns>
		public SecurityDeviceInfo CreateDevice(SecurityDeviceInfo deviceInfo)
		{
			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			deviceInfo.Device?.Policies.AddRange(deviceInfo.Policies.Select(p => new SecurityPolicyInstance(p.Policy, p.Grant)));

			var createdDevice = securityRepository.CreateDevice(deviceInfo.Device);

			return new SecurityDeviceInfo(createdDevice);
		}

		/// <summary>
		/// Deletes a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be deleted.</param>
		/// <returns>Returns the deleted device.</returns>
		public SecurityDeviceInfo DeleteDevice(string deviceId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(deviceId, out key))
			{
				throw new ArgumentException($"{nameof(deviceId)} must be a valid GUID");
			}

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			return new SecurityDeviceInfo(securityRepository.ObsoleteDevice(key));
		}

		/// <summary>
		/// Gets a specific device.
		/// </summary>
		/// <param name="deviceId">The id of the security device to be retrieved.</param>
		/// <returns>Returns the security device.</returns>
		public SecurityDeviceInfo GetDevice(string deviceId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(deviceId, out key))
			{
				throw new ArgumentException($"{nameof(deviceId)} must be a valid GUID");
			}

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			return new SecurityDeviceInfo(securityRepository.GetDevice(key));
		}

		/// <summary>
		/// Gets a list of devices.
		/// </summary>
		/// <returns>Returns a list of devices.</returns>
		public AmiCollection<SecurityDeviceInfo> GetDevices()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException($"{nameof(parameters)} cannot be empty");
			}

			var expression = QueryExpressionParser.BuildLinqExpression<SecurityDevice>(this.CreateQuery(parameters));

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			return new AmiCollection<SecurityDeviceInfo>
			{
				CollectionItem = securityRepository.FindDevices(expression).Select(d => new SecurityDeviceInfo(d)).ToList()
			};
		}

		/// <summary>
		/// Updates a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be updated.</param>
		/// <param name="deviceInfo">The device containing the updated information.</param>
		/// <returns>Returns the updated device.</returns>
		public SecurityDeviceInfo UpdateDevice(string deviceId, SecurityDeviceInfo deviceInfo)
		{
			var id = Guid.Empty;

			if (!Guid.TryParse(deviceId, out id))
			{
				throw new ArgumentException($"{nameof(deviceId)} must be a valid GUID");
			}

			if (id != deviceInfo.Id)
			{
				throw new ArgumentException($"Unable to update device using id: {id}, and id: {deviceInfo.Id}");
			}

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			deviceInfo.Device.Policies.AddRange(deviceInfo.Policies.Select(p => new SecurityPolicyInstance(p.Policy, p.Grant)));

			var updatedDevice = securityRepository.SaveDevice(deviceInfo.Device);

			return new SecurityDeviceInfo(updatedDevice);
		}
	}
}