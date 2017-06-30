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
 * User: justi
 * Date: 2016-8-2
 */
using MARC.Everest.Attributes;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OizDevTool
{
    /// <summary>
    /// Import value set from everest
    /// </summary>
    [Description("MARC-HI Everest Tooling")]
    public static class EverestImport
    {

        /// <summary>
        /// Console parameters
        /// </summary>
        private class ConsoleParameters
        {
            /// <summary>
            /// Gets or sets the assembly file
            /// </summary>
            [Parameter("asm")]
            [Description("The path to the assembly file from which to import data")]
            public String AssemblyFile { get; set; }

        }

        /// <summary>
        /// Convert an enumeration to a dataset
        /// </summary>
        [Description("Imports Everest Enumerations as dataset files")]
        [ParameterClass(typeof(ConsoleParameters))]
        [Example("Import all CDA vocabulary as OpenIZ datasets", "--asm=MARC.Everest.RMIM.UV.CDAr2.dll")]
        public static void EnumToDataset(String[] args)
        {

            var parms = new ParameterParser<ConsoleParameters>().Parse(args);

            var asm = Assembly.LoadFile(parms.AssemblyFile);
            foreach (var en in asm.ExportedTypes.Where(o => o.IsEnum && o.GetCustomAttribute<StructureAttribute>() != null))
            {
                StructureAttribute sta = en.GetCustomAttribute<StructureAttribute>();

                DatasetInstall conceptDataset = new DatasetInstall() { Id = String.Format("HL7v3 {0} Concept Set", sta.Name) };

                // Create vaccine code concept set
                var conceptSet = new DataUpdate()
                {
                    InsertIfNotExists = true,
                    Element = new ConceptSet()
                    {
                        Key = Guid.NewGuid(),
                        Mnemonic = en.Name,
                        Name = sta.Name,
                        Oid = sta.CodeSystem,
                        Url = String.Format("http://openiz.org/valuset/v3-{0}", en.Name)
                    },
                    Association = new List<DataAssociation>()
                };

                var codeSystem = new DataUpdate()
                {
                    InsertIfNotExists = true,
                    Element = new CodeSystem(sta.Name, sta.CodeSystem, en.Name)
                    {
                        Url = String.Format("http://hl7.org/fhir/v3-{0}", en.Name),
                        Key = Guid.NewGuid(),
                    }
                };
                conceptDataset.Action.Add(codeSystem);

                foreach (var enm in en.GetFields())
                {

                    var ena = enm.GetCustomAttribute<EnumerationAttribute>();

                    if (ena == null) continue;

                    var dsa = enm.GetCustomAttribute<DescriptionAttribute>();

                    ReferenceTerm refTerm = new ReferenceTerm()
                    {
                        CodeSystemKey = codeSystem.Element.Key,
                        Mnemonic = ena.Value,
                        DisplayNames = new List<ReferenceTermName>()
                        {
                            new ReferenceTermName() { Language = "en", Name = dsa?.Description ?? enm.Name}
                        },
                        Key = Guid.NewGuid()
                    };

                    var mnemonic = String.Format("{0}-{1}", en.Name, enm.Name);
                    if (mnemonic.Length > 64)
                        mnemonic = mnemonic.Substring(0, 64);
                    Concept concept = new Concept()
                    {
                        Key = Guid.NewGuid(),
                        Mnemonic = mnemonic,
                        ClassKey = ConceptClassKeys.Other,
                        ConceptNames = new List<ConceptName>()
                        {
                            new ConceptName() { Language = "en", Name = dsa?.Description }
                        },
                        ReferenceTerms = new List<ConceptReferenceTerm>() { new ConceptReferenceTerm()
                            {
                                ReferenceTermKey = refTerm.Key,
                                RelationshipTypeKey = ConceptRelationshipTypeKeys.SameAs
                            }
                        },
                        StatusConceptKey = StatusKeys.Active,
                        CreationTime = DateTime.Now
                    };

                    conceptDataset.Action.Add(new DataUpdate()
                    {
                        InsertIfNotExists = true,
                        Element = refTerm
                    });
                    conceptDataset.Action.Add(new DataUpdate()
                    {
                        InsertIfNotExists = true,
                        Element = concept
                    });
                    (conceptSet.Element as ConceptSet).ConceptsXml.Add(concept.Key.Value);

                }

                conceptDataset.Action.Add(conceptSet);
                XmlSerializer xsz = new XmlSerializer(typeof(DatasetInstall));
                using (FileStream fs = File.Create(en.Name + ".dataset"))
                    xsz.Serialize(fs, conceptDataset);
            }


        }
    }
}
