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
using NHapi.Base.Model;
using NHapi.Model.V25.Message;
using OpenIZ.Messaging.HL7.Configuration;
using System;
using System.Diagnostics;

namespace OpenIZ.Messaging.HL7.Queue
{
	/// <summary>
	/// Represents a message queue work item.
	/// </summary>
	internal class MessageQueueWorkItem
	{
		/// <summary>
		/// The internal reference to the message.
		/// </summary>
		private readonly IMessage message;

		/// <summary>
		/// The internal reference to the target configuration.
		/// </summary>
		private readonly TargetConfiguration targetConfiguration;

		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageQueueWorkItem" /> class
		/// with a specific message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <param name="targetConfiguration">The target configuration.</param>
		public MessageQueueWorkItem(IMessage message, TargetConfiguration targetConfiguration)
		{
			this.message = message;
			this.targetConfiguration = targetConfiguration;
		}

		/// <summary>
		/// Gets the fail count.
		/// </summary>
		public int FailCount { get; private set; }

		/// <summary>
		/// Tries to send a message to an endpoint.
		/// </summary>
		/// <returns>Returns true if the message was sent successfully.</returns>
		public bool TrySend()
		{
			var status = false;

			try
			{
				var sender = new MllpMessageSender(new Uri(this.targetConfiguration.ConnectionString), this.targetConfiguration.LlpClientCertificate, this.targetConfiguration.TrustedIssuerCertificate);

				var response = sender.SendAndReceive(this.message);

				if (response is NHapi.Model.V231.Message.ACK)
				{
					status = (response as NHapi.Model.V231.Message.ACK).MSA.AcknowledgementCode.Value.ToUpper() == "AA";
				}
				else if (response is ACK)
				{
					status = (response as ACK).MSA.AcknowledgmentCode.Value.ToUpper() == "AA";
				}
				else
				{
					this.FailCount++;
				}
			}
			catch (Exception e)
			{
				this.FailCount++;
#if DEBUG
				this.tracer.TraceEvent(TraceEventType.Error, 0, e.StackTrace);
#endif
				this.tracer.TraceEvent(TraceEventType.Error, 0, e.Message);
			}

			return status;
		}
	}
}