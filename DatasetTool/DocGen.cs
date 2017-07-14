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
            using (XmlWriter xw = XmlWriter.Create(parms.Output)) {
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
                foreach (var itm in conceptRepository.FindCodeSystems(o=>o.CreationTime > DateTimeOffset.MinValue))
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
                    foreach(var concept in conceptRepository.FindConcepts(o=>o.ConceptSets.Any(c=>c.Key == itm.Key)).OrderBy(o=>o.Mnemonic))
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

                            xw.WriteString(refTerm.Mnemonic + " " );

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
                        if(displayNames != null)
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
