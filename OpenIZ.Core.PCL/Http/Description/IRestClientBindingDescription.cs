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
		/// Gets or sets a value indicating whether this <see cref="OpenIZ.Mobile.Core.Configuration.ServiceClientBinding"/>
		/// is optimized
		/// </summary>
		/// <value><c>true</c> if optimize; otherwise, <c>false</c>.</value>
		bool Optimize
		{
			get;
		}
	}
}