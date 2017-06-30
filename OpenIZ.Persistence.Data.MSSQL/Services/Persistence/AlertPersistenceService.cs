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
 * Date: 2016-9-16
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Persistence.Data.MSSQL.Data;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
	public class AlertPersistenceService : BaseDataPersistenceService<Core.Alert.Alerting.AlertMessage, Data.AlertMessage>
	{
		public override object FromModelInstance(Core.Alert.Alerting.AlertMessage modelInstance, ModelDataContext context, IPrincipal princpal)
		{
			var alert = base.FromModelInstance(modelInstance, context, princpal) as Data.AlertMessage;

			alert.Flags = (int?)modelInstance.Flags;

			return alert;
		}

		public override Core.Alert.Alerting.AlertMessage ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
		{
			var modelInstance = base.ToModelInstance(dataInstance, context, principal);

			modelInstance.Flags = (AlertMessageFlags)(dataInstance as Data.AlertMessage).Flags;
			return base.ToModelInstance(dataInstance, context, principal);
		}
	}
}
