/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OizDevTool
{
    /// <summary>
    /// A tool which imports CDC formatted XML data files
    /// </summary>
    [Description("Vocabulary management tool")]
    public static class VocabTool
    {

        private class CvxOptions
        {
            /// <summary>
            /// Gets or sets the input
            /// </summary>
            [Parameter("input")]
            [Parameter("i")]
            [Description("The path to the import file")]
            public String Input { get; set; }

            /// <summary>
            /// Represents the group file
            /// </summary>
            [Parameter("group")]
            [Parameter("g")]
            [Description("The grouping file (not used)")]
            public String Group { get; set; }

            /// <summary>
            /// Output
            /// </summary>
            [Parameter("output")]
            [Parameter("o")]
            [Description("The output dataset file to create")]
            public String Output { get; set; }

            /// <summary>
            /// Concept set name
            /// </summary>
            [Parameter("setName")]
            [Description("The ConceptSet which should be created")]
            public String ConceptSetName { get; set; }

            /// <summary>
            /// The OID of the concept set
            /// </summary>
            [Parameter("setOid")]
            [Description("The OID of the created concept set")]
            public String ConceptSetOid { get; set; }

            /// <summary>
            /// The OID of the concept set
            /// </summary>
            [Parameter("setDescription")]
            [Description("The description to attach to the created concept set")]
            public String ConceptSetDescription { get; set; }
        }

        /// <summary>
        /// Converts PHIN VADS to dataset
        /// </summary>
        [Description("Converts a CDC PHIN VADs file to DATASET import file")]
        [ParameterClass(typeof(CvxOptions))]
        [Example("Import a downloaded PHIN VADs file for Cause of Death to dataset", "--input=CauseOfDeath.txt --output=CauseOfDeath.dataset --setName=CauseOfDeath")]
        public static void PhinVadsToDataset(String[] args)
        {

            var options = new ParameterParser<CvxOptions>().Parse(args);
            DatasetInstall conceptDataset = new DatasetInstall();
            using (var sr = File.OpenText(options.Input))
            {
                // Consume the first line
                sr.ReadLine();
                var components = sr.ReadLine().Split('\t');
                // Next line is the value set information
                // Create  code concept set
                var conceptSet = new ConceptSet()
                {
                    Key = Guid.NewGuid(),
                    Mnemonic = options.ConceptSetName ?? components[1],
                    Name = options.ConceptSetDescription ?? components[0],
                    Oid = options.ConceptSetOid ?? components[2],
                    CreationTime = DateTime.Parse(String.IsNullOrEmpty(components[6]) ? DateTime.Now.ToString() : components[6]),
                    Url = !String.IsNullOrEmpty(options.ConceptSetName) ? "http://openiz.org/conceptset/" + options.ConceptSetName :
                            "http://openiz.org/conceptsets/contrib/" + components[1]
                };

                Dictionary<String, CodeSystem> codeSystems = new Dictionary<string, CodeSystem>();

                // Consume the next set of lines 
                while (!sr.ReadLine().StartsWith("Concept Code")) ;

                while(!sr.EndOfStream)
                {
                    components = sr.ReadLine().Split('\t');

                    // Try to get the code system
                    CodeSystem codeSys = null;
                    if (!codeSystems.TryGetValue(components[4], out codeSys))
                    {
                        codeSys = new CodeSystem()
                        {
                            Key = Guid.NewGuid(),
                            Authority = components[8],
                            Name = components[6],
                            Oid = components[4],
                            Description = components[5],
                            CreationTime = conceptSet.CreationTime
                        };
                        codeSystems.Add(codeSys.Oid, codeSys);
                        conceptDataset.Action.Add(new DataUpdate()
                        {
                            InsertIfNotExists = true,
                            Element = codeSys
                        });
                    }

                    // Insert the code
                    var refTerm = new ReferenceTerm()
                    {
                        Key = Guid.NewGuid(),
                        CodeSystemKey = codeSys.Key,
                        CreationTime = conceptSet.CreationTime,
                        DisplayNames = new List<ReferenceTermName>()
                        {
                            new ReferenceTermName()
                            {
                                Name = components[1],
                                Language = "en"
                            }
                        },
                        Mnemonic = components[0]
                    };
                    var concept = new Concept()
                    {
                        Key = Guid.NewGuid(),
                        Mnemonic = components[6] + "-" + components[1].Replace("[","").Replace("]","").Replace("-","").Replace(" ",""),
                        CreationTime = conceptSet.CreationTime,
                        StatusConceptKey = StatusKeys.Active,
                        ClassKey = ConceptClassKeys.Other,
                        ConceptNames = new List<ConceptName>()
                        {
                            new ConceptName()
                            {
                                Name = components[1],
                                Language = "en"
                            }
                        },
                        ReferenceTerms = new List<ConceptReferenceTerm>()
                        {
                            new ConceptReferenceTerm()
                            {
                                ReferenceTermKey = refTerm.Key,
                                RelationshipTypeKey = ConceptRelationshipTypeKeys.SameAs
                            }
                        }
                    };
                    conceptDataset.Action.Add(new DataUpdate() { InsertIfNotExists = true, Element = refTerm });
                    conceptDataset.Action.Add(new DataUpdate() { InsertIfNotExists = true, Element = concept });
                    conceptSet.ConceptsXml.Add(concept.Key.Value);
                }
                conceptDataset.Action.Add(new DataUpdate() { InsertIfNotExists = true, Element = conceptSet });
            }

            XmlSerializer xsz = new XmlSerializer(typeof(DatasetInstall));
            using (FileStream fs = File.Create(options.Output))
                xsz.Serialize(fs, conceptDataset);
        }

        /// <summary>
        /// CVX Import
        /// </summary>
        [Description("Converts a HL7 CVX definition file to DATASET import file. Download is in text pip-delimited format")]
        [ParameterClass(typeof(CvxOptions))]
        [Example("Import a downloaded CVX definition file", "--input=CVXCodes.txt --output=VaccineCodes.dataset")]
        public static void CvxToDataset(String[] args)
        {
            var options = new ParameterParser<CvxOptions>().Parse(args);
            DatasetInstall conceptDataset = new DatasetInstall() { Id = "CDC CVX Codes" };
            // Create vaccine code concept set
            var conceptSet = new DataUpdate()
            {
                InsertIfNotExists = true,
                Element = new ConceptSet()
                {
                    Key = Guid.NewGuid(),
                    Mnemonic = "VaccineTypeCodes",
                    Name = "Vaccines",
                    Oid = "1.3.6.1.4.1.33349.3.1.5.9.1.25",
                    Url = "http://openiz.org/conceptset/VaccineTypeCodes"
                },
                Association = new List<DataAssociation>()
            };
            var codeSystem = new DataUpdate()
            {
                InsertIfNotExists = true,
                Element = new CodeSystem("HL7 CVX Codes", "2.16.840.1.113883.3.88.12.80.22", "CVX")
                {
                    Url = "http://hl7.org/fhir/sid/cvx",
                    Key = Guid.Parse("EBA4F94A-2CAD-4BB3-ACA7-F4E54EAAC4BD")
                }
            };
            conceptDataset.Action.Add(codeSystem);

            using (var codeReader = File.OpenText(options.Input))
            {

                // Now import the concepts from CVX to their OpenIZ Concepts
                while (!codeReader.EndOfStream)
                {
                    var sourceLine = codeReader.ReadLine();
                    var components = sourceLine.Split('|').Select(o => o.Trim()).ToArray();

                    ReferenceTerm refTerm = new ReferenceTerm()
                    {
                        CodeSystemKey = codeSystem.Element.Key,
                        Mnemonic = components[0],
                        DisplayNames = new List<ReferenceTermName>()
                        {
                            new ReferenceTermName() { Language = "en", Name = components[1] }
                        },
                        Key = Guid.NewGuid()
                    };

                    var mnemonic = String.Format("VaccineType-{0}", components[1].Replace(" ", "").Replace(".", "").Replace("(", "").Replace(")", ""));
                    if (mnemonic.Length > 64)
                        mnemonic = mnemonic.Substring(0, 64);
                    Concept concept = new Concept()
                    {
                        Key = Guid.NewGuid(),
                        Mnemonic = mnemonic,
                        ClassKey = ConceptClassKeys.Material,
                        ConceptNames = new List<ConceptName>()
                        {
                            new ConceptName() { Language = "en", Name = components[2] },
                            new ConceptName() { Language = "en", Name = components[1] }
                        },
                        ReferenceTerms = new List<ConceptReferenceTerm>() { new ConceptReferenceTerm()
                            {
                                ReferenceTermKey = refTerm.Key,
                                RelationshipTypeKey = ConceptRelationshipTypeKeys.SameAs
                            }
                        },
                        StatusConceptKey = components[3] == "Inactive" ? StatusKeys.Obsolete : StatusKeys.Active,
                        CreationTime = DateTime.Parse(components[6])
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
            }

            conceptDataset.Action.Add(conceptSet);
            XmlSerializer xsz = new XmlSerializer(typeof(DatasetInstall));
            using (FileStream fs = File.Create(options.Output))
                xsz.Serialize(fs, conceptDataset);

        }


        /// <summary>
        /// CVX Import
        /// </summary>
        [Description("Converts a Tab-delimited file to DATASET import file. The format of this file is described on the OpenIZ Wiki")]
        [ParameterClass(typeof(CvxOptions))]
        [Example("Import a downloaded CVX definition file", "--input=mycustomcodes.tsv --setName=\"ACME_MATL\" --setOid=1.2.3.4.5.6.7 --setDescription=\"ACME Material Codes\" --output=AcmeMaterials.dataset")]
        public static void TsvToDataset(String[] args)
        {
            var options = new ParameterParser<CvxOptions>().Parse(args);
            DatasetInstall conceptDataset = new DatasetInstall() { Id = "TSV File" };
            
            using (var codeReader = File.OpenText(options.Input))
            {

                var sourceLine = codeReader.ReadLine();
                var components = sourceLine.Split('\t').Select(o => o.Trim()).ToArray();

                // Create vaccine code concept set
                var conceptSet = new DataUpdate()
                {
                    InsertIfNotExists = true,
                    Element = new ConceptSet()
                    {
                        Key = Guid.NewGuid(),
                        Mnemonic = options.ConceptSetName ?? "TsvConceptSet",
                        Name = components[5],
                        Oid = components[6],
                        Url = "http://openiz.org/conceptset/imported/" + (options.ConceptSetName ?? "TsvConceptSet")
                    },
                    Association = new List<DataAssociation>()
                };
                var codeSystem = new DataUpdate()
                {
                    InsertIfNotExists = true,
                    Element = new CodeSystem(components[5], components[6], (options.ConceptSetName ?? "TsvConceptSet"))
                    {
                        Key = Guid.NewGuid(),

                        Url = "http://openiz.org/conceptset/imported/" + (options.ConceptSetName ?? "TsvConceptSet"),
                    }
                };
                conceptDataset.Action.Add(codeSystem);
                
                // Now import the concepts from CVX to their OpenIZ Concepts
                while (!codeReader.EndOfStream)
                {
                   

                    ReferenceTerm refTerm = new ReferenceTerm()
                    {
                        CodeSystemKey = codeSystem.Element.Key,
                        Mnemonic = components[1],
                        DisplayNames = new List<ReferenceTermName>()
                        {
                            new ReferenceTermName() { Language = "en", Name = components[2] }
                        },
                        Key = Guid.NewGuid()
                    };

                    var mnemonic = String.Format("{0}-{1}", options.ConceptSetName ?? "Tsv",  components[1].Replace(" ", "").Replace(".", "").Replace("(", "").Replace(")", ""));
                    if (mnemonic.Length > 64)
                        mnemonic = mnemonic.Substring(0, 64);
                    Concept concept = new Concept()
                    {
                        Key = Guid.Parse(components[0]),
                        Mnemonic = mnemonic,
                        ClassKey = ConceptClassKeys.Material,
                        ConceptNames = new List<ConceptName>()
                        {
                            new ConceptName() { Language = "en", Name = components[2] }
                        },
                        ReferenceTerms = new List<ConceptReferenceTerm>() { new ConceptReferenceTerm()
                            {
                                ReferenceTermKey = refTerm.Key,
                                RelationshipTypeKey = ConceptRelationshipTypeKeys.SameAs
                            }
                        },
                        StatusConceptKey = StatusKeys.Active
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

                    sourceLine = codeReader.ReadLine();
                    components = sourceLine.Split('\t').Select(o => o.Trim()).ToArray();
                }

                conceptDataset.Action.Add(conceptSet);

            }

            XmlSerializer xsz = new XmlSerializer(typeof(DatasetInstall));
            using (FileStream fs = File.Create(options.Output))
                xsz.Serialize(fs, conceptDataset);

        }
    }
}
