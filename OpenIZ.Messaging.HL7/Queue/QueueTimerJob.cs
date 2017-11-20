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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Timer;
using System.Diagnostics;
using System.Timers;

namespace OpenIZ.Messaging.HL7.Queue
{
	/// <summary>
	/// Represents a queue timer job.
	/// </summary>
	internal class QueueTimerJob : ITimerJob
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Called when the timer has elapsed.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The elapsed event arguments.</param>
		public void Elapsed(object sender, ElapsedEventArgs e)
		{
			var workItem = Hl7MessageQueue.Current.Dequeue();

			if (workItem == null)
			{
				this.tracer.TraceEvent(TraceEventType.Information, 0, "Queue is now empty");
				return;
			}

			if (!workItem.TrySend() && workItem.FailCount < 10)
			{
				Hl7MessageQueue.Current.Enqueue(workItem);
			}
			else
			{
				var localizationService = ApplicationContext.Current.GetService<ILocalizationService>();

				this.tracer.TraceEvent(TraceEventType.Error, 0, string.Format("{0}", localizationService?.GetString("NTFW006")));
			}
		}
	}
}