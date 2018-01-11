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
 * User: justi
 * Date: 2016-8-2
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenIZ.Core.Model;
using System;
using System.IO;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Represents a body serializer that uses JSON
	/// </summary>
	internal class JsonBodySerializer : IBodySerializer
	{
		// Serializer
		private JsonSerializer m_serializer;

		// The type
		private Type m_type;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Core.Http.JsonBodySerializer"/> class.
		/// </summary>
		public JsonBodySerializer(Type type)
		{
			this.m_serializer = new JsonSerializer()
			{
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				NullValueHandling = NullValueHandling.Ignore,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				TypeNameHandling = TypeNameHandling.Auto
			};

			this.m_serializer.Converters.Add(new StringEnumConverter());
			this.m_type = type;
		}

		#region IBodySerializer implementation

		/// <summary>
		/// Serialize
		/// </summary>
		public void Serialize(System.IO.Stream s, object o)
		{
			using (TextWriter tw = new StreamWriter(s, System.Text.Encoding.UTF8, 2048, true))
			using (JsonTextWriter jw = new JsonTextWriter(tw))
				this.m_serializer.Serialize(jw, o);
		}

		/// <summary>
		/// De-serialize the body
		/// </summary>
		public object DeSerialize(System.IO.Stream s)
		{
			using (TextReader tr = new StreamReader(s, System.Text.Encoding.UTF8, true, 2048, true))
			using (JsonTextReader jr = new JsonTextReader(tr))
				return this.m_serializer.Deserialize(jr, this.m_type);
		}

		#endregion IBodySerializer implementation
	}
}