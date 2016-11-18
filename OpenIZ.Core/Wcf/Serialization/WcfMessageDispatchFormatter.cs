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
 * Date: 2016-8-2
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.ServiceModel.Web;
using OpenIZ.Core.Model;
using System.Xml.Schema;
using OpenIZ.Core.Model.Serialization;
using OpenIZ.Core.Security;
using System.Diagnostics;
using OpenIZ.Core.Model.Collection;
using Newtonsoft.Json.Converters;
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Wcf.Serialization
{
    /// <summary>
    /// Represents a dispatch message formatter which uses the JSON.NET serialization
    /// </summary>
    public class WcfMessageDispatchFormatter<TContract> : IDispatchMessageFormatter
    {
        // The operation description
        private OperationDescription m_operationDescription;

        private String m_version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        private String m_versionName = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unnamed";
        // Trace source
        private TraceSource m_traceSource = new TraceSource(OpenIzConstants.WcfTraceSourceName);
        // Known types
        private static Type[] s_knownTypes = typeof(TContract).GetCustomAttributes<ServiceKnownTypeAttribute>().Select(t => t.Type).ToArray();
        // Serializers
        private static Dictionary<Type, XmlSerializer> s_serializers = new Dictionary<Type, XmlSerializer>();

        // Static ctor
        static WcfMessageDispatchFormatter()
        {
            foreach (var s in s_knownTypes)
                s_serializers.Add(s, new XmlSerializer(s,  s.GetCustomAttributes<XmlIncludeAttribute>().Select(o => o.Type).ToArray()));
        }

        public WcfMessageDispatchFormatter()
        {

        }
        /// <summary>
        /// Creates a new json dispatch message formatter
        /// </summary>
        public WcfMessageDispatchFormatter(OperationDescription operationDescription)
        {
            this.m_operationDescription = operationDescription;
        }

        /// <summary>
        /// Deserialize the request
        /// </summary>
        public void DeserializeRequest(Message request, object[] parameters)
        {

            try
            {
#if DEBUG
				RemoteEndpointMessageProperty endpoint = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];

				this.m_traceSource.TraceEvent(TraceEventType.Information, 0, "Received request from: {0}:{1}", endpoint.Address, endpoint.Port);
#endif

				HttpRequestMessageProperty httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                string contentType = httpRequest.Headers[HttpRequestHeader.ContentType];

                UriTemplateMatch templateMatch = (UriTemplateMatch)request.Properties.SingleOrDefault(o => o.Value is UriTemplateMatch).Value;
                // Not found
                if (templateMatch == null)
                {
                    throw new WebFaultException(HttpStatusCode.NotFound);
                }

                for (int pNumber = 0; pNumber < parameters.Length; pNumber++)
                {
                    var parm = this.m_operationDescription.Messages[0].Body.Parts[pNumber];

                    // Simple parameter
                    if (templateMatch.BoundVariables[parm.Name] != null)
                    {
                        var rawData = templateMatch.BoundVariables[parm.Name];
                        parameters[pNumber] = Convert.ChangeType(rawData, parm.Type);
                    }
                    // Use XML Serializer
                    else if (contentType?.StartsWith("application/xml") == true)
                    {
                        XmlDictionaryReader rawReader = request.GetReaderAtBodyContents();
						//rawReader.ReadStartElement("Binary");

						byte[] rawBody = rawReader.ReadContentAsBase64();

						using (rawReader)
						{
							Type eType = s_knownTypes.FirstOrDefault(o => o.GetCustomAttribute<XmlRootAttribute>()?.ElementName == rawReader.LocalName && o.GetCustomAttribute<XmlRootAttribute>()?.Namespace == rawReader.NamespaceURI);

							this.m_traceSource.TraceEvent(TraceEventType.Information, 0, "Contract: {0}", typeof(TContract).Name);
							this.m_traceSource.TraceEvent(TraceEventType.Information, 0, "Attempting to deserialize type: {0}", eType?.Name);

							XmlSerializer xsz = s_serializers[eType];
							parameters[pNumber] = xsz.Deserialize(rawReader);
						}
					}
					// Use JSON Serializer
					else if (contentType?.StartsWith("application/json") == true)
                    {
                        // Read the binary contents form the WCF pipeline
                        XmlDictionaryReader bodyReader = request.GetReaderAtBodyContents();
                        bodyReader.ReadStartElement("Binary");
                        byte[] rawBody = bodyReader.ReadContentAsBase64();

                        // Now read the JSON data
                        MemoryStream ms = new MemoryStream(rawBody);
                        StreamReader sr = new StreamReader(ms);
                        JsonSerializer jsz = new JsonSerializer()
                        {
                            Binder = new ModelSerializationBinder(),
                            TypeNameAssemblyFormat = 0,
                            TypeNameHandling = TypeNameHandling.All
                        };
                        jsz.Converters.Add(new StringEnumConverter());
                        var dserType = parm.Type;
                        parameters[pNumber] = jsz.Deserialize(sr, dserType);
                    }
                    else if (contentType != null)// TODO: Binaries
                        throw new InvalidOperationException("Invalid request format");
                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }

        }

        /// <summary>
        /// Serialize the reply
        /// </summary>
        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            try
            {
                // Outbound control
                var format = WebContentFormat.Raw;
                Message request = OperationContext.Current.RequestContext.RequestMessage;
                HttpRequestMessageProperty httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                string accepts = httpRequest.Headers[HttpRequestHeader.Accept],
                    contentType = httpRequest.Headers[HttpRequestHeader.ContentType];
                Message reply = null;

                // Result is serializable
                if (result?.GetType().GetCustomAttribute<XmlTypeAttribute>() != null ||
                    result?.GetType().GetCustomAttribute<JsonObjectAttribute>() != null)
                {
                    // The request was in JSON or the accept is JSON
                    if (accepts?.StartsWith("application/json") == true ||
                        contentType?.StartsWith("application/json") == true)
                    {
                        // Prepare the serializer
                        JsonSerializer jsz = new JsonSerializer();
                        jsz.Converters.Add(new StringEnumConverter());

                        // Write json data
                        byte[] body = null;
                        using (MemoryStream ms = new MemoryStream())
                        using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                        using (JsonWriter jsw = new JsonTextWriter(sw))
                        {
                            jsz.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                            jsz.NullValueHandling = NullValueHandling.Ignore;
                            jsz.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                            jsz.TypeNameHandling = TypeNameHandling.Auto;
                            jsz.Converters.Add(new StringEnumConverter());
                            jsz.Serialize(jsw, result);
                            sw.Flush();
                            body = ms.ToArray();
                        }

                        // Prepare reply for the WCF pipeline
                        format = WebContentFormat.Raw;
                        contentType = "application/json";
                        reply = Message.CreateMessage(messageVersion, this.m_operationDescription?.Messages[1]?.Action, new RawBodyWriter(body));

                    }
                    // The request was in XML and/or the accept is JSON
                    else
                    {

                        XmlSerializer xsz = s_serializers[result.GetType()];
                        MemoryStream ms = new MemoryStream();
                        xsz.Serialize(ms, result);
                        format = WebContentFormat.Xml;
                        contentType = "application/xml";
                        ms.Seek(0, SeekOrigin.Begin);

                        reply = Message.CreateMessage(messageVersion, this.m_operationDescription?.Messages[1]?.Action, XmlDictionaryReader.Create(ms));
                    }
                }
                else if (result is XmlSchema)
                {
                    MemoryStream ms = new MemoryStream();
                    (result as XmlSchema).Write(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    format = WebContentFormat.Xml;
                    contentType = "text/xml";
                    reply = Message.CreateMessage(messageVersion, this.m_operationDescription.Messages[1].Action, XmlDictionaryReader.Create(ms));
                }
                else if (result is Stream) // TODO: This is messy, clean it up
                {
                    reply = Message.CreateMessage(messageVersion, this.m_operationDescription.Messages[1].Action, new RawBodyWriter(result as Stream));
                }
                else
                    reply = Message.CreateMessage(messageVersion, this.m_operationDescription.Messages[1].Action);

                reply.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(format));
                //var responseProperty = (HttpResponseMessageProperty)OperationContext.Current.OutgoingMessageProperties[HttpResponseMessageProperty.Name];
                //var responseProperty = new HttpResponseMessageProperty();
                WebOperationContext.Current.OutgoingResponse.ContentType= contentType;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("X-PoweredBy", String.Format("OpenIZ {0} ({1})", m_version, m_versionName));
                WebOperationContext.Current.OutgoingResponse.Headers.Add("X-GeneratedOn", DateTime.Now.ToString("o"));
                //reply.Properties.Add(HttpResponseMessageProperty.Name, responseProperty);
                // TODO: Determine best way to clear current authentication context
                AuthenticationContext.Current = null;
                return reply;
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                Message reply = null;
                new WcfErrorHandler().ProvideFault(e, messageVersion, ref reply);
                return reply;
            }
        }
    }
}
