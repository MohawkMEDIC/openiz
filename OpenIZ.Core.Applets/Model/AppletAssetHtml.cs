using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{

    /// <summary>
    /// The tag name
    /// </summary>
    public enum HtmlTagName
    {
        [XmlEnum("http://www.w3.org/1999/xhtml:div")]
        Div,
        [XmlEnum("http://www.w3.org/1999/xhtml:html")]
        Html,
        [XmlEnum("http://www.w3.org/1999/xhtml:body")]
        Body
    }

    /// <summary>
    /// Applet asset XML 
    /// </summary>
    [XmlType(nameof(AppletAssetHtml), Namespace = "http://openiz.org/applet")]
    public class AppletAssetHtml
    {

        /// <summary>
        /// Applet asset html
        /// </summary>
        public AppletAssetHtml()
        {
            this.Bundle = new List<string>();
            this.Script = new List<string>();
            this.Style = new List<string>();
        }

        /// <summary>
        /// Gets or sets the master asset for this asset
        /// </summary>
        [XmlElement("layout")]
        public string Layout { get; set; }

        /// <summary>
        /// Gets or sets the references for the assets
        /// </summary>
        [XmlElement("bundle")]
        public List<String> Bundle { get; set; }

        /// <summary>
        /// Gets or sets the script
        /// </summary>
        [XmlElement("script")]
        public List<String> Script { get; set; }

        /// <summary>
        /// Gets or sets the script
        /// </summary>
        [XmlElement("style")]
        public List<String> Style { get; set; }

        /// <summary>
        /// Content of the element
        /// </summary>
        [XmlElement("body", Type = typeof(XElement), Namespace = "http://www.w3.org/1999/xhtml")]
        [XmlElement("html", Type = typeof(XElement), Namespace = "http://www.w3.org/1999/xhtml")]
        [XmlElement("div", Type = typeof(XElement), Namespace = "http://www.w3.org/1999/xhtml")]
        [XmlChoiceIdentifier("HtmlTag")]
        public XElement Html { get; set; }

        /// <summary>
        /// Tag name
        /// </summary>
        [XmlIgnore]
        public HtmlTagName HtmlTag { get; set; }
    }
}