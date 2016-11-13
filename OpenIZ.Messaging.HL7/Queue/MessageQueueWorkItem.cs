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
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Messaging.HL7.Configuration;

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
		/// The internal reference to the sync lock object.
		/// </summary>
		private object syncLock = new object();

		/// <summary>
		/// The internal reference to the target configuration.
		/// </summary>
		private readonly TargetConfiguration targetConfiguration;

		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageQueueWorkItem"/> class
		/// with a specific message.
		/// </summary>
		/// <param name="message">The message to send.</param>
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
		/// Gets or sets the message text.
		/// </summary>
		public string MessageText { get; set; }

		/// <summary>
		/// Tries to send a message to an endpoint.
		/// </summary>
		/// <returns>Returns true if the message was sent successfully.</returns>
		public bool TrySend()
		{
			try
			{
				// HACK: hard code the endpoint address for now, until configuration is setup
				var sender = new MllpMessageSender(new Uri(this.targetConfiguration.ConnectionString), this.targetConfiguration.LlpClientCertificate, this.targetConfiguration.TrustedIssuerCertificate);

				var response = sender.SendAndReceive(this.message) as ACK;

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

				return false;
			}
		}
	}
}
