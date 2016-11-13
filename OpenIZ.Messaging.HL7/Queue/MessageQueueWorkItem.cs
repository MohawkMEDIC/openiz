/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-11-3
 */
using NHapi.Base.Model;
using NHapi.Model.V25.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.HL7.Queue
{
	/// <summary>
	/// Represents a message queue work item.
	/// </summary>
	internal class MessageQueueWorkItem
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageQueueWorkItem"/> class.
		/// </summary>
		public MessageQueueWorkItem()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageQueueWorkItem"/> class
		/// with a specific message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		public MessageQueueWorkItem(IMessage message)
		{
			this.Message = message;
		}

		/// <summary>
		/// Gets or sets the message text.
		/// </summary>
		public string MessageText { get; set; }

		/// <summary>
		/// Gets or sets the message to be sent.
		/// </summary>
		public IMessage Message { get; set; }

		/// <summary>
		/// Gets or sets the fail count.
		/// </summary>
		public int FailCount { get; set; }

		/// <summary>
		/// Tries to send a message to an endpoint.
		/// </summary>
		/// <returns>Returns true if the message was sent successfully.</returns>
		public bool TrySend()
		{
			try
			{
				// HACK: hard code the endpoint address for now, until configuration is setup
				MllpMessageSender sender = new MllpMessageSender(new Uri("llp://cr.marc-hi.ca:2100"));

				var response = sender.SendAndReceive(this.Message) as ACK;

				if (response == null)
				{
					this.FailCount++;
					return false;
				}

				if (response.MSA.AcknowledgmentCode.Value != "AA")
				{
					this.FailCount++;
					return false;
				}

				return true;
			}
			catch (Exception e)
			{
				this.FailCount++;
#if DEBUG
				this.tracer.TraceEvent(TraceEventType.Error, 0, e.StackTrace);
#endif
				this.tracer.TraceEvent(TraceEventType.Error, 0, e.Message);

				this.tracer.TraceEvent(TraceEventType.Error, 0, string.Format("Unable to send message to endpoint: {0}. Re-queuing message", "llp://cr.marc-hi.ca:2100"));

				return false;
			}
		}
	}
}
