/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: khannan
 * Date: 2016-11-30
 */
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenIZ.Messaging.HL7.Queue
{
	/// <summary>
	/// Represents an HL7 message queue.
	/// </summary>
	internal class Hl7MessageQueue
	{
		/// <summary>
		/// The internal reference to the <see cref="Hl7MessageQueue"/> instance.
		/// </summary>
		private static Hl7MessageQueue instance;

		/// <summary>
		/// The internal reference to the sync lock.
		/// </summary>
		private static object syncLock = new object();

		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// The internal reference to the <see cref="Queue{T}"/> instance.
		/// </summary>
		private Queue<MessageQueueWorkItem> queue = new Queue<MessageQueueWorkItem>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Hl7MessageQueue"/> class.
		/// </summary>
		private Hl7MessageQueue()
		{
		}

		/// <summary>
		/// Gets the HL7 message queue.
		/// </summary>
		public static Hl7MessageQueue Current
		{
			get
			{
				if (instance == null)
				{
					lock (syncLock)
					{
						if (instance == null)
						{
							instance = new Hl7MessageQueue();
						}
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Dequeues a work item from the queue.
		/// </summary>
		/// <returns>Returns the dequeued work item or null if the queue is empty.</returns>
		public MessageQueueWorkItem Dequeue()
		{
			lock (syncLock)
			{
				if (this.queue.Count == 0)
				{
					return null;
				}
				else
				{
					this.tracer.TraceEvent(TraceEventType.Information, 0, "De-queuing item from: {0}", nameof(Hl7MessageQueue));
					return this.queue.Dequeue();
				}
			}
		}

		/// <summary>
		/// Enqueues a work item to the queue.
		/// </summary>
		/// <param name="workItem">The work item to add to the queue.</param>
		public void Enqueue(MessageQueueWorkItem workItem)
		{
			lock (syncLock)
			{
				this.tracer.TraceEvent(TraceEventType.Information, 0, "En-queuing item to: {0}", nameof(Hl7MessageQueue));
				this.queue.Enqueue(workItem);
			}
		}
	}
}