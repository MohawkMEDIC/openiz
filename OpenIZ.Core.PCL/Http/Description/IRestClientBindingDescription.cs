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
 * Date: 2016-8-2
 */
namespace OpenIZ.Core.Http.Description
{
	/// <summary>
	/// REST client binding
	/// </summary>
	public interface IRestClientBindingDescription
	{
		/// <summary>
		/// Content type mapper
		/// </summary>
		/// <value>The content type mapper.</value>
		IContentTypeMapper ContentTypeMapper { get; }

		/// <summary>
		/// Gets or sets the security configuration
		/// </summary>
		/// <value>The security.</value>
		IRestClientSecurityDescription Security { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IRestClientBindingDescription"/> is optimize.
		/// </summary>
		/// <value><c>true</c> if optimize; otherwise, <c>false</c>.</value>
		bool Optimize { get; }
	}
}