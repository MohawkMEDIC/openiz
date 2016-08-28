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
 * User: Nityan
 * Date: 2016-8-27
 */

namespace OpenIZ.Messaging.RISI.Configuration
{
	/// <summary>
	/// Represents a configuration for a RISI configuration.
	/// </summary>
	public class RisiConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RisiConfiguration"/> class
		/// with a specified service name.
		/// </summary>
		/// <param name="wcfServiceName"></param>
		public RisiConfiguration(string wcfServiceName)
		{
			this.WcfServiceName = wcfServiceName;
		}

		/// <summary>
		/// Gets the WCF service name of the configuration.
		/// </summary>
		public string WcfServiceName { get; private set; }
	}
}