using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Base data persistence service
    /// </summary>
    [Service(ServiceInstantiationType.Instance)] // <- Some people may use GetService<> to get this context
    public class SqlDataPersistenceContext : IDataPersistenceContext
    {
        // Disposed indicator
        private Boolean m_disposed = false;

        // Configuration
        protected SqlConfiguration m_configuration = ConfigurationManager.GetSection("openiz.persistence.data.mssql") as SqlConfiguration;

        // Connection for the service
        protected DbConnection m_connection;

        // Readonly connection
        protected DbConnection m_readOnlyConnection;

        // The transaction
        protected DbTransaction m_transaction;

        /// <summary>
        /// Open the connection to the database if it is not already open
        /// </summary>
        public void Open()
        {
            this.ThrowIfDisposed();
            if (this.m_connection == null)
                this.m_connection = new SqlConnection(this.m_configuration.ReadWriteConnectionString);
            if (this.m_readOnlyConnection == null && this.m_configuration.ReadonlyConnectionString != this.m_configuration.ReadWriteConnectionString)
                this.m_readOnlyConnection = new SqlConnection(this.m_configuration.ReadonlyConnectionString);
            else if (this.m_readOnlyConnection == null)
                this.m_readOnlyConnection = this.m_connection;

            if (this.m_connection.State == System.Data.ConnectionState.Closed)
                this.m_connection.Open();
            if (this.m_readOnlyConnection.State == System.Data.ConnectionState.Closed)
                this.m_readOnlyConnection.Open();
        }

        /// <summary>
        /// Create data context
        /// </summary>
        public ModelDataContext CreateDataContext()
        {
            var retVal = new ModelDataContext(this.m_connection);
            if (this.m_transaction != null)
                retVal.Transaction = this.m_transaction;
            return retVal;
        }

        /// <summary>
        /// Create data context
        /// </summary>
        public ModelDataContext CreateReadonlyContext()
        {
            var retVal = new ModelDataContext(this.m_readOnlyConnection);
            if (this.m_transaction != null)
                retVal.Transaction = this.m_transaction;
            return retVal;
        }
        /// <summary>
        /// Close all connections
        /// </summary>
        public void Close()
        {
            this.ThrowIfDisposed();
            this.m_connection?.Close();
            this.m_readOnlyConnection?.Close();
        }

        /// <summary>
        /// Commit any transactions
        /// </summary>
        public void Commit()
        {
            this.ThrowIfDisposed();
            this.m_transaction?.Commit();
            this.m_transaction?.Dispose();
            this.m_transaction = null;
        }

        /// <summary>
        /// Rollback the transaction
        /// </summary>
        public void Rollback()
        {
            this.ThrowIfDisposed();
            this.m_transaction?.Rollback();
            this.m_transaction?.Dispose();
            this.m_transaction = null;

        }

        /// <summary>
        /// Start a transaction
        /// </summary>
        public bool BeginTransaction()
        {
            this.ThrowIfDisposed();
            this.Open();
            if (this.m_transaction == null)
            {
                this.m_transaction = this.m_connection?.BeginTransaction();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            if (!this.m_disposed)
            {
                this.Rollback();
                this.m_transaction?.Dispose();
                this.m_connection?.Dispose();
                this.m_readOnlyConnection?.Dispose();
                this.m_disposed = true;
            }
        }

        /// <summary>
        /// Throw an exception if the object is disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.m_disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }
    }

}
