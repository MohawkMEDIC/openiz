using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Data;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Diagnostics;
using OpenIZ.Core.Data.Warehouse;

namespace OpenIZ.Warehouse.ADO
{
    /// <summary>
    /// Represents a simple SQLite ad-hoc data warehouse
    /// </summary>
    public class ADODataWarehouse : IAdHocDatawarehouseService, IDaemonService
    {

        // Lock 
        private Object m_lock = new object();

        // Disposed
        private bool m_disposed = false;

        // Tracer
        private TraceSource m_tracer = new TraceSource(DataWarehouseConstants.TraceSourceName);

        // Connection to the data-warehouse
        private IDbConnection m_connection;

        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;

        /// <summary>
        /// Gets the name of the data provider so callers can determine how to create stored queries
        /// </summary>
        public string DataProvider
        {
            get
            {

                return "ado.net";
            }
        }

        /// <summary>
        /// True if this is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_connection != null;
            }
        }
        

        /// <summary>
        /// Constructs the SQLite data warehouse file
        /// </summary>
        public ADODataWarehouse()
        {
        }

        public DatamartDefinition CreateDatamart(string name, object schema)
        {
            throw new NotImplementedException();
        }

        public List<DatamartDefinition> GetDatamarts()
        {
            throw new NotImplementedException();
        }

        public DatamartDefinition GetDatamart(string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteDatamart(Guid datamartId)
        {
            throw new NotImplementedException();
        }

        public dynamic Get(Guid datamartId, Guid tupleId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> AdhocQuery(Guid datamartId, dynamic filterParameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> AdhocQuery(Guid datamartId, dynamic filterParameters, int offset, int count, out int totalResults)
        {
            throw new NotImplementedException();
        }

        public void CreateStoredQuery(Guid datamartId, object queryDefinition)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> StoredQuery(Guid datamartId, string queryId, dynamic queryParameters)
        {
            throw new NotImplementedException();
        }

        public Guid Add(Guid datamartId, dynamic obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid datamartId, dynamic matchingQuery)
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
