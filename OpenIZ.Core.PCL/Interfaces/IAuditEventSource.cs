using System;
using System.Collections.Generic;

namespace OpenIZ.Core.Interfaces
{

    /// <summary>
    /// Event arguments for data disclosure
    /// </summary>
    public class AuditDataEventArgs : EventArgs
    {

        /// <summary>
        /// Objects which were impacted
        /// </summary>
        public IEnumerable<Object> Objects { get; private set; }

        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Creates a new data event args
        /// </summary>
        public AuditDataEventArgs(IEnumerable<object> objects)
        {
            this.Success = true;
            this.Objects = objects;
        }

        /// <summary>
        /// Creates a new data event args
        /// </summary>
        public AuditDataEventArgs(params object[] objects)
        {
            this.Objects = objects;
        }
    }

    /// <summary>
    /// Audit the fact that data was dislcosed
    /// </summary>
    public class AuditDataDisclosureEventArgs : AuditDataEventArgs
    {

        /// <summary>
        /// Gets the query that caused the data to be disclosed
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// Audit disclosure 
        /// </summary>
        public AuditDataDisclosureEventArgs(string query, IEnumerable<object> objects) : base(objects)
        {
            this.Query = query;
        }
    }

    /// <summary>
    /// Represents a service which creates data event related audits
    /// </summary>
    public interface IAuditEventSource
    {

        /// <summary>
        /// Indicates that data was imported
        /// </summary>
        event EventHandler<AuditDataEventArgs> DataCreated;

        /// <summary>
        /// Data was updated
        /// </summary>
        event EventHandler<AuditDataEventArgs> DataUpdated;

        /// <summary>
        /// Data was obsoleted
        /// </summary>
        event EventHandler<AuditDataEventArgs> DataObsoleted;

        /// <summary>
        /// Indicates that data was disclosed
        /// </summary>
        event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;
    }
}