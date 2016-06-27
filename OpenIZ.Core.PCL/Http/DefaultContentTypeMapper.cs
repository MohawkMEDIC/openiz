using System;

namespace OpenIZ.Core.PCL.Http
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
		/// <returns>The serializer.</returns>
		/// <param name="contentType">Content type.</param>
		public IBodySerializer GetSerializer (string contentType, Type typeHint)
		{
			switch (contentType) {
				case "text/xml":
				case "application/xml":
					return new XmlBodySerializer (typeHint);
				case "application/json":
					return new JsonBodySerializer (typeHint);
				case "application/x-www-urlform-encoded":
					return new FormBodySerializer ();
				default:
					throw new InvalidOperationException ();
			}
		}
		#endregion
	}
}

