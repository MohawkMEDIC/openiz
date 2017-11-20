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
using System;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Default body binder.
	/// </summary>
	public class DefaultContentTypeMapper : IContentTypeMapper
	{
		#region IBodySerializerBinder implementation

		/// <summary>
		/// Gets the body serializer based on the content type
		/// </summary>
		/// <param name="contentType">Content type.</param>
		/// <param name="typeHint">The type hint.</param>
		/// <returns>The serializer.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">contentType - Not supported</exception>
		public IBodySerializer GetSerializer(string contentType, Type typeHint)
		{
			switch (contentType)
			{
				case "text/xml":
				case "application/xml":
				case "application/xml; charset=utf-8":
				case "application/xml; charset=UTF-8":
					return new XmlBodySerializer(typeHint);

				case "application/json":
				case "application/json; charset=utf-8":
				case "application/json; charset=UTF-8":
					return new JsonBodySerializer(typeHint);

				case "application/x-www-urlform-encoded":
					return new FormBodySerializer();

                case "application/octet-stream":
                    return new BinaryBodySerializer();

                default:
                    if (contentType.StartsWith("multipart/form-data"))
                        return new MultipartBinarySerializer(contentType);

					throw new ArgumentOutOfRangeException(nameof(contentType), contentType, "Not supported");
			}
		}

		#endregion IBodySerializerBinder implementation
	}
}