using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Core.Model.RISI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.RISI.Wcf
{
    /// <summary>
    /// RISI contract members for the data-warehouse
    /// </summary>
    [ServiceKnownType(typeof(RisiCollection<DatamartDefinition>))]
    [ServiceKnownType(typeof(RisiCollection<DatamartStoredQuery>))]
    [ServiceKnownType(typeof(RisiCollection<DataWarehouseObject>))]
    [ServiceKnownType(typeof(DatamartDefinition))]
    [ServiceKnownType(typeof(DatamartStoredQuery))]
    [ServiceKnownType(typeof(DataWarehouseObject))]
    public partial interface IRisiContract
    {

        /// <summary>
        /// Gets a list of all datamarts from the warehouse
        /// </summary>
        [WebGet(UriTemplate = "/datamart")]
        RisiCollection<DatamartDefinition> GetDatamarts();

        /// <summary>
        /// Create a datamart
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/datamart")]
        DatamartDefinition CreateDatamart(DatamartDefinition definition);

        /// <summary>
        /// Delete data mart
        /// </summary>
        [WebInvoke(Method = "DELETE", UriTemplate = "/datamart/{id}")]
        void DeleteDatamart(String id);

        /// <summary>
        /// Gets a specified datamart
        /// </summary>
        [WebGet(UriTemplate = "/datamart/{id}")]
        DatamartDefinition GetDatamart(String id);

        /// <summary>
        /// Get stored queries
        /// </summary>
        [WebGet(UriTemplate = "/datamart/{datamartId}/query")]
        RisiCollection<DatamartStoredQuery> GetStoredQueries(String datamartId);

        /// <summary>
        /// Create a stored query
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/datamart/{datamartId}/query")]
        DatamartStoredQuery CreateStoredQuery(String datamartId, DatamartStoredQuery queryDefinition);

        /// <summary>
        /// Executes a stored query
        /// </summary>
        [WebGet(UriTemplate = "/datamart/{datamartId}/query/{queryId}")]
        RisiCollection<DataWarehouseObject> ExecuteStoredQuery(String datamartId, String queryId);

        /// <summary>
        /// Create warehouse object
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/datamart/{datamartId}/data")]
        DataWarehouseObject CreateWarehouseObject(String datamartId, DataWarehouseObject obj);

        /// <summary>
        /// Execute adhoc query
        /// </summary>
        [WebGet(UriTemplate = "/datamart/{datamartId}/data")]
        RisiCollection<DataWarehouseObject> ExecuteAdhocQuery(String datamartId);

        /// <summary>
        /// Get warehouse object
        /// </summary>
        [WebGet(UriTemplate = "/datamart/{datamartId}/data/{objectId}")]
        DataWarehouseObject GetWarehouseObject(String datamartId, String objectId);

    }
}
