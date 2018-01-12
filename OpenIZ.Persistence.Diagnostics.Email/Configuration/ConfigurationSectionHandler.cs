using System;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace OpenIZ.Persistence.Diagnostics.Email.Configuration
{
	/// <summary>
	/// Configuration section handler
	/// </summary>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates the specified configuration object
		/// </summary>
		public object Create(object parent, object configContext, XmlNode section)
		{
			XmlElement smtp = section.SelectSingleNode("./*[local-name() = 'smtp']") as XmlElement;
            var recipients = section.SelectNodes("./*[local-name() = 'recipient']/*[local-name() = 'add']");

            DiagnosticEmailServiceConfiguration retVal = new DiagnosticEmailServiceConfiguration();
			if (smtp == null)
				throw new ConfigurationErrorsException("Missing SMTP configuration", section);
			else
				retVal.Smtp = new SmtpConfiguration(new Uri(smtp.Attributes["server"]?.Value ?? "smtp://localhost:25"), smtp.Attributes["username"]?.Value ?? string.Empty, smtp.Attributes["password"]?.Value ?? string.Empty, Boolean.Parse(smtp.Attributes["ssl"]?.Value ?? "false"), smtp.Attributes["from"]?.Value ?? "no-reply@openiz.org");

            retVal.Recipients = new System.Collections.Generic.List<string>();
            foreach (XmlElement itm in recipients)
                retVal.Recipients.Add(itm.InnerText);
            return retVal;
		}
	}
}