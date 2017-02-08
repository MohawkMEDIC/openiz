using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenIZ.OrmLite.Providers;
using OpenIZ.Core.Diagnostics;
using System.Threading;

namespace OpenIZ.OrmLite
{
    /// <summary>
    /// Represents a data context
    /// </summary>
    public partial class DataContext : IDisposable
    {

        /// <summary>
        /// Context lock
        /// </summary>
        public class ContextLock : IDisposable
        {
            // Lock object
            private object m_lockObject = null;

            /// <summary>
            /// Locks the lock object
            /// </summary>
            /// <param name="lockObject"></param>
            public ContextLock(Object lockObject)
            {
                this.m_lockObject = lockObject;
                bool lockTaken = false;
                Monitor.Enter(this.m_lockObject, ref lockTaken);
            }

            /// <summary>
            /// Dispose of the lock
            /// </summary>
            public void Dispose()
            {
                if (Monitor.IsEntered(this.m_lockObject))
                    Monitor.Exit(this.m_lockObject);
            }
        }

        // the connection
        private IDbConnection m_connection;

        // the transaction
        private IDbTransaction m_transaction;

        // The provider
        private IDbProvider m_provider;

        // Data dictionary
        private Dictionary<String, Object> m_dataDictionary = new Dictionary<string, object>();

        // Trace source
        private Tracer m_tracer = Tracer.GetTracer(typeof(DataContext));

        /// <summary>
        /// Connection
        /// </summary>
        public IDbConnection Connection { get { return this.m_connection; } }

        /// <summary>
        /// Data dictionary
        /// </summary>
        public IDictionary<String, Object> Data { get { return this.m_dataDictionary; } }

        /// <summary>
        /// Current Transaction
        /// </summary>
        public IDbTransaction Transaction { get { return this.m_transaction; } }

        /// <summary>
        /// Gets a lock for the specific connection
        /// </summary>
        public ContextLock Lock()
        {
            return new ContextLock(this.m_provider.Lock(this.m_connection));
        }

        /// <summary>
        /// Creates a new data context
        /// </summary>
        public DataContext(IDbProvider provider, IDbConnection connection)
        {
            this.m_provider = provider;
            this.m_connection = connection;
        }

        /// <summary>
        /// Creates a new data context
        /// </summary>
        public DataContext(IDbProvider provider, IDbConnection connection, IDbTransaction tx) : this(provider, connection)
        {
            this.m_transaction = tx;
        }

        /// <summary>
        /// Begin a transaction
        /// </summary>
        public IDbTransaction BeginTransaction()
        {
            if (this.m_transaction == null)
                this.m_transaction = this.m_connection.BeginTransaction();
            return this.m_transaction;
        }

        /// <summary>
        /// Open the connection to the database
        /// </summary>
        public void Open()
        {
            if (this.m_connection.State == ConnectionState.Closed)
                this.m_connection.Open();
            else if (this.m_connection.State == ConnectionState.Broken)
            {
                this.m_connection.Close();
                this.m_connection.Open();
            }
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            this.m_transaction?.Dispose();
            this.m_connection?.Dispose();
        }

    }
}
