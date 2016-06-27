using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.PCL.Http
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
		public XmlBodySerializer (Type type)
		{
			this.m_serializer = new XmlSerializer (type);
		}


		#region IBodySerializer implementation
		/// <summary>
		/// Serialize the object
		/// </summary>
		public void Serialize (System.IO.Stream s, object o)
		{
			this.m_serializer.Serialize (s, o);
		}

		/// <summary>
		/// Serialize the reply stream
		/// </summary>
		public object DeSerialize (System.IO.Stream s)
		{
			return this.m_serializer.Deserialize (s);
		}
		#endregion
	}
}

