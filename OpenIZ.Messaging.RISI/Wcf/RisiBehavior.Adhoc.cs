using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Core.Model.RISI;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Query;
using System.ServiceModel.Web;
using System.IO;

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

            return new RisiCollection<DataWarehouseObject>(adhocService.AdhocQuery(Guid.Parse(datamartId), WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.ToQuery()).Select(o=>new DataWarehouseObject(o)));
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
