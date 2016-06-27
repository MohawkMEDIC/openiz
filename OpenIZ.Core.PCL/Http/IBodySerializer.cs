using System;
using System.IO;

namespace OpenIZ.Core.PCL.Http
{
	/// <summary>
	/// Defines behavior of a content/type mapper
	/// </summary>
	public interface IBodySerializer
	{

		/// <summary>
		/// Serialize the specified object
		/// </summary>
		void Serialize (Stream s, Object o);

		/// <summary>
		/// Serialize the reply stream
		/// </summary>
		Object DeSerialize(Stream s);

	}

	/// <summary>
	/// Defines a class that binds a series of serializers to content/types
	/// </summary>
	public interface IContentTypeMapper
	{

		/// <summary>
		/// Gets the body serializer based on the content type
		/// </summary>
		IBodySerializer GetSerializer(String contentType, Type type);

	}
}

