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
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.BusinessRules
{
	/// <summary>
	/// Represents a business rules information wrapper.
	/// </summary>
	[XmlType(nameof(BusinessRuleInfo), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(BusinessRuleInfo), Namespace = "http://openiz.org/ami")]
	public class BusinessRuleInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessRuleInfo"/> class.
		/// </summary>
		public BusinessRuleInfo()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessRuleInfo"/> class
		/// with specific content.
		/// </summary>
		/// <param name="content">The content of the business rule.</param>
		public BusinessRuleInfo(byte[] content)
		{
			this.Content = content;
		}

		/// <summary>
		/// Gets or sets the encoded content of the business rule.
		/// </summary>
		public byte[] Content { get; set; }
	}
}