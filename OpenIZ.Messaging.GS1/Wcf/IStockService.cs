using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
    }
}
