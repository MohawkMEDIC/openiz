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
using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.HL7.Configuration;

namespace OpenIZ.Messaging.HL7.Notifier
{
	/// <summary>
	/// Represents a patient identity cross reference manager notifier.
	/// </summary>
	public class PAT_ID_X_REF_MGR : INotifier<Patient>
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
			throw new NotImplementedException();
		}
	}
}
