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
 * User: justi
 * Date: 2017-7-4
 */
using OpenIZ.Core.Interop.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Http;
using OpenIZ.Messaging.GS1.Model;
using OpenIZ.Messaging.GS1.Configuration;
using System.Xml.Serialization;
using System.IO;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;

namespace OpenIZ.Messaging.GS1.Transport.AS2
{
    /// <summary>
    /// GS1 service client
    /// </summary>
    public class Gs1ServiceClient : ServiceClientBase
    {

        // Configuration
        private As2ServiceElement m_configuration = (ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.gs1") as Gs1ConfigurationSection)?.Gs1BrokerAddress;

        /// <summary>
        /// Create the GS1 service client
        /// </summary>
        public Gs1ServiceClient(IRestClient restClient) : base(restClient)
        {
            if (!string.IsNullOrEmpty(this.m_configuration.UserName))
                this.Client.Credentials = new HttpBasicCredentials(this.m_configuration.UserName, this.m_configuration.Password);

            this.Client.Accept = this.Client.Accept ?? "application/xml";
            this.m_configuration = this.Client.Description as As2ServiceElement;
        }

        /// <summary>
        /// Issue an order
        /// </summary>
        public void IssueOrder(OrderMessageType orderType)
        {
            String boundary = String.Format("------{0:N}", Guid.NewGuid());
            if (this.m_configuration.UseAS2MimeEncoding)
                this.Client.Post<MultipartAttachment, object>("order", String.Format("multipart/form-data; boundary={0}", boundary), this.CreateAttachment(orderType));
            else
                this.Client.Post<OrderMessageType, object>("order", "application/xml", orderType);
        }

        /// <summary>
        /// Issue an order
        /// </summary>
        public void IssueReceivingAdvice(ReceivingAdviceMessageType advice)
        {
            String boundary = String.Format("------{0:N}", Guid.NewGuid());
            if (this.m_configuration.UseAS2MimeEncoding)
                this.Client.Post<MultipartAttachment, object>("receivingAdvice", String.Format("multipart/form-data; boundary={0}", boundary), this.CreateAttachment(advice));
            else
                this.Client.Post<ReceivingAdviceMessageType, object>("receivingAdvice", "application/xml", advice);
        }

        /// <summary>
        /// Issue an order
        /// </summary>
        public void IssueDespatchAdvice(DespatchAdviceMessageType advice)
        {
            String boundary = String.Format("------{0:N}", Guid.NewGuid());
            if (this.m_configuration.UseAS2MimeEncoding)
                this.Client.Post<MultipartAttachment, object>("despatchAdvice", String.Format("multipart/form-data; boundary={0}", boundary), this.CreateAttachment(advice));
            else
                this.Client.Post<DespatchAdviceMessageType, object>("despatchAdvice", "application/xml", advice);
        }

        /// <summary>
        /// Create an appropriate MIME encoding
        /// </summary>
        private MultipartAttachment CreateAttachment(object content)
        {
            XmlSerializer xsz = new XmlSerializer(content.GetType());
            using (var ms = new MemoryStream())
            {
                xsz.Serialize(ms, content);
                return new MultipartAttachment(ms.ToArray(), "edi/xml", "body", false);
            }
        }
    }
}
