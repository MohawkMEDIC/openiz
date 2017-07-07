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
 * Date: 2016-11-30
 */
using OpenIZ.Messaging.GS1.Model;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.GS1.Wcf
{
	/// <summary>
	/// Stock service request
	/// </summary>
	[ServiceContract(ConfigurationName = "GS1BMS", Name = "GS1BMS")]
	[XmlSerializerFormat]
	public interface IStockService
	{
		/// <summary>
		/// Represents a request for issuance of an inventory report
		/// </summary>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/inventoryReport")]
		LogisticsInventoryReportMessageType IssueInventoryReportRequest(LogisticsInventoryReportRequestMessageType parameters);

        /// <summary>
        /// Represents a request to issue despatch advice
        /// </summary>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/despatchAdvice")]
        void IssueDespatchAdvice(DespatchAdviceMessageType advice);

        /// <summary>
        /// Issue receiving advice to the OpenIZ IMS system
        /// </summary>
        /// TODO: Finish this
        //[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/receivingAdvice")]
        //void IssueReceivingAdvice(ReceivingAdviceMessageType advice);

        /// <summary>
        /// Represents a request to issue an order
        /// </summary>
        /// TODO: Finish this
        //[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/order")]
        //void IssueOrder(OrderMessageType order);

        /// <summary>
        /// Issues order response
        /// </summary>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/orderResponse")]
        void IssueOrderResponse(OrderResponseMessageType orderResponse);
    }
}