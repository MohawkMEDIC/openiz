/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */

using NHapi.Base.Model;
using NHapi.Model.V231.Message;
using NHapi.Model.V231.Segment;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.HL7.Configuration;
using OpenIZ.Messaging.HL7.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NHapi.Base.Parser;
using TS = MARC.Everest.DataTypes.TS;

namespace OpenIZ.Messaging.HL7.Notifier
{
	/// <summary>
	/// Represents a patient identity source notifier.
	/// </summary>
	public class PAT_IDENTITY_SRC : NotifierBase, INotifier
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PAT_IDENTITY_SRC"/> class.
		/// </summary>
		public PAT_IDENTITY_SRC() : base()
		{
		}

		/// <summary>
		/// Gets or sets the target configuration of the notifier.
		/// </summary>
		public TargetConfiguration TargetConfiguration { get; set; }

		/// <summary>
		/// Creates the message.
		/// </summary>
		/// <typeparam name="T">The type of identified data.</typeparam>
		/// <param name="workItem">The work item.</param>
		/// <returns>Returns the created message</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Invalid notification type</exception>
		public IMessage CreateMessage<T>(NotificationQueueWorkItem<T> workItem) where T : IdentifiedData
		{
			IMessage notificationMessage;

			var patient = workItem.Event as Patient;

			MSH msh;
			PID pid;
			EVN evn;
			PV1 pv1;
			MRG mrg = null;

			switch (workItem.ActionType)
			{
				case ActionType.Create:
					{
						tracer.TraceEvent(TraceEventType.Information, 0, "Received create notification");

						var message = new ADT_A01();

						msh = message.MSH;
						msh.MessageType.MessageType.Value = "ADT";
						msh.MessageType.MessageStructure.Value = "ADT_A01";
						msh.MessageType.TriggerEvent.Value = "A01";

						pid = message.PID;

						evn = message.EVN;
						evn.EventTypeCode.Value = "A01";

						pv1 = message.PV1;
						notificationMessage = message;

						break;
					}
				case ActionType.DuplicatesResolved:
					{
						tracer.TraceEvent(TraceEventType.Information, 0, "Received duplicates resolved notification");

						var message = new ADT_A39();

						msh = message.MSH;
						msh.MessageType.MessageType.Value = "ADT";
						msh.MessageType.MessageStructure.Value = "ADT_A40";
						msh.MessageType.TriggerEvent.Value = "A40";

						pid = message.GetPATIENT(0).PID;

						evn = message.EVN;
						evn.EventTypeCode.Value = "A40";

						pv1 = message.GetPATIENT(0).PV1;
						mrg = message.GetPATIENT(0).MRG;
						notificationMessage = message;

						break;
					}
				case ActionType.Update:
					{
						tracer.TraceEvent(TraceEventType.Information, 0, "Received update notification");

						var message = new ADT_A01();

						msh = message.MSH;
						msh.MessageType.MessageType.Value = "ADT";
						msh.MessageType.MessageStructure.Value = "ADT_A08";
						msh.MessageType.TriggerEvent.Value = "A08";

						pid = message.PID;

						evn = message.EVN;
						evn.EventTypeCode.Value = "A08";

						pv1 = message.PV1;
						notificationMessage = message;

						break;
					}
				default:
					throw new ArgumentOutOfRangeException($"Invalid notification type {workItem.ActionType}");
			}

			NotifierBase.UpdateMSH(msh, patient, this.TargetConfiguration);

			evn.RecordedDateTime.TimeOfAnEvent.Value = (TS)patient.CreationTime.DateTime;

			NotifierBase.UpdatePID(patient, pid, this.TargetConfiguration);

			pv1.PatientClass.Value = "I";

			// TODO: populate the merge information
			if (mrg != null)
			{
			}

			return notificationMessage;
		}

		/// <summary>
		/// Notifies a remote system.
		/// </summary>
		/// <typeparam name="T">The type of identified data.</typeparam>
		/// <param name="workItem">The work item of the notification.</param>
		public void Notify<T>(NotificationQueueWorkItem<T> workItem) where T : IdentifiedData
		{
			var notificationMessage = this.CreateMessage(workItem);

			try
			{
				// attempt to write the message we are sending to the file

				var parser = new PipeParser();
				var encodedNotificationMessage = parser.Encode(notificationMessage);
				var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hl7");

				if (!Directory.Exists(filePath))
				{
					Directory.CreateDirectory(filePath);
				}
				using (var fileStream = File.Create(filePath + "hl7_output_" + DateTime.Now.ToString("yyyyMMdd") + ".hl7"))
				{
					// Write message in ASCII encoding
					byte[] buffer = Encoding.UTF8.GetBytes(encodedNotificationMessage);
					byte[] sendBuffer = new byte[buffer.Length + 3];

					sendBuffer[0] = 0x0b;

					Array.Copy(buffer, 0, sendBuffer, 1, buffer.Length);
					Array.Copy(new byte[] { 0x1c, 0x0d }, 0, sendBuffer, sendBuffer.Length - 2, 2);

					fileStream.Write(sendBuffer, 0, sendBuffer.Length);

					// Write end message
					fileStream.Flush();
				}
			}
			catch (Exception e)
			{
				tracer.TraceEvent(TraceEventType.Error, 1911, $"Unable to write message to file: {e}");
			}

			var queueItem = new MessageQueueWorkItem(notificationMessage, this.TargetConfiguration);

			if (!queueItem.TrySend())
			{
				tracer.TraceEvent(TraceEventType.Warning, 0, "Unable to send message to remote endpoint: {0}", this.TargetConfiguration.ConnectionString);
				Hl7MessageQueue.Current.Enqueue(queueItem);
			}
		}
	}
}