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
using MARC.Everest.DataTypes;
using NHapi.Base.Model;
using NHapi.Model.V231.Message;
using NHapi.Model.V231.Segment;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.HL7.Configuration;
using OpenIZ.Messaging.HL7.Queue;
using System;
using System.Diagnostics;

namespace OpenIZ.Messaging.HL7.Notifier
{
	/// <summary>
	/// Represents a patient identity cross reference manager notifier.
	/// </summary>
	public class PAT_ID_X_REF_MGR : NotifierBase, INotifier
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PAT_ID_X_REF_MGR"/> class.
		/// </summary>
		public PAT_ID_X_REF_MGR() : base()
		{
		}

		/// <summary>
		/// Gets or sets the target configuration of the notifier.
		/// </summary>
		public TargetConfiguration TargetConfiguration { get; set; }

		/// <summary>
		/// Notifies a remote system.
		/// </summary>
		/// <param name="workItem">The work item of the notification.</param>
		public void Notify<T>(NotificationQueueWorkItem<T> workItem) where T : IdentifiedData
		{
			IMessage notificationMessage = null;

			var patient = workItem.Event as Patient;

			MSH msh = null;
			PID pid = null;
			EVN evn = null;
			PV1 pv1 = null;

			switch (workItem.ActionType)
			{
				case ActionType.Create:
				case ActionType.DuplicatesResolved:
				case ActionType.Update:
					{
						tracer.TraceEvent(TraceEventType.Information, 0, "Received update notification");

						ADT_A05 message = new ADT_A05();

						msh = message.MSH;
						msh.MessageType.TriggerEvent.Value = "A31";

						pid = message.PID;

						evn = message.EVN;
						evn.EventTypeCode.Value = "A31";

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

			pv1.PatientClass.Value = "N";

			var queueItem = new MessageQueueWorkItem(notificationMessage, this.TargetConfiguration);

			if (!queueItem.TrySend())
			{
				tracer.TraceEvent(TraceEventType.Warning, 0, "Unable to send message to remote endpoint: {0}", this.TargetConfiguration.ConnectionString);
				Hl7MessageQueue.Current.Enqueue(queueItem);
			}
		}
	}
}