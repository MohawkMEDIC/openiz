/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */
using OpenIZ.Messaging.GS1.Model;
using SwaggerWcf.Attributes;
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
        [SwaggerWcfPath("Issue Inventory Report", "Builds and returns a detailed inventory report for the specified facilities", ExternalDocsUrl = "https://www.gs1.org/edi-xml/xml-inventory-report/3-3", ExternalDocsDescription = "GS1 BMS XML 3.3 Specification for Inventory Report")]
		LogisticsInventoryReportMessageType IssueInventoryReportRequest(LogisticsInventoryReportRequestMessageType parameters);

        /// <summary>
        /// Represents a request to issue despatch advice
        /// </summary>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/despatchAdvice")]
        [SwaggerWcfPath("Issue Despatch Advice", "Sends a despatch (shipment of goods) to the Open Immunize server", ExternalDocsUrl = "https://www.gs1.org/edi-xml/xml-despatch-advice/3-3", ExternalDocsDescription = "GS1 BMS XML 3.3 Specification for Despatch Advice")]
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
        [SwaggerWcfPath("Issue Order Response", "Allows an external GS1 trading parter to send a response to an order request which originated from Open Immunize", ExternalDocsUrl = "https://www.gs1.org/edi-xml/xml-order-response/3-3", ExternalDocsDescription = "GS1 BMS XML 3.3 Specification for Order Response")]
        void IssueOrderResponse(OrderResponseMessageType orderResponse);
    }
}