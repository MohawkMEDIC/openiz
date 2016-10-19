using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{

    /// <summary>
    /// Represents model descriptions
    /// </summary>
    [XmlType(nameof(TypeModelDescription), Namespace = "http://openiz.org/model/view")]
    public class TypeModelDescription : PropertyContainerDescription
    {

        /// <summary>
        /// Name of the type to be loaded
        /// </summary>
        [XmlAttribute("type")]
        public string TypeName { get; set; }


    }
}