using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents a single view in the applet view state
    /// </summary>
    [XmlType(nameof(AppletView), Namespace = "http://openiz.org/applet")]
    public class AppletView : AppletViewState
    {
        
        /// <summary>
        /// Gets or sets the list of titles 
        /// </summary>
        [XmlElement("title")]
        public List<LocaleString> Title { get; set; }

        /// <summary>
        /// Gets or sets the controller for the object
        /// </summary>
        [XmlElement("controller")]
        public string Controller { get; set; }

    }
}