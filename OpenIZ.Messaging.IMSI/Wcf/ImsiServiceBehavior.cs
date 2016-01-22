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

        public Stream GetSchema(int schemaId)
        {
            try
            {
                XsdDataContractExporter exporter = new XsdDataContractExporter();

                List<Type> exportTypes = new List<Type>(typeof(IdentifiedData).Assembly.GetTypes().Where(o => o.GetCustomAttribute<ResourceAttribute>() != null && (o.GetCustomAttribute<ResourceAttribute>().Scope & (ModelScope.Clinical | ModelScope.Concept | ModelScope.Protocol | ModelScope.MetaData)) != (ModelScope)0 && !o.IsGenericTypeDefinition && !o.IsAbstract));
                exportTypes.AddRange(typeof(ImsiServiceBehavior).Assembly.GetTypes().Where(o => o.GetCustomAttribute<ResourceAttribute>()?.Scope == ModelScope.Clinical &&  !o.IsGenericTypeDefinition && !o.IsAbstract));
                exporter.Export(exportTypes);

                var schemas = exporter.Schemas.Schemas().OfType<XmlSchema>().ToArray();
                if (schemaId > schemas.Length)
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return null;
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                    var outStream = new MemoryStream();
                    schemas[schemaId].Write(outStream);
                    outStream.Seek(0, SeekOrigin.Begin);
                    return outStream;
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
