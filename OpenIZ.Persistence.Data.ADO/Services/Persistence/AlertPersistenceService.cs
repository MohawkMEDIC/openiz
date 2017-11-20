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
 * Date: 2017-1-21
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Alerts;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
	/// <summary>
	/// Represents an alert persistence service.
	/// </summary>
	public class AlertPersistenceService : BaseDataPersistenceService<AlertMessage, DbAlertMessage>
	{
		/// <summary>
		/// Converts a <see cref="AlertMessage"/> instance to an <see cref="DbAlertMessage"/> instance.
		/// </summary>
		/// <param name="modelInstance">The alert message instance.</param>
		/// <param name="context">The data context.</param>
		/// <param name="princpal">The authentication context.</param>
		/// <returns>Returns the converted instance.</returns>
		public override object FromModelInstance(AlertMessage modelInstance, DataContext context, IPrincipal princpal)
		{
			var alert = base.FromModelInstance(modelInstance, context, princpal) as DbAlertMessage;

			alert.Flags = (int)modelInstance.Flags;

			return alert;
		}

		/// <summary>
		/// Inserts an alert.
		/// </summary>
		/// <param name="context">The data context.</param>
		/// <param name="data">The alert to insert.</param>
		/// <param name="principal">The authentication context.</param>
		/// <returns>Returns the inserted alert.</returns>
		public override AlertMessage InsertInternal(DataContext context, AlertMessage data, IPrincipal principal)
		{
			var alert = base.InsertInternal(context, data, principal);

			foreach (var securityUser in alert.RcptTo)
			{
				context.Insert(new DbAlertRcptTo(data.Key.Value, securityUser.Key.Value));
			}

			return alert;
		}

		/// <summary>
		/// Converts a <see cref="DbAlertMessage"/> instance to a <see cref="AlertMessage"/> instance.
		/// </summary>
		/// <param name="dataInstance">The db alert message instance.</param>
		/// <param name="context">The data context.</param>
		/// <param name="principal">The authentication context.</param>
		/// <returns>Returns the converted instance.</returns>
		public override AlertMessage ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
		{
			var modelInstance = base.ToModelInstance(dataInstance, context, principal);

			modelInstance.Flags = (AlertMessageFlags)(dataInstance as DbAlertMessage).Flags;

			return base.ToModelInstance(dataInstance, context, principal);
		}
	}
}
