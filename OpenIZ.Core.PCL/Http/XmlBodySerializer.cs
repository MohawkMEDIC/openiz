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
using System.Xml.Serialization;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Represents a body serializer that uses XmlSerializer
	/// </summary>
	internal class XmlBodySerializer : IBodySerializer
	{
		// Serializer
		private XmlSerializer m_serializer;

		/// <summary>
		/// Creates a new body serializer
		/// </summary>
		public XmlBodySerializer(Type type)
		{
			this.m_serializer = new XmlSerializer(type);
		}

		#region IBodySerializer implementation

		/// <summary>
		/// Serialize the object
		/// </summary>
		public void Serialize(System.IO.Stream s, object o)
		{
			this.m_serializer.Serialize(s, o);
		}

		/// <summary>
		/// Serialize the reply stream
		/// </summary>
		public object DeSerialize(System.IO.Stream s)
		{
			return this.m_serializer.Deserialize(s);
		}

		#endregion IBodySerializer implementation
	}
}