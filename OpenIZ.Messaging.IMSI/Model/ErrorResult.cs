using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.IMSI.Model
{
    /// <summary>
    /// Identified data
    /// </summary>
    [XmlType(nameof(ErrorResult), Namespace = "http://openiz.org/imsi")]
    public class ErrorResult : IdentifiedData
    {

        /// <summary>
        /// Represents an error result
        /// </summary>
        public ErrorResult()
        {
            this.Details = new List<ResultDetail>();
        }

        /// <summary>
        /// Gets or sets the details of the result
        /// </summary>
        [XmlElement("detail")]
        public List<ResultDetail> Details { get; set; }

    }
}
