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
 * Date: 2016-11-30
 */
using MARC.Everest.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.ResultsDetails
{
	/// <summary>
	/// Represents an unrecognized target domain result detail.
	/// </summary>
	public class UnrecognizedTargetDomainResultDetail : ResultDetail
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnrecognizedTargetDomainResultDetail"/> class
		/// with a specific message.
		/// </summary>
		/// <param name="message">The message of the result detail.</param>
		public UnrecognizedTargetDomainResultDetail(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnrecognizedTargetDomainResultDetail"/> class
		/// with a specific type, message, and exception.
		/// </summary>
		/// <param name="type">The type of the result detail.</param>
		/// <param name="message">The message of the result detail.</param>
		/// <param name="exception">The exception of the result detail.</param>
		public UnrecognizedTargetDomainResultDetail(ResultDetailType type, string message, Exception exception) : base(type, message, exception)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnrecognizedTargetDomainResultDetail"/> class
		/// with a specific type, message, and location.
		/// </summary>
		/// <param name="type">The type of the result detail.</param>
		/// <param name="message">The message of the result detail.</param>
		/// <param name="location">The location of the result detail.</param>
		public UnrecognizedTargetDomainResultDetail(ResultDetailType type, string message, string location) : base(type, message, location)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnrecognizedTargetDomainResultDetail"/> class
		/// with a specific type, message, location, and exception.
		/// </summary>
		/// <param name="type">The type of the result detail.</param>
		/// <param name="message">The message of the result detail.</param>
		/// <param name="location">The location of the result detail.</param>
		/// <param name="exception">The exception of the result detail.</param>
		public UnrecognizedTargetDomainResultDetail(ResultDetailType type, string message, string location, Exception exception) : base(type, message, location, exception)
		{
		}
	}
}
