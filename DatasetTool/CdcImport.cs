using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DatasetTool
{
    /// <summary>
    /// A tool which imports CDC formatted XML data files
    /// </summary>
    public static class CdcImport
    {

        private class CvxOptions
        {
            /// <summary>
            /// Gets or sets the input
            /// </summary>
            [Parameter("input")]
            [Parameter("i")]
            public String Input { get; set; }

            /// <summary>
            /// Represents the group file
            /// </summary>
            [Parameter("group")]
            [Parameter("g")]
            public String Group { get; set; }

            /// <summary>
            /// Output
            /// </summary>
            [Parameter("output")]
            [Parameter("o")]
            public String Output { get; set; }


        }


        /// <summary>
        /// CVX Import
        /// </summary>
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
                    Url = "http://openiz.org/valueset/VaccineTypeCodes"
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
                        Mnemonic = "TsvConceptSet",
                        Name = components[5],
                        Oid = components[6],
                        Url = "http://openiz.org/valueset/TsvConceptCodes"
                    },
                    Association = new List<DataAssociation>()
                };
                var codeSystem = new DataUpdate()
                {
                    InsertIfNotExists = true,
                    Element = new CodeSystem(components[5], components[6], "TSV")
                    {
                        Key = Guid.NewGuid(),

                        Url = "http://hl7.org/fhir/sid/TSV",
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

                    var mnemonic = String.Format("TabSavedFile-{0}", components[1].Replace(" ", "").Replace(".", "").Replace("(", "").Replace(")", ""));
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
