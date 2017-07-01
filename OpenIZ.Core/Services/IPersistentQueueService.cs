using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{

    public class PersistentQueueEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the queue
        /// </summary>
        public String QueueName { get; private set; }

        /// <summary>
        /// Get the correlation token or object that was provided by the queue
        /// </summary>
        public Object Data { get; private set; }

        public PersistentQueueEventArgs(String queueName, object data)
        {
            this.Data = data;
            this.QueueName = queueName;
        }
    }

    /// <summary>
    /// Represents a persistent queue where data can be stored and retrieved
    /// </summary>
    public interface IPersistentQueueService
    {
        /// <summary>
        /// Opens the specified queue name and enables subscriptions
        /// </summary>
        void Open(String queueName);

        /// <summary>
        /// Enqueue the specified data to the persistent queue
        /// </summary>
        void Enqueue(String queueName, Object data);

        /// <summary>
        /// Dequeues the last added item from the persistent queue
        /// </summary>
        Object Dequeue(String queueName);

        /// <summary>
        /// Fired when an item was queued to a queue
        /// </summary>
        event EventHandler<PersistentQueueEventArgs> Queued;
    }
}
