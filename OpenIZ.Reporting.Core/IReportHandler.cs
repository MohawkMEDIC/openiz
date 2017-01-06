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
 * Date: 2017-1-5
 */

using OpenIZ.Core.Model.RISI;
using System;
using System.Collections.Generic;

namespace OpenIZ.Reporting.Core
{
	/// <summary>
	/// Represents a report handler for a reporting engine.
	/// </summary>
	public interface IReportHandler : IDisposable
	{
		/// <summary>
		/// Gets or sets the report URI.
		/// </summary>
		Uri ReportUri { get; set; }

		/// <summary>
		/// Runs a report.
		/// </summary>
		/// <param name="reportId">The id of the report.</param>
		/// <param name="format">The format of the report.</param>
		/// <param name="parameters">The parameters of the report.</param>
		byte[] RunReport(Guid reportId, ReportFormat format, IEnumerable<ReportParameter> parameters);
	}
}