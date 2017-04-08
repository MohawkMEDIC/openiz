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
 * Date: 2017-4-7
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Persistence;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
	public class CsdImport
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CsdImport"/> class.
		/// </summary>
		public CsdImport()
		{
			
		}

		/// <summary>
		/// Console parameters
		/// </summary>
		public class ConsoleParameters
		{
			/// <summary>
			/// Gets or sets the url.
			/// </summary>
			[Parameter("url")]
			public string Url { get; set; }

			/// <summary>
			/// Gets or sets the file.
			/// </summary>
			[Parameter("file")]
			public string File { get; set; }
		}

		public static void ImportCsd(string[] args)
		{
			var parameters = new ParameterParser<ConsoleParameters>().Parse(args);

			var healthFacilityDataSetInstall = new DatasetInstall { Id = "Health Facilities", Action = new List<DataInstallAction>() };

			var serializer = new XmlSerializer(typeof(CSD));

			var csd = (CSD)serializer.Deserialize(new StreamReader(parameters.File));

			// map CSD organization to IMSI organization
			foreach (var csdOrganization in csd.organizationDirectory)
			{
				var organizaton = new Organization
				{
					Key = Guid.NewGuid()
				};

				if (csdOrganization.primaryName != null)
				{
					organizaton.Names.Add(new EntityName(NameUseKeys.OfficialRecord, csdOrganization.primaryName));
				}

			}
		}

		private static EntityName MapEntityName(organizationOtherName organizationName)
		{
			return new EntityName(NameUseKeys.OfficialRecord, organizationName.Value);
		}
	}
}

