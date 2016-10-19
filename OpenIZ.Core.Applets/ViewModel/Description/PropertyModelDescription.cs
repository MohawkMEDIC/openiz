using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// Property model description
    /// </summary>
    [XmlType(nameof(PropertyModelDescription) ,Namespace = "http://openiz.org/model/view")]
    public class PropertyModelDescription : PropertyContainerDescription
    {

        /// <summary>
        /// The property of the model
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or ssets the where classifiers
        /// </summary>
        [XmlAttribute("classifier")]
        public String Classifier { get; set; }


    }
}