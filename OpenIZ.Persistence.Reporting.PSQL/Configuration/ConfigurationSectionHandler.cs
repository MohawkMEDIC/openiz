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
 * User: khannan
 * Date: 2017-1-16
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.OrmLite.Providers;
using System;
using System.Configuration;
using System.Xml;

namespace OpenIZ.Persistence.Reporting.PSQL.Configuration
{
	/// <summary>
	/// Represents a configuration section handler for the reporting configuration.
	/// </summary>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates a configuration section handler.
		/// </summary>
		/// <param name="parent">Parent object.</param>
		/// <param name="configContext">Configuration context object.</param>
		/// <param name="section">Section XML node.</param>
		/// <returns>The created section handler object.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			var configuration = new ReportingConfiguration();

			var connectionManagerElement = section.SelectSingleNode("./*[local-name() = 'connectionManager']") as XmlElement;

			if (connectionManagerElement == null)
			{
				throw new ConfigurationErrorsException("No connection manager specified", section);
			}

			var readWriteConnection = connectionManagerElement.Attributes["readWriteConnection"]?.Value;

			if (readWriteConnection == null)
			{
				throw new ConfigurationErrorsException("The 'connectionManager' element must have a 'readWriteConnection' attribute");
			}

			var readonlyConnection = connectionManagerElement.Attributes["readonlyConnection"]?.Value;

			if (readonlyConnection == null)
			{
				throw new ConfigurationErrorsException("The 'connectionManager' element must have a 'readonlyConnection' attribute");
			}

			configuration.ReadWriteConnectionString = readWriteConnection;
			configuration.ReadonlyConnectionString = readonlyConnection;
			configuration.TraceSql = bool.Parse(connectionManagerElement.Attributes["traceSql"]?.Value);

			var provider = connectionManagerElement.Attributes["provider"]?.Value;

			if (provider == null)
			{
				throw new ConfigurationErrorsException("Reporting PSQL persistence requires a 'provider' attribute");
			}

			var databaseProviderInstance = Activator.CreateInstance(Type.GetType(provider)) as IDbProvider;

			if (databaseProviderInstance == null)
			{
				throw new ConfigurationErrorsException($"Type {provider} does not implement {nameof(IDbProvider)}");
			}

			databaseProviderInstance.ConnectionString = ApplicationContext.Current.GetService<IConfigurationManager>().ConnectionStrings[configuration.ReadWriteConnectionString]?.ConnectionString;
			databaseProviderInstance.ReadonlyConnectionString = ApplicationContext.Current.GetService<IConfigurationManager>().ConnectionStrings[configuration.ReadonlyConnectionString]?.ConnectionString;
			databaseProviderInstance.TraceSql = configuration.TraceSql;

			configuration.Provider = databaseProviderInstance;

			return configuration;
		}
	}
}