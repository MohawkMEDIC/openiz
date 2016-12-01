using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI.Model
{
    /// <summary>
    /// Represents a stored query to be performed against the RISI
    /// </summary>
    [XmlType(nameof(ReportDefinition), Namespace = "http://openiz.org/risi")]
    [XmlRoot(nameof(ReportDefinition), Namespace = "http://openiz.org/risi")]
    public class ReportDefinition : BaseEntityData
    {

        /// <summary>
        /// Gets the name of the stored query
        /// </summary>
        [XmlElement("name")]
        public String Name { get; set; }

        /// <summary>
        /// A list of parameters which is supported for the specified query
        /// </summary>
        [XmlElement("parameters")]
        public List<ParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Security policy instances related to the query definition
        /// </summary>
        [XmlElement("policy")]
        public List<SecurityPolicyInstance> Policies { get; set; }

    }
}
