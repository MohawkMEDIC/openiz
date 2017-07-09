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
 * Date: 2017-4-10
 */

using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace OizDevTool
{
	/// <summary>
	/// Represents a shared value set import.
	/// </summary>
	[Description("Tooling for import IHE Shared Value Sets (SVS) files")]
	public class SvsImport
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SvsImport"/> class.
		/// </summary>
		public SvsImport()
		{
		}

		/// <summary>
		/// Imports the SVS.
		/// </summary>
		/// <param name="args">The arguments.</param>
		[Description("Imports an SVS file and generates the related DataSet file")]
		[ParameterClass(typeof(SvsOptions))]
		public static void ImportSvs(string[] args)
		{
			var parameters = new ParameterParser<SvsOptions>().Parse(args);

			var svsDatasetInstall = new DatasetInstall
			{
				Action = new List<DataInstallAction>()
			};

			var fileInfo = new FileInfo(parameters.File);

			var serializer = new XmlSerializer(typeof(RetrieveValueSetResponseType));

			var svs = (RetrieveValueSetResponseType)serializer.Deserialize(new StreamReader(parameters.File));

			svsDatasetInstall.Id = svs.ValueSet.displayName;

			foreach (var conceptListType in svs.ValueSet.ConceptList)
			{
				var codeSystem = new CodeSystem
				{
					Key = Guid.NewGuid(),
					Name = svs.ValueSet.displayName,
					Oid = svs.ValueSet.id,
					VersionText = svs.ValueSet.version
				};

				svsDatasetInstall.Action.Add(new DataUpdate
				{
					Element = codeSystem,
					InsertIfNotExists = true
				});

				foreach (var cpt in conceptListType.Concept)
				{
					var referenceTerm = new ReferenceTerm
					{
						CodeSystemKey = codeSystem.Key,
						DisplayNames = new List<ReferenceTermName>
						{
							new ReferenceTermName(conceptListType.lang?.Length > 2 ? conceptListType.lang?.Substring(0, 2) : "en", cpt.displayName)
						},
						Key = Guid.NewGuid(),
						Mnemonic = cpt.code
					};

					svsDatasetInstall.Action.Add(new DataUpdate
					{
						Element = referenceTerm,
						InsertIfNotExists = true
					});

					var concept = new Concept
					{
						ClassKey = ConceptClassKeys.Other,
						ConceptNames = new List<ConceptName>
						{
							new ConceptName(conceptListType.lang?.Length > 2 ? conceptListType.lang?.Substring(0, 2) : "en", cpt.displayName)
						},
						CreationTime = DateTimeOffset.Now,
						Key = Guid.NewGuid(),
						Mnemonic = cpt.code,
						ReferenceTerms = new List<ConceptReferenceTerm>
						{
							new ConceptReferenceTerm(referenceTerm.Key, ConceptRelationshipTypeKeys.SameAs)
						},
						StatusConceptKey = StatusKeys.Active
					};

					svsDatasetInstall.Action.Add(new DataUpdate
					{
						Element = concept,
						InsertIfNotExists = true
					});
				}
			}

			serializer = new XmlSerializer(typeof(DatasetInstall));

			using (var fileStream = File.Create($"999-SVS-import-{fileInfo.Name}.dataset"))
			{
				serializer.Serialize(fileStream, svsDatasetInstall);
			}
		}

		/// <summary>
		/// Represents SVS import options.
		/// </summary>
		internal class SvsOptions
		{
			/// <summary>
			/// Gets or sets the file.
			/// </summary>
			[Parameter("file")]
			[Description("The SVS file to be imported")]
			public string File { get; set; }
		}
	}
}