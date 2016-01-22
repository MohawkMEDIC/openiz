using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using OpenIZ.Messaging.IMSI.Wcf.Compression;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;

namespace OpenIZ.Messaging.IMSI.Wcf
{
    /// <summary>
    /// Represents an IMSI message inspector which can inspect messages and perform tertiary functions
    /// not included in WCF (such as compression)
    /// </summary>
    public class ImsiMessageInspector : IDispatchMessageInspector
    {
        // Trace source
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.IMSI");

        /// <summary>
        /// Get the message's classified format
        /// </summary>
        private WebContentFormat GetContentFormat(Message message)
        {
            WebContentFormat retVal = WebContentFormat.Default;
            if (message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name))
            {
                WebBodyFormatMessageProperty propertyValue = message.Properties[WebBodyFormatMessageProperty.Name] as WebBodyFormatMessageProperty;
                retVal = propertyValue.Format;
            }
            return retVal;
        }

        /// <summary>
        /// After receiving a request, do any message stuff here
        /// </summary>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            try
            {
                // Handle compressed requests
                var compressionScheme = CompressionUtil.GetCompressionScheme(WebOperationContext.Current.IncomingRequest.Headers[System.Net.HttpRequestHeader.ContentEncoding]);
                if (compressionScheme != null)
                    CompressionUtil.DeCompressMessage(ref request, compressionScheme, this.GetContentFormat(request));
                return null;
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Before sending a reply
        /// </summary>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {

            try
            {
                string encodings = WebOperationContext.Current.IncomingRequest.Headers.Get("Accept-Encoding");
                string compressionScheme = String.Empty;

                if (!string.IsNullOrEmpty(encodings))
                {
                    encodings = encodings.ToLowerInvariant();

                    if (encodings.Contains("gzip"))
                        compressionScheme = "gzip";
                    else if (encodings.Contains("deflate"))
                        compressionScheme = "deflate";
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add("X-CompressResponseStream", "no-known-accept");
                }


                // CORS
                // TODO: Add a configuration option to disable this
                Dictionary<String, String> requiredHeaders = new Dictionary<string, string>() {
                {"Access-Control-Allow-Origin", "*"},
                {"Access-Control-Request-Method", "GET,POST,OPTIONS"},
                {"Access-Control-Allow-Headers", "X-Requested-With,Content-Type"}
            };
                foreach (var kv in requiredHeaders)
                    if (!WebOperationContext.Current.OutgoingResponse.Headers.AllKeys.Contains(kv.Key))
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(kv.Key, kv.Value);

                // Finally compress
                // Compress
                if (!String.IsNullOrEmpty(compressionScheme))
                {
                    try
                    {
                        WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Encoding", compressionScheme);
                        WebOperationContext.Current.OutgoingResponse.Headers.Add("X-CompressResponseStream", compressionScheme);

                        using (MemoryStream ms = new MemoryStream())
                        {

                            // Write out the XML
                            using (var xdr = XmlDictionaryWriter.CreateTextWriter(ms, Encoding.UTF8, false))
                                reply.WriteBodyContents(xdr);

                            // Hack: If it is RAW we want to skip the <Binary> tags
                            var messageContent = ms.ToArray();
                            if(this.GetContentFormat(reply) == WebContentFormat.Raw) // Skip binary 
                            {
                                int headerLength = Encoding.UTF8.GetByteCount("<Binary>"),
                                    footerLength = Encoding.UTF8.GetByteCount("</Binary>");
                                //byte[] binArray = new byte[messageContent.Length - headerLength - footerLength];
                                Array.ConstrainedCopy(messageContent, headerLength, messageContent, 0, messageContent.Length - headerLength - footerLength);
                                Array.Resize(ref messageContent, messageContent.Length - headerLength - footerLength);
                                messageContent = Convert.FromBase64String(Encoding.UTF8.GetString(messageContent));
                            }

                            Message compressedMessage = Message.CreateMessage(reply.Version, reply.Headers.Action, new CompressionWriter(messageContent, CompressionUtil.GetCompressionScheme(compressionScheme)));
                            compressedMessage.Properties.CopyProperties(reply.Properties);
                            compressedMessage.Properties.Remove(WebBodyFormatMessageProperty.Name);
                            compressedMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
                            reply = compressedMessage;
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                    }
                }
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
            }
        }
    }
}