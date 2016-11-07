using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents view state information related to the applet
    /// </summary>
    [XmlType(nameof(AppletViewState), Namespace = "http://openiz.org/applet")]
    public class AppletViewState
    {

        /// <summary>
        /// Gets or sets the name of the view state
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the list of routes associated with the applet view
        /// </summary>
        [XmlElement("route")]
        public String Route { get; set; }

        /// <summary>
        /// Gets or sets the view(s) related to the applet view state
        /// </summary>
        [XmlElement("view")]
        public List<AppletView> View { get; set; }

        /// <summary>
        /// Indicates the view is abstract
        /// </summary>
        [XmlAttribute("abstract")]
        public bool IsAbstract { get; set; }
    }
}