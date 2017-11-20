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
 * Date: 2017-4-22
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Core.Model.RISI;
using OpenIZ.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.RISI.Wcf
{
	/// <summary>
	/// Represents the RISI behavior implementation
	/// </summary>
	public partial class RisiBehavior
	{
		/// <summary>
		/// Create datamart
		/// </summary>
		public DatamartDefinition CreateDatamart(DatamartDefinition definition)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			return adhocService.CreateDatamart(definition.Name, definition.Schema);
		}

		/// <summary>
		/// Create stored query
		/// </summary>
		public DatamartStoredQuery CreateStoredQuery(string datamartId, DatamartStoredQuery queryDefinition)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			return adhocService.CreateStoredQuery(Guid.Parse(datamartId), queryDefinition);
		}

		/// <summary>
		/// Create warehouse object
		/// </summary>
		public DataWarehouseObject CreateWarehouseObject(string datamartId, DataWarehouseObject obj)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			adhocService.Add(Guid.Parse(datamartId), obj.ToExpando());

			return obj;
		}

		/// <summary>
		/// Delete a datamart
		/// </summary>
		public void DeleteDatamart(string id)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			adhocService.DeleteDatamart(Guid.Parse(id));
		}

		/// <summary>
		/// Execute an ad-hoc query
		/// </summary>
		public RisiCollection<DataWarehouseObject> ExecuteAdhocQuery(string datamartId)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			return new RisiCollection<DataWarehouseObject>(adhocService.AdhocQuery(Guid.Parse(datamartId), WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.ToQuery()).Select(o => new DataWarehouseObject(o)));
		}

		/// <summary>
		/// Execute a stored query
		/// </summary>
		public RisiCollection<DataWarehouseObject> ExecuteStoredQuery(string datamartId, string queryId)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			return new RisiCollection<DataWarehouseObject>(adhocService.StoredQuery(Guid.Parse(datamartId), queryId, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.ToQuery()).Select(o => new DataWarehouseObject(o)));
		}

		/// <summary>
		/// Get a particular datamart
		/// </summary>
		public DatamartDefinition GetDatamart(string id)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			var retVal = adhocService.GetDatamart(Guid.Parse(id));
			if (retVal == null)
				throw new FileNotFoundException(id);
			return retVal;
		}

		/// <summary>
		/// Get all datamarts
		/// </summary>
		public RisiCollection<DatamartDefinition> GetDatamarts()
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			return new RisiCollection<DatamartDefinition>(adhocService.GetDatamarts());
		}

		/// <summary>
		/// Get stored queries for the specified datamart
		/// </summary>
		public RisiCollection<DatamartStoredQuery> GetStoredQueries(string datamartId)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			var dm = adhocService.GetDatamart(Guid.Parse(datamartId));
			if (dm == null)
				throw new FileNotFoundException(datamartId);

			return new RisiCollection<DatamartStoredQuery>(dm.Schema.Queries);
		}

		/// <summary>
		/// Get warehouse object
		/// </summary>
		public DataWarehouseObject GetWarehouseObject(string datamartId, string objectId)
		{
			var adhocService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
			if (adhocService == null)
				throw new InvalidOperationException("Cannot find the adhoc data warehouse service");

			var retVal = adhocService.Get(Guid.Parse(datamartId), Guid.Parse(objectId));
			if (retVal == null)
				throw new FileNotFoundException(objectId);
			return retVal;
		}
	}
}