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
 * User: justi
 * Date: 2016-8-24
 */
using System;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a thread pooling service
	/// </summary>
	public interface IThreadPoolService
	{
		/// <summary>
		/// Queues the specified action into the worker pool
		/// </summary>
		void QueueUserWorkItem(Action<Object> action);

		/// <summary>
		/// Queues the specified action into the worker pool
		/// </summary>
		void QueueUserWorkItem(Action<Object> action, Object parm);

        /// <summary>
        /// Queue a user work item
        /// </summary>
        void QueueUserWorkItem(TimeSpan timeout, Action<Object> action, Object parm);

        /// <summary>
        /// Creates a normal thread which is not in the pool
        /// </summary>
        void QueueNonPooledWorkItem(Action<Object> action, Object parm);
    }
}
