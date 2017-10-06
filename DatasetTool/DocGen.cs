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
 * User: fyfej
 * Date: 2017-8-3
 */
using MARC.HI.EHRS.SVC.Core;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenIZ.Core.Model;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.EntityLoader;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using System.ServiceModel.Web;

namespace OizDevTool
{
    /// <summary>
    /// Documentation generator
    /// </summary>
    [Description("Tooling for generating documentation")]
    public static class DocGen
    {
        // XHTML
        public const string NS_HTML = "http://www.w3.org/1999/xhtml";

        /// <summary>
        /// Concept set parameter reference
        /// </summary>
        public class ConceptSetParameters
        {
            /// <summary>
            /// Gets or sets the concept sets
            /// </summary>
            [Parameter("concept-set")]
            [Description("Identifies the concept set(s) for which documentation should be generated")]
            public StringCollection ConceptSet { get; set; }

            /// <summary>
            /// Output
            /// </summary>
            [Parameter("output")]
            [Description("Identifies the output HTML file to write")]
            public String Output { get; set; }
        }

        /// <summary>
        /// Concept set parameter reference
        /// </summary>
        public class ServiceDocParameters
        {
            /// <summary>
            /// Gets or set the assembly
            /// </summary>
            [Parameter("asm")]
            [Description("Identifies the assembly to load from")]
            public string Assembly { get; set; }

            /// <summary>
            /// Output
            /// </summary>
            [Parameter("contract")]
            [Description("Identifies the contract to create")]
            public string Contract { get; set; }

            /// <summary>
            /// Get or sets the output 
            /// </summary>
            [Parameter("output")]
            [Description("Sets the output")]
            public String Output { get; set; }

            /// <summary>
            /// Gets or sets the documentation file
            /// </summary>
            [Parameter("xmlDoc")]
            [Description("Sets the xml documentation file")]
            public String XmlDocFile { get; set; }
        }

        /// <summary>
        /// Schema documentation parameters
        /// </summary>
        public class SchemaDocParameters
        {

            /// <summary>
            /// Gets or sets the schema which should be loaded
            /// </summary>
            [Parameter("asm")]
            [Description("Sets the input assembly")]
            public String Assembly { get; set; }

            /// <summary>
            /// Gets or sets the documentation file
            /// </summary>
            [Parameter("xmlDoc")]
            [Description("Sets the xml documentation file")]
            public String XmlDocFile { get; set; }

            /// <summary>
            /// Gets or sets the output schema
            /// </summary>
            [Parameter("output")]
            [Description("Sets the output file")]
            public String Output { get; set; }

        }

        /// <summary>
        /// Documetnation
        /// </summary>
        private struct SvcDoc
        {
            public MethodInfo Method { get; set; }

            public WebInvokeAttribute Contract { get; set; }

        }

