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

namespace DatasetTool
{
    /// <summary>
    /// View model documentation
    /// </summary>
    public static class ViewModelDoc
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
        public static void GenerateJsDoc(String[] args)
        {

            var parms = new ParameterParser<ConsoleParameters>().Parse(args);

            // First we want to open the output file
            using (TextWriter output = File.CreateText(parms.Output ?? "out.js"))
            {

                // First, we shall read
                using (StreamReader templateReader = new StreamReader(typeof(ViewModelDoc).Assembly.GetManifestResourceStream("DatasetTool.Resources.jsdoc-template.js")))
                    output.Write(templateReader.ReadToEnd());

                // Output namespace
                output.WriteLine("var {0} = {0} || {{", parms.Namespace);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(parms.DocumentationFile ?? Path.ChangeExtension(parms.AssemblyFile, "xml"));

                foreach (var type in Assembly.LoadFile(parms.AssemblyFile).GetTypes().Where(o => o.GetCustomAttribute<JsonObjectAttribute>() != null ))
                {

                    GenerateTypeDocumentation(output, type, xmlDoc, parms);
                }
                output.WriteLine("}} // {0}", parms.Namespace);
            }
        }

        /// <summary>
        /// Generate a javascript "class"
        /// </summary>
        private static void GenerateTypeDocumentation(TextWriter writer, Type type, XmlDocument xmlDoc, ConsoleParameters parms)
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
                    itmJobject.Id = String.Format("{0}.{1}", parms.Namespace, itmJobject.Id);

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
                        writer.Write(typeDoc.SelectSingleNode(".//*[local-name() = 'summary']").InnerText.Replace("\r\n",""));
                }
                writer.WriteLine();
            }
            writer.WriteLine(" * @param {{{0}.{1}}} copyData Copy constructor (if present)", parms.Namespace, jobject.Id);

            writer.WriteLine(" */");
            writer.WriteLine("{0} : function(copyData) {{ ", jobject.Id);

            writer.WriteLine("\tif(copyData) {");
            // Get all properties and document them
            foreach (var itm in copyCommands)
            {
                writer.WriteLine("\tthis.{0} = copyData.{0};", itm);
            }
            writer.WriteLine("\t}");

            writer.WriteLine("}},  // {0} ", jobject.Id);
        }
    }
}
