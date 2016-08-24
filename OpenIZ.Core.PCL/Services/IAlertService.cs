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
    /// Represents an alerting service.
    /// </summary>
    public interface IAlertService
    {
		/// <summary>
		/// Fired when an alert was raised and is being processed.
		/// </summary>
		event EventHandler<AlertEventArgs> Received;

		/// <summary>
		/// Fired when an alert is received.
		/// </summary>
		event EventHandler<AlertEventArgs> Committed;

		/// <summary>
		/// Broadcasts an alert.
		/// </summary>
		/// <param name="msg">The alert message to be broadcast.</param>
		void BroadcastAlert(AlertMessage msg);

		/// <summary>
		/// Searches for alerts.
		/// </summary>
		/// <param name="predicate">The predicate to use to search for alerts.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The count of the search results.</param>
		/// <returns>Returns a list of alerts.</returns>
		List<AlertMessage> FindAlerts(Expression<Func<AlertMessage, bool>> predicate, int offset, int? count);

		/// <summary>
		/// Gets an alert.
		/// </summary>
		/// <param name="id">The id of the alert to be retrieved.</param>
		/// <returns>Returns an alert.</returns>
		AlertMessage GetAlert(Guid id);

		/// <summary>
		/// Saves an alert.
		/// </summary>
		/// <param name="msg">The alert message to be saved.</param>
		void SaveAlert(AlertMessage msg);
    }
}
