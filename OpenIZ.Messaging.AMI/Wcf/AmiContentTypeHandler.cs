/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */

using System.ServiceModel.Channels;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents an AMI content type handler.
	/// </summary>
	/// <seealso cref="System.ServiceModel.Channels.WebContentTypeMapper" />
	public class AmiContentTypeHandler : WebContentTypeMapper
	{
		/// <summary>
		/// When overridden in a derived class, returns the message format used for a specified content type.
		/// </summary>
		/// <param name="contentType">The content type that indicates the MIME type of data to be interpreted.</param>
		/// <returns>The <see cref="T:System.ServiceModel.Channels.WebContentFormat" /> that specifies the format to which the message content type is mapped.</returns>
		public override WebContentFormat GetMessageFormatForContentType(string contentType)
		{
			return WebContentFormat.Raw;
		}
	}
}