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
 * Date: 2016-11-23
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
		/// Creates an alert.
		/// </summary>
		/// <param name="alertMessageInfo">The alert message to be created.</param>
		/// <returns>Returns the created alert.</returns>
		public AlertMessageInfo CreateAlert(AlertMessageInfo alertMessageInfo)
		{
			var alertRepositoryService = ApplicationContext.Current.GetService<IAlertRepositoryService>();

			if (alertRepositoryService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IAlertRepositoryService)));
			}

			var createdAlert = alertRepositoryService.Insert(alertMessageInfo.AlertMessage);

			return new AlertMessageInfo(createdAlert);
		}

		/// <summary>
		/// Gets a specific alert.
		/// </summary>
		/// <param name="alertId">The id of the alert to retrieve.</param>
		/// <returns>Returns the alert.</returns>
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
		public AmiCollection<AlertMessageInfo> GetAlerts()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} cannot be empty", nameof(parameters)));
			}

			var expression = QueryExpressionParser.BuildLinqExpression<AlertMessage>(this.CreateQuery(parameters));

			var alertRepository = ApplicationContext.Current.GetService<IAlertRepositoryService>();

			if (alertRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IAlertRepositoryService)));
			}

			AmiCollection<AlertMessageInfo> alerts = new AmiCollection<AlertMessageInfo>();

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
		public AlertMessageInfo UpdateAlert(string alertId, AlertMessageInfo alert)
		{
			Guid key = Guid.Empty;

			if (!Guid.TryParse(alertId, out key))
			{
				throw new ArgumentException(string.Format("{0} must be a valid GUID", nameof(alertId)));
			}

			var alertRepository = ApplicationContext.Current.GetService<IAlertRepositoryService>();

			if (alertRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IAlertRepositoryService)));
			}

			var updatedAlert = alertRepository.Save(alert.AlertMessage);

			return new AlertMessageInfo(updatedAlert);
		}
	}
}
