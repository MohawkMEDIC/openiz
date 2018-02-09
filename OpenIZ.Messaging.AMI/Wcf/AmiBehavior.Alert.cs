/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using SwaggerWcf.Attributes;
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
		/// Creates an alert.
		/// </summary>
		/// <param name="alertMessageInfo">The alert message to be created.</param>
		/// <returns>Returns the created alert.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(409, "You are attempting to create a resource that already exists")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(201, "The object was created successfully")]
		public AlertMessageInfo CreateAlert(AlertMessageInfo alertMessageInfo)
		{
			var alertRepositoryService = ApplicationContext.Current.GetService<IAlertRepositoryService>();

			if (alertRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(IAlertRepositoryService)} not found");
			}

			var createdAlert = alertRepositoryService.Insert(alertMessageInfo.AlertMessage);

			return new AlertMessageInfo(createdAlert);
		}

		/// <summary>
		/// Gets a specific alert.
		/// </summary>
		/// <param name="alertId">The id of the alert to retrieve.</param>
		/// <returns>Returns the alert.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
		public AlertMessageInfo GetAlert(string alertId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(alertId, out key))
			{
				throw new ArgumentException($"{nameof(alertId)} must be a valid GUID");
			}

			var alertRepository = ApplicationContext.Current.GetService<IAlertRepositoryService>();

			if (alertRepository == null)
			{
				throw new InvalidOperationException($"{nameof(IAlertRepositoryService)} not found");
			}

			var alert = alertRepository.Get(key);

			return new AlertMessageInfo(alert);
		}

		/// <summary>
		/// Gets a list of alert for a specific query.
		/// </summary>
		/// <returns>Returns a list of alert which match the specific query.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
		public AmiCollection<AlertMessageInfo> GetAlerts()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException($"{nameof(parameters)} cannot be empty");
			}

			var expression = QueryExpressionParser.BuildLinqExpression<AlertMessage>(this.CreateQuery(parameters));

			var alertRepository = ApplicationContext.Current.GetService<IAlertRepositoryService>();

			if (alertRepository == null)
			{
				throw new InvalidOperationException($"{nameof(IAlertRepositoryService)} not found");
			}

			var alerts = new AmiCollection<AlertMessageInfo>();

			int totalCount = 0;

			alerts.CollectionItem = alertRepository.Find(expression, 0, null, out totalCount).Select(a => new AlertMessageInfo(a)).ToList();
			alerts.Size = totalCount;

			return alerts;
		}

		/// <summary>
		/// Updates an alert.
		/// </summary>
		/// <param name="alertId">The id of the alert to be updated.</param>
		/// <param name="alert">The alert containing the updated information.</param>
		/// <returns>Returns the updated alert.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(409, "You are attempting to perform an update on an old version of the resource, or the conditional HTTP headers don't match the current version of the resource")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The object was updated successfully")]
		public AlertMessageInfo UpdateAlert(string alertId, AlertMessageInfo alert)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(alertId, out key))
			{
				throw new ArgumentException($"{nameof(alertId)} must be a valid GUID");
			}

			var alertRepository = ApplicationContext.Current.GetService<IAlertRepositoryService>();

			if (alertRepository == null)
			{
				throw new InvalidOperationException($"{nameof(IAlertRepositoryService)} not found");
			}

			var updatedAlert = alertRepository.Save(alert.AlertMessage);

			return new AlertMessageInfo(updatedAlert);
		}
	}
}