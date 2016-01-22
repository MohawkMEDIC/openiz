using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using OpenIZ.Core.Model;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Diagnostics;
using System.ServiceModel.Web;
using System.IO;
using OpenIZ.Core.Model.Attributes;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.IMSI.Wcf
{
    /// <summary>
    /// Data implementation
    /// </summary>
    [ServiceBehavior(ConfigurationName = "IMSI")]
    public class ImsiServiceBehavior : IImsiServiceContract
    {
        // Trace source
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.IMSI");

        public IdentifiedData Create(string resourceType, IdentifiedData body)
        {
            throw new NotImplementedException();
        }

        public IdentifiedData CreateUpdate(string resourceType, string id, IdentifiedData body)
        {
            throw new NotImplementedException();
        }

        public IdentifiedData Get(string resourceType, string id)
        {
            throw new NotImplementedException();
        }

        public IdentifiedData GetVersion(string resourceType, string id, string versionId)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema(int schemaId)
        {
            try
            {
                XmlSchemas schemaCollection = new XmlSchemas();

                XmlReflectionImporter importer = new XmlReflectionImporter("http://openiz.org/model");
                XmlSchemaExporter exporter = new XmlSchemaExporter(schemaCollection);

                foreach (var cls in typeof(IdentifiedData).Assembly.GetTypes().Where(o => o.GetCustomAttribute<XmlRootAttribute>() != null && !o.IsGenericTypeDefinition))
                    exporter.ExportTypeMapping(importer.ImportTypeMapping(cls, "http://openiz.org/model"));

                if (schemaId > schemaCollection.Count)
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return null;
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                    return schemaCollection[schemaId];
                }                
            }
            catch(Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return null;
            }
        }

        public IdentifiedData History(string resourceType, string id)
        {
            throw new NotImplementedException();
        }

        public IdentifiedData Search(string resourceType)
        {
            throw new NotImplementedException();
        }

        public DateTime Time()
        {
            throw new NotImplementedException();
        }

        public IdentifiedData Update(string resourceType, string id, IdentifiedData body)
        {
            throw new NotImplementedException();
        }
    }
}
