using OpenIZ.OrmLite.Util.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OpenIZ.OrmLite
{
    /// <summary>
    /// Represents a data context
    /// </summary>
    public partial class DataContext : IDisposable
    {
        // the connection
        private IDbConnection m_connection;

        // the transaction
        private IDbTransaction m_transaction;

        // The provider
        private IDbProvider m_provider;

        // Trace source
        private TraceSource m_traceSource = new TraceSource(Constants.TraceSourceName);

        /// <summary>
        /// Connection
        /// </summary>
        public IDbConnection Connection { get { return this.m_connection; } }

        /// <summary>
        /// Current Transaction
        /// </summary>
        public IDbTransaction Transaction { get { return this.m_transaction; } }

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
            if(this.m_transaction == null)
                this.m_transaction = this.m_connection.BeginTransaction();
            return this.m_transaction;
        }

        /// <summary>
        /// Open the connection to the database
        /// </summary>
        public void Open()
        {
            this.m_connection.Open();
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
