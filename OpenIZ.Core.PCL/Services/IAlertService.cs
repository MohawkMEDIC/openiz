using OpenIZ.Core.Alerting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents an alerting service
    /// </summary>
    public interface IAlertService
    {

        /// <summary>
        /// Indicates an alert was raised and is being processed
        /// </summary>
        event EventHandler<AlertEventArgs> Received;

        /// <summary>
        /// Indicates an alert was received and 
        /// </summary>
        event EventHandler<AlertEventArgs> Committed;

        /// <summary>
        /// Broadcast (insert and raise event)
        /// </summary>
        void BroadcastAlert(AlertMessage msg);

        /// <summary>
        /// Sends an alert
        /// </summary>
        void SaveAlert(AlertMessage msg);

        /// <summary>
        /// Gets the specified alert
        /// </summary>
        AlertMessage GetAlert(Guid id);

        /// <summary>
        /// Get alerts matching the predicate
        /// </summary>
        List<AlertMessage> FindAlerts(Expression<Func<AlertMessage, bool>> predicate, int offset, int? count);
    }
}