        /// <summary>
        /// Generates documentation for the specified contract
        /// </summary>
        [Description("Generate WCF interface documentation")]
        [ParameterClass(typeof(ServiceDocParameters))]
        public static void WcfDoc(String[] args)
        {
            var parms = new ParameterParser<ServiceDocParameters>().Parse(args);

            Assembly loadedAssembly = Assembly.LoadFile(parms.Assembly);
            var type = loadedAssembly.ExportedTypes.FirstOrDefault(o => o.Name == parms.Contract && o.IsInterface);
            if(type == null)
            {
                Console.WriteLine("Could not find contract: {0}\r\nAvailable Interfaces:", parms.Contract);
                foreach (var v in loadedAssembly.ExportedTypes.Where(o => o.IsInterface).Select(o => o.Name))
                    Console.WriteLine("\t{0}", v);
                return;
            }
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(parms.XmlDocFile ?? Path.ChangeExtension(parms.Assembly, "xml"));

            // Generate documentation
            using (XmlWriter xw = XmlWriter.Create(parms.Output))
            {
                xw.WriteStartElement("html", NS_HTML);
                xw.WriteStartElement("head", NS_HTML);
                xw.WriteElementString("title", NS_HTML, "Service Documentation");
                xw.WriteStartElement("link", NS_HTML);
                xw.WriteAttributeString("rel", "stylesheet");
                xw.WriteAttributeString("type", "text/css");
                xw.WriteAttributeString("href", "bootstrap.css");
                xw.WriteEndElement(); // link
                xw.WriteEndElement(); // head
                xw.WriteStartElement("body");
                xw.WriteElementString("h1", NS_HTML, type.Name);

                xw.WriteStartElement("table", NS_HTML);
                xw.WriteAttributeString("class", "table table-bordered");

                xw.WriteStartElement("thead", NS_HTML);
                xw.WriteStartElement("tr", NS_HTML);
                xw.WriteElementString("th", NS_HTML, "Resource");
                xw.WriteElementString("th", NS_HTML, "Operation");
                xw.WriteElementString("th", NS_HTML, "Description");
                xw.WriteEndElement(); // tr
                xw.WriteEndElement(); // thead

                xw.WriteStartElement("tbody", NS_HTML);

                var description = type.GetMethods().Select(o => new { Method = o, Doc = (object)o.GetCustomAttribute<WebGetAttribute>() ?? o.GetCustomAttribute<WebInvokeAttribute>() });
                var combined = description.Where(o => o.Doc is WebGetAttribute).Select(o => new SvcDoc() { Method = o.Method, Contract = new WebInvokeAttribute() { Method = "GET", UriTemplate = (o.Doc as WebGetAttribute).UriTemplate } }).Union(description.Where(o=>o.Doc is WebInvokeAttribute).Select(o=>new SvcDoc() { Method = o.Method, Contract = o.Doc as WebInvokeAttribute }));

                foreach (var comb in combined.GroupBy(o => o.Contract.UriTemplate).OrderBy(o => o.Key))
                {
                    xw.WriteStartElement("tr", NS_HTML);
                    xw.WriteStartElement("td", NS_HTML);
                    xw.WriteAttributeString("rowspan", comb.Count().ToString());
                    xw.WriteString(comb.Key);
                    xw.WriteEndElement(); // td

                    foreach(var itm in comb)
                    {
                        xw.WriteElementString("td", NS_HTML, itm.Contract.Method);
                        
                        XmlNode typeDoc = xmlDoc.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'M:{0}.{1}']", itm.Method.DeclaringType.FullName, itm.Method.Name));
                        if(typeDoc == null)
                        {
                            typeDoc = xmlDoc.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'M:{0}.{1}({2})']", itm.Method.DeclaringType.FullName, itm.Method.Name, String.Join(",", itm.Method.GetParameters().Select(o=>o.ParameterType.FullName))));
                        }
                        xw.WriteElementString("td", NS_HTML, typeDoc?.InnerText);
                        if (itm.Method != comb.Last().Method)
                        {
                            xw.WriteEndElement(); // tr
                            xw.WriteStartElement("tr", NS_HTML);
                        }
                    }
                    xw.WriteEndElement(); // tr
                }
                xw.WriteEndElement();// tbody
                xw.WriteEndElement(); // table
                xw.WriteEndElement(); // body
                xw.WriteEndElement(); // html
            }
        }

        /// <summary>
        /// Adds documentation to an XML Schema File
        /// </summary>
        [Description("Generate XML Schema documentation")]
        [ParameterClass(typeof(SchemaDocParameters))]
        public static void SchemaDoc(String[] args)
        {
            var docParameters = new ParameterParser<SchemaDocParameters>().Parse(args);

            XmlSchema outputXsd = new XmlSchema();
            XmlSchemas outputSchemas = new XmlSchemas();
            var schemaExporter = new XmlSchemaExporter(outputSchemas);
            var reflectionImporter = new XmlReflectionImporter();

            Assembly loadedAssembly = Assembly.LoadFile(docParameters.Assembly);
            // Load the specified types
            foreach (var t in loadedAssembly.ExportedTypes.Where(o => o.GetCustomAttribute<XmlRootAttribute>() != null))
            {
                var typeMapping = reflectionImporter.ImportTypeMapping(t);
                schemaExporter.ExportTypeMapping(typeMapping);
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(docParameters.XmlDocFile ?? Path.ChangeExtension(docParameters.Assembly, "xml"));

            // Schema set objects
            foreach (XmlSchema itm in outputSchemas)
            {
                foreach (object xtype in itm.Items)
                {
                    if (xtype is XmlSchemaComplexType)
                        CreateAssemblyDoc(xtype as XmlSchemaComplexType, loadedAssembly.ExportedTypes.FirstOrDefault(o => o.GetCustomAttribute<XmlTypeAttribute>()?.TypeName == (xtype as XmlSchemaComplexType).Name), xmlDoc);
                    else if (xtype is XmlSchemaSimpleType)
                        CreateAssemblyDoc(xtype as XmlSchemaSimpleType, loadedAssembly.ExportedTypes.FirstOrDefault(o => o.GetCustomAttribute<XmlTypeAttribute>()?.TypeName == (xtype as XmlSchemaSimpleType).Name), xmlDoc);
                }
            }

            // Schema writer
            using (var xwriter = File.Create(docParameters.Output ?? "out.xsd"))
                foreach (XmlSchema itm in outputSchemas)
                    itm.Write(xwriter);

        }

        /// <summary>
        /// Create assembly documentation for a simple type
        /// </summary>
        private static void CreateAssemblyDoc(XmlSchemaSimpleType xmlSchemaSimpleType, Type type, XmlDocument xmlDoc)
        {
            Console.WriteLine("Generating {0}...", xmlSchemaSimpleType.Name);



        }

        /// <summary>
        /// Create documentation for complex type
        /// </summary>
        private static void CreateAssemblyDoc(XmlSchemaComplexType schemaType, Type type, XmlDocument docComments)
        {
            Console.WriteLine("Generating {0}...", schemaType.Name);

            if (type == null) return;
            var typeDoc = docComments.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'T:{0}']", type.FullName));
            if (typeDoc == null) return;
            else
            {
                if (schemaType.Annotation == null)
                    schemaType.Annotation = new XmlSchemaAnnotation();
                schemaType.Annotation.Items.Add(new XmlSchemaDocumentation()
                {
                    Language = "en",
                    Markup = typeDoc.ChildNodes.OfType<XmlNode>().Select(o => CreateDocNode(o)).ToArray()
                });
            }


            if (schemaType.Particle is XmlSchemaSequence)
                CreateAssemblyDoc(schemaType.Particle as XmlSchemaSequence, type, typeDoc);
            else if(schemaType.ContentModel is XmlSchemaComplexContent)
                CreateAssemblyDoc(schemaType.ContentModel as XmlSchemaComplexContent, type, typeDoc);
           
        }

        /// <summary>
        /// Create assembly doc
        /// </summary>
        private static void CreateAssemblyDoc(XmlSchemaComplexContent complexContext, Type type, XmlNode typeDoc)
        {
            if (complexContext.Content is XmlSchemaComplexContentExtension)
            {
                var cce = complexContext.Content as XmlSchemaComplexContentExtension;
                if (cce.Particle is XmlSchemaSequence)
                    CreateAssemblyDoc(cce.Particle as XmlSchemaSequence, type, typeDoc);
            }
            else if (complexContext.Content is XmlSchemaComplexContentRestriction) {
                var cre = complexContext.Content as XmlSchemaComplexContentExtension;
                if (cre.Particle is XmlSchemaSequence)
                    CreateAssemblyDoc(cre.Particle as XmlSchemaSequence, type, typeDoc);
            }
            else
                System.Diagnostics.Debugger.Break();
        }

  
        /// <summary>
        /// Schema content
        /// </summary>
        private static void CreateAssemblyDoc(XmlSchemaSequence sequence, Type type, XmlNode typeDoc)
        {

            foreach(var itm in sequence.Items)
            {
                if(itm is XmlSchemaElement)
                {
                    var ele = itm as XmlSchemaElement;
                    // Output documentation
                    var propName = type.GetRuntimeProperties().FirstOrDefault(o => o.GetCustomAttributes<XmlElementAttribute>().Any(e => e.ElementName == (itm as XmlSchemaElement).Name))?.Name;

                    var propDoc = typeDoc.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'P:{0}.{1}']", type.FullName, propName));
                    if (propDoc != null)
                    {
                        if (ele.Annotation == null)
                            ele.Annotation = new XmlSchemaAnnotation();
                        ele.Annotation.Items.Add(new XmlSchemaDocumentation()
                        {
                            Language = "en",
                            Markup = propDoc.ChildNodes.OfType<XmlNode>().Select(o => CreateDocNode(o)).ToArray()
                        });
                    }

                }
            }
        }

        /// <summary>
        /// Create documentation node
        /// </summary>
        private static XmlNode CreateDocNode(XmlNode sourceNode, XmlDocument context = null)
        {
            XmlDocument doc = context ?? new XmlDocument();

            if (sourceNode is XmlText)
                return doc.CreateTextNode(sourceNode.InnerText);
            else if (sourceNode is XmlElement)
            {
                return doc.CreateTextNode(sourceNode.InnerText);
            }
            else
                return doc.CreateTextNode(sourceNode.InnerText);
        }

    /// <summary>
    /// Generate a concept set documentation file
    /// </summary>
    [Description("Generates documentation for a concept set")]
    [ParameterClass(typeof(ConceptSetParameters))]
    public static void ConceptSet(String[] args)
    {
        ConceptSetParameters parms = new ParameterParser<ConceptSetParameters>().Parse(args);

        ApplicationServiceContext.Current = ApplicationContext.Current;
        //cp.Repository = new SeederProtocolRepositoryService();
        ApplicationContext.Current.Start();
        EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());
        AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
        // Concept sets
        var conceptRepository = ApplicationContext.Current.GetService<IConceptRepositoryService>();
        IEnumerable<ConceptSet> conceptSets = new List<ConceptSet>();
        if (parms.ConceptSet?.Count > 0)
            foreach (var itm in parms.ConceptSet)
                conceptSets = conceptSets.Union(conceptRepository.FindConceptSets(o => o.Mnemonic == itm));
        else
            conceptSets = conceptRepository.FindConceptSets(o => o.CreationTime > DateTimeOffset.MinValue);

        conceptSets = conceptSets.OrderBy(o => o.Name);

        // Generate documentation
        using (XmlWriter xw = XmlWriter.Create(parms.Output))
        {
            xw.WriteStartElement("html", NS_HTML);
            xw.WriteStartElement("head", NS_HTML);
            xw.WriteElementString("title", NS_HTML, "Concept Set Documentation");
            xw.WriteStartElement("link", NS_HTML);
            xw.WriteAttributeString("rel", "stylesheet");
            xw.WriteAttributeString("type", "text/css");
            xw.WriteAttributeString("href", "bootstrap.css");
            xw.WriteEndElement(); // link
            xw.WriteEndElement(); // head
            xw.WriteStartElement("body");
            xw.WriteElementString("h1", NS_HTML, "OpenIZ Concept Set Documentation");

            xw.WriteElementString("h2", NS_HTML, "Table of Contents");

            xw.WriteStartElement("ul", NS_HTML);
            xw.WriteStartElement("li", NS_HTML);
            xw.WriteString("Concept Sets");
            xw.WriteStartElement("ul", NS_HTML);
            foreach (var itm in conceptSets)
            {
                // Output concept set header
                xw.WriteStartElement("li", NS_HTML);
                xw.WriteStartElement("a", NS_HTML);
                xw.WriteAttributeString("href", $"#{itm.Mnemonic}");
                xw.WriteString(itm.Name);
                xw.WriteEndElement(); // a
                xw.WriteEndElement(); // li
            }
            xw.WriteEndElement(); // ul
            xw.WriteEndElement(); // li
            xw.WriteStartElement("li", NS_HTML);

            xw.WriteString("Code Systems");
            xw.WriteStartElement("ul", NS_HTML);
            foreach (var itm in conceptRepository.FindCodeSystems(o => o.CreationTime > DateTimeOffset.MinValue))
            {
                // Output concept set header
                xw.WriteStartElement("li", NS_HTML);
                xw.WriteStartElement("a", NS_HTML);
                xw.WriteAttributeString("href", $"#{itm.Authority}");
                xw.WriteString(itm.Name ?? itm.Authority);
                xw.WriteEndElement(); // a
                xw.WriteEndElement(); // li
            }

            xw.WriteEndElement(); // ul
            xw.WriteEndElement(); // li
            xw.WriteEndElement(); // ul

            xw.WriteElementString("h2", NS_HTML, "Concept Sets");
            // Write contents
            foreach (var itm in conceptSets)
            {
                xw.WriteStartElement("a", NS_HTML);
                xw.WriteAttributeString("name", itm.Mnemonic);
                xw.WriteElementString("h3", NS_HTML, itm.Name);
                xw.WriteEndElement(); // a

                xw.WriteStartElement("p", NS_HTML);
                xw.WriteElementString("strong", NS_HTML, "Definition URL: ");
                xw.WriteString(itm.Url);
                xw.WriteStartElement("br", NS_HTML);
                xw.WriteEndElement(); // br
                xw.WriteElementString("strong", NS_HTML, "OID: ");
                xw.WriteString(itm.Oid);
                xw.WriteStartElement("br", NS_HTML);
                xw.WriteEndElement(); // br
                xw.WriteElementString("strong", NS_HTML, "Mnemonic: ");
                xw.WriteString(itm.Mnemonic);
                xw.WriteEndElement(); // p

                xw.WriteStartElement("table", NS_HTML);
                xw.WriteAttributeString("class", "table table-bordered");
                xw.WriteStartElement("thead", NS_HTML);
                xw.WriteStartElement("tr", NS_HTML);
                xw.WriteElementString("th", NS_HTML, "Mnemonic");
                xw.WriteElementString("th", NS_HTML, "Status");
                xw.WriteElementString("th", NS_HTML, "Name");
                xw.WriteElementString("th", NS_HTML, "Reference Term(s)");
                xw.WriteEndElement(); //tr
                xw.WriteEndElement(); //thead
                xw.WriteStartElement("tbody", NS_HTML);

                // Load the concepts
                foreach (var concept in conceptRepository.FindConcepts(o => o.ConceptSets.Any(c => c.Key == itm.Key)).OrderBy(o => o.Mnemonic))
                {
                    xw.WriteStartElement("tr", NS_HTML);
                    xw.WriteElementString("td", NS_HTML, concept.Mnemonic);
                    xw.WriteElementString("td", NS_HTML, concept.LoadProperty<Concept>("StatusConcept")?.Mnemonic);
                    xw.WriteStartElement("td", NS_HTML);
                    foreach (var name in concept.LoadCollection<ConceptName>("ConceptNames"))
                    {
                        xw.WriteString(name.Name + " ");
                        xw.WriteElementString("strong", NS_HTML, $"({name.Language})");
                        xw.WriteStartElement("br", NS_HTML);
                        xw.WriteEndElement(); // br
                    }
                    xw.WriteEndElement(); // td

                    xw.WriteStartElement("td", NS_HTML);
                    foreach (var cref in concept.LoadCollection<ConceptReferenceTerm>("ReferenceTerms"))
                    {
                        var refTerm = cref.LoadProperty<ReferenceTerm>("ReferenceTerm");
                        var codeSystem = refTerm.LoadProperty<CodeSystem>("CodeSystem");

                        xw.WriteString(refTerm.Mnemonic + " ");

                        xw.WriteStartElement("a", NS_HTML);
                        xw.WriteAttributeString("href", $"#{codeSystem.Authority}");
                        xw.WriteString($"({codeSystem.Name ?? codeSystem.Authority})");
                        xw.WriteEndElement(); // a
                        xw.WriteStartElement("br", NS_HTML);
                        xw.WriteEndElement(); // br
                    }
                    xw.WriteEndElement(); // td

                    xw.WriteEndElement(); // tr
                }

                xw.WriteEndElement(); // tbody
                xw.WriteEndElement(); // table
            }


            xw.WriteElementString("h2", NS_HTML, "Code Systems");
            // Write contents
            foreach (var itm in conceptRepository.FindCodeSystems(o => o.CreationTime > DateTimeOffset.MinValue).OrderBy(o => o.Authority))
            {
                xw.WriteStartElement("a", NS_HTML);
                xw.WriteAttributeString("name", itm.Authority);
                xw.WriteElementString("h3", NS_HTML, itm.Name ?? itm.Authority);
                xw.WriteEndElement(); // a

                xw.WriteStartElement("p", NS_HTML);
                xw.WriteElementString("strong", NS_HTML, "Definition URL: ");
                xw.WriteString(itm.Url);
                xw.WriteStartElement("br", NS_HTML);
                xw.WriteEndElement(); // br
                xw.WriteElementString("strong", NS_HTML, "OID: ");
                xw.WriteString(itm.Oid);
                xw.WriteStartElement("br", NS_HTML);
                xw.WriteEndElement(); // br
                xw.WriteElementString("strong", NS_HTML, "Description: ");
                xw.WriteString(itm.Description);
                xw.WriteStartElement("br", NS_HTML);
                xw.WriteEndElement(); // br
                xw.WriteElementString("strong", NS_HTML, "Version: ");
                xw.WriteString(itm.VersionText);
                xw.WriteEndElement(); // p

                xw.WriteStartElement("table", NS_HTML);
                xw.WriteAttributeString("class", "table table-bordered");
                xw.WriteStartElement("thead", NS_HTML);
                xw.WriteStartElement("tr", NS_HTML);
                xw.WriteElementString("th", NS_HTML, "Mnemonic");
                xw.WriteElementString("th", NS_HTML, "Name");
                xw.WriteEndElement(); //tr
                xw.WriteEndElement(); //thead
                xw.WriteStartElement("tbody", NS_HTML);

                // Load the concepts
                foreach (var concept in conceptRepository.FindReferenceTerms(o => o.CodeSystemKey == itm.Key).OrderBy(o => o.Mnemonic))
                {
                    xw.WriteStartElement("tr", NS_HTML);
                    xw.WriteElementString("td", NS_HTML, concept.Mnemonic);
                    xw.WriteStartElement("td", NS_HTML);
                    var displayNames = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTermName>>().Query(o => o.SourceEntityKey == concept.Key, AuthenticationContext.Current.Principal);
                    if (displayNames != null)
                        foreach (var name in displayNames)
                        {
                            xw.WriteString(name.Name + " ");
                            xw.WriteElementString("strong", NS_HTML, $"({name.Language})");
                            xw.WriteStartElement("br", NS_HTML);
                            xw.WriteEndElement(); // br
                        }
                    xw.WriteEndElement(); // td
                    xw.WriteEndElement(); // tr
                }

                xw.WriteEndElement(); // tbody
                xw.WriteEndElement(); // table
            }
            xw.WriteEndElement(); // body
            xw.WriteEndElement(); // html
        }
    }
}
}
