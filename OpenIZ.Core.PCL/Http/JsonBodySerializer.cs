using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
		public JsonBodySerializer (Type type)
		{
			this.m_serializer = new JsonSerializer () {
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				NullValueHandling = NullValueHandling.Ignore
			};
			this.m_serializer.Converters.Add (new StringEnumConverter ());
			this.m_type = type;
		}

		#region IBodySerializer implementation
		/// <summary>
		/// Serialize 
		/// </summary>
		public void Serialize (System.IO.Stream s, object o)
		{
			using(TextWriter tw = new StreamWriter(s))
			using(JsonTextWriter jw = new JsonTextWriter(tw))
				this.m_serializer.Serialize(jw, o);
		}

		/// <summary>
		/// De-serialize the body
		/// </summary>
		public object DeSerialize (System.IO.Stream s)
		{
			using (TextReader tr = new StreamReader (s))
			using (JsonTextReader jr = new JsonTextReader (tr))
				return this.m_serializer.Deserialize (jr, this.m_type);
		}

		#endregion

	}
}

