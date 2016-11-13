/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-11-3
 */
using MohawkCollege.Util.Console.Parameters;
using Newtonsoft.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OizDevTool
{
    /// <summary>
    /// View model documentation
    /// </summary>
    public static class JsProxy
    {

        private static Dictionary<Type, JsonObjectAttribute> primitives = new Dictionary<Type, JsonObjectAttribute>()
        {
            { typeof(DateTimeOffset), new JsonObjectAttribute("date") },
            { typeof(DateTimeOffset?), new JsonObjectAttribute("date") },
            { typeof(DateTime), new JsonObjectAttribute("date") },
            { typeof(DateTime?), new JsonObjectAttribute("date") },
            { typeof(String), new JsonObjectAttribute("string") },
            { typeof(Int32), new JsonObjectAttribute("number") },
            { typeof(Int32?), new JsonObjectAttribute("number") },
            { typeof(Decimal), new JsonObjectAttribute("number") },
            { typeof(Decimal?), new JsonObjectAttribute("number") },
            { typeof(byte), new JsonObjectAttribute("byte") },
            { typeof(byte[]), new JsonObjectAttribute("bytea") },
            { typeof(Guid), new JsonObjectAttribute("uuid") },
            { typeof(Guid?), new JsonObjectAttribute("uuid") },
            { typeof(bool), new JsonObjectAttribute("bool") },
            { typeof(bool?), new JsonObjectAttribute("bool") },

        };

        public class ConsoleParameters
        {

            [Parameter("asm")]
            public String AssemblyFile { get; set; }

            [Parameter("xml")]
            public String DocumentationFile { get; set; }

            [Parameter("out")]
            public String Output { get; set; }

            [Parameter("ns")]
            public String Namespace { get; set; }
        }

        /// <summary>
        /// Generate javascript documentation
        /// </summary>
        public static void GenerateProxy(String[] args)
        {

            var parms = new ParameterParser<ConsoleParameters>().Parse(args);

            // First we want to open the output file
            using (TextWriter output = File.CreateText(parms.Output ?? "out.js"))
            {

                // First, we shall read
                using (StreamReader templateReader = new StreamReader(typeof(JsProxy).Assembly.GetManifestResourceStream("OizDevTool.Resources.jsdoc-template.js")))
                    output.Write(templateReader.ReadToEnd());

                // Output namespace
                output.WriteLine("var {0} = {0} || {{", parms.Namespace);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(parms.DocumentationFile ?? Path.ChangeExtension(parms.AssemblyFile, "xml"));

                List<Type> enumerationTypes = new List<Type>();

                foreach (var type in Assembly.LoadFile(parms.AssemblyFile).GetTypes().Where(o => o.GetCustomAttribute<JsonObjectAttribute>() != null))
                    GenerateTypeDocumentation(output, type, xmlDoc, parms, enumerationTypes);
                // Generate type documentation for each of the binding enumerations
                foreach (var typ in enumerationTypes.Distinct())
                    GenerateEnumerationDocumentation(output, typ, xmlDoc, parms);

                using (StreamReader templateReader = new StreamReader(typeof(JsProxy).Assembly.GetManifestResourceStream("OizDevTool.Resources.jsdoc-addlclasses.js")))
                    output.Write(templateReader.ReadToEnd());

                // Output static 
                output.WriteLine("}} // {0}", parms.Namespace);
            }
        }

        /// <summary>
        /// Generate enumeration documentation
        /// </summary>
        private static void GenerateEnumerationDocumentation(TextWriter writer, Type type, XmlDocument xmlDoc, ConsoleParameters parms)
        {
            writer.WriteLine("// {0}", type.AssemblyQualifiedName);
            writer.WriteLine("/**");
            writer.WriteLine(" * @enum {uuid}");
            writer.WriteLine(" * @memberof {0}", parms.Namespace);
            writer.WriteLine(" * @public");
            writer.WriteLine(" * @readonly");
            var jobject = type.GetCustomAttribute<JsonObjectAttribute>();
            if (jobject == null)
                jobject = new JsonObjectAttribute(type.Name);

            // Lookup the summary information
            var typeDoc = xmlDoc.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'T:{0}']", type.FullName));
            if (typeDoc != null)
            {
                if (typeDoc.SelectSingleNode(".//*[local-name() = 'summary']") != null)
                    writer.WriteLine(" * @summary {0}", typeDoc.SelectSingleNode(".//*[local-name() = 'summary']").InnerText.Replace("\r\n", ""));
                if (typeDoc.SelectSingleNode(".//*[local-name() = 'remarks']") != null)
                    writer.WriteLine(" * @description {0}", typeDoc.SelectSingleNode(".//*[local-name() = 'remarks']").InnerText.Replace("\r\n", ""));
                if (typeDoc.SelectSingleNode(".//*[local-name() = 'example']") != null)
                    writer.WriteLine(" * @example {0}", typeDoc.SelectSingleNode(".//*[local-name() = 'example']").InnerText.Replace("\r\n", ""));
            }
            writer.WriteLine(" */");
            writer.WriteLine("{0} : {{ ", jobject.Id);

            // Enumerate fields
            foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                writer.WriteLine("\t/** ");
                writer.Write("\t * ");
                typeDoc = xmlDoc.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'F:{0}.{1}']", fi.DeclaringType.FullName, fi.Name));
                if (typeDoc != null)
                {
                    if (typeDoc.SelectSingleNode(".//*[local-name() = 'summary']") != null)
                        writer.Write(typeDoc.SelectSingleNode(".//*[local-name() = 'summary']").InnerText.Replace("\r\n", ""));
                }
                writer.WriteLine();
                writer.WriteLine("\t */");
                writer.WriteLine("\t{0} : '{1}',", fi.Name, fi.GetValue(null));
            }

            writer.WriteLine("}},  // {0} ", jobject.Id);

        }

        /// <summary>
        /// Generate a javascript "class"
        /// </summary>
        private static void GenerateTypeDocumentation(TextWriter writer, Type type, XmlDocument xmlDoc, ConsoleParameters parms, List<Type> enumerationTypes)
        {

            writer.WriteLine("// {0}", type.AssemblyQualifiedName);
            writer.WriteLine("/**");
            writer.WriteLine(" * @class");
            writer.WriteLine(" * @memberof {0}", parms.Namespace);
            writer.WriteLine(" * @public");
            if (type.IsAbstract)
                writer.WriteLine(" * @abstract");
            var jobject = type.GetCustomAttribute<JsonObjectAttribute>();
            if (type.BaseType != typeof(Object))
                writer.WriteLine(" * @extends {0}.{1}", parms.Namespace, type.BaseType.GetCustomAttribute<JsonObjectAttribute>().Id);

            // Lookup the summary information
            var typeDoc = xmlDoc.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'T:{0}']", type.FullName));
            if (typeDoc != null)
            {
                if (typeDoc.SelectSingleNode(".//*[local-name() = 'summary']") != null)
                    writer.WriteLine(" * @summary {0}", typeDoc.SelectSingleNode(".//*[local-name() = 'summary']").InnerText.Replace("\r\n", ""));
                if (typeDoc.SelectSingleNode(".//*[local-name() = 'remarks']") != null)
                    writer.WriteLine(" * @description {0}", typeDoc.SelectSingleNode(".//*[local-name() = 'remarks']").InnerText.Replace("\r\n", ""));
                if (typeDoc.SelectSingleNode(".//*[local-name() = 'example']") != null)
                    writer.WriteLine(" * @example {0}", typeDoc.SelectSingleNode(".//*[local-name() = 'example']").InnerText.Replace("\r\n", ""));
            }

            List<String> copyCommands = new List<string>();
            // Get all properties and document them
            foreach (var itm in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {

                if (itm.GetCustomAttribute<JsonPropertyAttribute>() == null && itm.GetCustomAttribute<SerializationReferenceAttribute>() == null)
                    continue;

                Type itmType = itm.PropertyType;
                if (itmType.IsGenericType) itmType = itmType.GetGenericArguments()[0];

                var itmJobject = itmType.GetCustomAttribute<JsonObjectAttribute>();
                if (itmJobject == null)
                {
                    if (!primitives.TryGetValue(itmType, out itmJobject))
                        itmJobject = new JsonObjectAttribute(itmType.Name);
                }
                else
                    itmJobject = new JsonObjectAttribute(String.Format("{0}.{1}", parms.Namespace, itmJobject.Id));

                var simpleAtt = itmType.GetCustomAttribute<SimpleValueAttribute>();
                if (simpleAtt != null)
                {
                    var simpleProperty = itmType.GetProperty(simpleAtt.ValueProperty);
                    if (!primitives.TryGetValue(simpleProperty.PropertyType, out itmJobject))
                        itmJobject = new JsonObjectAttribute(simpleProperty.PropertyType.Name);
                }

                var originalType = itmJobject.Id;

                // Is this a classified object? if so then the classifier values act as properties themselves
                var classAttr = itmType.GetCustomAttribute<ClassifierAttribute>();
                if (classAttr != null && itm.PropertyType.IsGenericType)
                {
                    itmJobject = new JsonObjectAttribute("object");
                }

                writer.Write(" * @property {{{0}}} ", itmJobject.Id);
                var jprop = itm.GetCustomAttribute<JsonPropertyAttribute>();
                var redir = itm.GetCustomAttribute<SerializationReferenceAttribute>();
                if (jprop != null)
                {
                    writer.Write(jprop.PropertyName);
                    copyCommands.Add(jprop.PropertyName);
                }
                else if (redir != null)
                {
                    var backingProperty = type.GetProperty(redir.RedirectProperty);
                    jprop = backingProperty.GetCustomAttribute<JsonPropertyAttribute>();
                    writer.Write("{0}Model [Delay loaded from {0}], ", jprop.PropertyName);
                    copyCommands.Add(jprop.PropertyName + "Model");

                }
                else
                {
                    writer.Write(itm.Name + "Model");
                    copyCommands.Add(itm.Name + "Model");

                }

                // Output documentation
                typeDoc = xmlDoc.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'P:{0}.{1}']", itm.DeclaringType.FullName, itm.Name));
                if (typeDoc != null)
                {
                    if (typeDoc.SelectSingleNode(".//*[local-name() = 'summary']") != null)
                        writer.Write(typeDoc.SelectSingleNode(".//*[local-name() = 'summary']").InnerText.Replace("\r\n", ""));
                }

                var bindAttr = itm.GetCustomAttribute<BindingAttribute>();
                if (bindAttr != null)
                {
                    enumerationTypes.Add(bindAttr.Binding);
                    writer.Write("(see: {{@link {0}.{1}}} for values)", parms.Namespace, bindAttr.Binding.Name);
                }
                writer.WriteLine();

                // Classified object? If so we need to clarify how the object is propogated
                if (classAttr != null && itm.PropertyType.IsGenericType)
                {
                    // Does the classifier have a binding
                    var classProperty = itmType.GetProperty(classAttr.ClassifierProperty);
                    if (classProperty.GetCustomAttribute<SerializationReferenceAttribute>() != null)
                        classProperty = itmType.GetProperty(classProperty.GetCustomAttribute<SerializationReferenceAttribute>().RedirectProperty);
                    bindAttr = classProperty.GetCustomAttribute<BindingAttribute>();
                    if (bindAttr != null)
                    {
                        enumerationTypes.Add(bindAttr.Binding);

                        // Binding attribute found so lets enumerate it
                        foreach (var fi in bindAttr.Binding.GetFields(BindingFlags.Public | BindingFlags.Static))
                        {
                            writer.Write(" * @property {{{0}}} {1}.{2} ", originalType, jprop.PropertyName, fi.Name, classProperty.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName);
                            typeDoc = xmlDoc.SelectSingleNode(String.Format("//*[local-name() = 'member'][@name = 'F:{0}.{1}']", fi.DeclaringType.FullName, fi.Name));
                            if (typeDoc != null)
                            {
                                if (typeDoc.SelectSingleNode(".//*[local-name() = 'summary']") != null)
                                    writer.Write(typeDoc.SelectSingleNode(".//*[local-name() = 'summary']").InnerText.Replace("\r\n", ""));
                            }
                            writer.WriteLine();
                        }
                        writer.WriteLine(" * @property {{{0}}} {1}.$other Unclassified", originalType, jprop.PropertyName);

                    }
                    else
                    {
                        writer.Write(" * @property {{{0}}} {1}.{2} ", originalType, jprop.PropertyName, "classifier");
                        writer.Write(" where classifier is from {{@link {0}.{1}}} {2}", parms.Namespace, classProperty.DeclaringType.GetCustomAttribute<JsonObjectAttribute>().Id, classProperty.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName);
                        writer.WriteLine();
                    }
                }
            }
            writer.WriteLine(" * @param {{{0}.{1}}} copyData Copy constructor (if present)", parms.Namespace, jobject.Id);

            writer.WriteLine(" */");
            writer.WriteLine("{0} : function(copyData) {{ ", jobject.Id);

            writer.WriteLine("\tthis.$type = '{0}';", jobject.Id);
            writer.WriteLine("\tif(copyData) {");
            copyCommands.Reverse();
            // Get all properties and document them
            foreach (var itm in copyCommands.Where(o=>o != "$type"))
            {
                writer.WriteLine("\tthis.{0} = copyData.{0};", itm);
            }
            writer.WriteLine("\t}");

            writer.WriteLine("}},  // {0} ", jobject.Id);
        }
    }
}
