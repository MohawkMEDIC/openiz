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
using NHapi.Model.V231.Message;
using NHapi.Model.V231.Segment;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.HL7.Configuration;
using OpenIZ.Messaging.HL7.Queue;
using System;
using System.Diagnostics;
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
		/// Notifies a remote system.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="workItem">The work item of the notification.</param>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		public void Notify<T>(NotificationQueueWorkItem<T> workItem) where T : IdentifiedData
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

			var queueItem = new MessageQueueWorkItem(notificationMessage, this.TargetConfiguration);

			if (!queueItem.TrySend())
			{
				tracer.TraceEvent(TraceEventType.Warning, 0, "Unable to send message to remote endpoint: {0}", this.TargetConfiguration.ConnectionString);
				Hl7MessageQueue.Current.Enqueue(queueItem);
			}
		}
	}
}