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
 * Date: 2016-8-28
 */
using System;
using System.Collections.Generic;
using System.ServiceModel;
using OpenIZ.Core.Model.RISI;

namespace OpenIZ.Messaging.RISI.Wcf
{
	/// <summary>
	/// Provides operations for running and managing reports.
	/// </summary>
	[ServiceBehavior(ConfigurationName = "RISI")]
	public class RisiBehavior : IRisiContract
	{
		/// <summary>
		/// Deletes a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to delete.</param>
		/// <returns>Returns the deleted report definition.</returns>
		public ReportDefinition DeleteReportDefinition(string id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a report definition by id.
		/// </summary>
		/// <param name="id">The id of the report definition to retrieve.</param>
		/// <returns>Returns a report definition.</returns>
		public ReportDefinition GetReportDefinition(string id)
		{
			throw new NotImplementedException();
		}

		public List<ParameterDefinition> GetReportParameters(string id)
		{
			throw new NotImplementedException();
		}

		public ReportDefinition GetReportSource(string id)
		{
			throw new NotImplementedException();
		}
	}
}