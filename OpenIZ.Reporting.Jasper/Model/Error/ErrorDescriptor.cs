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
 * Date: 2017-4-4
 */
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.Error
{
	/// <summary>
	/// Class ErrorDescriptor.
	/// </summary>
	[XmlType("errorDescriptor")]
	[XmlRoot("errorDescriptor")]
	public class ErrorDescriptor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorDescriptor"/> class.
		/// </summary>
		public ErrorDescriptor()
		{
			
		}

		/// <summary>
		/// Gets or sets the error code.
		/// </summary>
		/// <value>The error code.</value>
		[XmlElement("errorCode")]
		public string ErrorCode { get; set; }

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>The message.</value>
		[XmlElement("message")]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the parameters.
		/// </summary>
		/// <value>The parameters.</value>
		[XmlElement("parameters")]
		public List<ErrorParameter> Parameters { get; set; }
	}
}
