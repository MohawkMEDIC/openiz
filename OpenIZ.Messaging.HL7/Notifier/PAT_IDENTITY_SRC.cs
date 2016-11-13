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
 * User: khannan
 * Date: 2016-11-12
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using NHapi.Base.Model;
using NHapi.Model.V25.Message;
using NHapi.Model.V25.Segment;
using OpenIZ.Core.Event;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.HL7.Configuration;

namespace OpenIZ.Messaging.HL7.Notifier
{
	/// <summary>
	/// Represents a patient identity source notifier.
	/// </summary>
	public class PAT_IDENTITY_SRC : INotifier<Patient>
	{
		/// <summary>
		/// Gets or sets the target configuration of the notifier.
		/// </summary>
		public TargetConfiguration TargetConfiguration { get; set; }

		/// <summary>
		/// Notifies a remote system.
		/// </summary>
		/// <param name="workItem">The work item of the notification.</param>
		public void Notify(NotificationQueueWorkItem<Patient> workItem)
		{
			IMessage notificationMessage = null;

			MSH msh = null;
			PID pid = null;
			EVN evn = null;
			PV1 pv1 = null;
			MRG mrg = null;

			switch (workItem.NotificationType)
			{
				case NotificationType.Create:
					{
						ADT_A01 message = new ADT_A01();

						msh = message.MSH;
						msh.MessageType.TriggerEvent.Value = "A04";

						pid = message.PID;
						evn = message.EVN;
						pv1 = message.PV1;
						notificationMessage = message;

						break;
					}
				case NotificationType.DuplicatesResolved:
					{
						ADT_A39 message = new ADT_A39();

						msh = message.MSH;
						msh.MessageType.TriggerEvent.Value = "A40";

						pid = message.GetPATIENT(0).PID;
						evn = message.EVN;
						pv1 = message.GetPATIENT(0).PV1;
						mrg = message.GetPATIENT(0).MRG;
						notificationMessage = message;

						break;
					}
				case NotificationType.Update:
					{
						ADT_A01 message = new ADT_A01();

						msh = message.MSH;
						msh.MessageType.TriggerEvent.Value = "A08";

						pid = message.PID;
						evn = message.EVN;
						pv1 = message.PV1;
						notificationMessage = message;

						break;
					}
				default:
					throw new ArgumentOutOfRangeException($"Invalid notification type {workItem.NotificationType}");
			}

			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(IAssigningAuthorityRepositoryService)} cannot be null");
			}


		}
	}
}
