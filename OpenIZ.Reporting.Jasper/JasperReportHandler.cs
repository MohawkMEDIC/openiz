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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.RISI;
using OpenIZ.Reporting.Core;
using OpenIZ.Reporting.Core.Attributes;

namespace OpenIZ.Reporting.Jasper
{
	/// <summary>
	/// Represents a Jasper server report handler.
	/// </summary>
    public class JasperReportHandler : IReportHandler, ISupportBasicAuthentication
	{
		/// <summary>
		/// The internal reference to the <see cref="HttpClient"/> instance.
		/// </summary>
		private HttpClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="JasperReportHandler"/> class
		/// with a specific report URI.
		/// </summary>
		/// <param name="reportUri">The URI of the report server.</param>
		public JasperReportHandler(Uri reportUri)
		{
			this.client = new HttpClient();
			this.ReportUri = reportUri;
		}

		/// <summary>
		/// Gets or sets the report URI.
		/// </summary>
		public Uri ReportUri { get; set; }

		/// <summary>
		/// Authenticates against a remote system using a username and password.
		/// </summary>
		/// <param name="username">The username of the user.</param>
		/// <param name="password">The password of the user.</param>
		public void Authenticate(string username, string password)
		{
			this.client.DefaultRequestHeaders.Add("Authorization", "BASIC " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password)));
		}

		/// <summary>
		/// Runs a report.
		/// </summary>
		/// <param name="reportId">The id of the report.</param>
		/// <param name="format">The format of the report.</param>
		/// <param name="parameters">The parameters of the report.</param>
		public byte[] RunReport(Guid reportId, ReportFormat format, IEnumerable<ReportParameter> parameters)
		{
			byte[] report = null;

			var orderedParameters = parameters.OrderBy(p => p.Order);

			var output = (format.GetType().GetField(format.ToString()).GetCustomAttributes(typeof(ReportFormat), false) as StringValueAttribute[])?[0]?.Value;

			if (output == null)
			{
				throw new ArgumentException($"Invalid report format { format }");
			}

			var path = this.ReportUri + "/" + reportId + "." + output;

			var first = true;

			foreach (var parameter in orderedParameters)
			{
				if (first)
				{
					path += "?" + orderedParameters.First().Name + "=" + orderedParameters.First().Value;
					first = false;
				}
				else
				{
					path += "&" + parameter.Name + "=" + parameter.Value;
				}
			}

			var response = this.client.GetAsync(path, HttpCompletionOption.ResponseContentRead).Result;

			if (response.IsSuccessStatusCode)
			{
				report = response.Content.ReadAsByteArrayAsync().Result;
			}

			return report;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.client?.Dispose();
		}
	}
}
