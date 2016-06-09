using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenIZ.Core.Applets
{
    /// <summary>
    /// Represents an asset bundle
    /// </summary>
    public class RenderBundle
    {

        // Mobile meta bundle content
        public const string BUNDLE_JQUERY = "jquery";
        public const string BUNDLE_BOOTSTRAP = "bootstrap";
        public const string BUNDLE_ANGULAR = "angular";
        public const string BUNDLE_METRO = "metro-ui";
        public const string BUNDLE_SELECT2 = "select2";
        public const string BUNDLE_CHART = "chart-js";
        
        /// <summary>
        /// Asset bundle
        /// </summary>
        public RenderBundle(String name)
        {
            this.Content = new List<BundleContent>();
            this.Name = name;
        }

        /// <summary>
        /// Creates asset bundle with specified name and content
        /// </summary>
        public RenderBundle(String name, params BundleContent[] content) : this(name)
        {
            this.Content.AddRange(content);
        }

        /// <summary>
        /// Gets or sets the name of the asset bundle
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the content of the asset bundle
        /// </summary>
        public List<BundleContent> Content { get; set; }

    }

    /// <summary>
    /// Represents bundle contnet
    /// </summary>
    public abstract class BundleContent
    {

		// Namespace 
        protected readonly XNamespace xs_xhtml = "http://www.w3.org/1999/xhtml";

        /// <summary>
        /// Gets the header element
        /// </summary>
        public abstract XElement[] HeaderElement { get; }
    }

    /// <summary>
    /// Represents the necessary bundle content to fix mobile rendering
    /// </summary>
    public class MobileMetaBundleContent : BundleContent
    {
        
        /// <summary>
        /// Render the specified elements
        /// </summary>
        public override XElement[] HeaderElement
        {
            get
            {
                return new XElement[]
                {
                    new XElement (xs_xhtml + "meta", new XAttribute ("content", "true"), new XAttribute ("name", "HandheldFriendly")),
                    new XElement (xs_xhtml + "meta", new XAttribute ("content", "width=640px, initial-scale=0.50, maximum-scale=0.50, minimum-scale=0.50, user-scalable=0"), new XAttribute ("name", "viewport")),
                };
            }
        }
    }

    /// <summary>
    /// Represents a script bundle content
    /// </summary>
    public class ScriptBundleContent : BundleContent
    {

        // The HREF
        private string m_href = null;

        /// <summary>
        /// Creates a new script bundle content
        /// </summary>
        public ScriptBundleContent(String href)
        {
            this.m_href = href;
        }

        /// <summary>
        /// Header element
        /// </summary>
        public override XElement[] HeaderElement
        {
            get
            {
                return new XElement[] { new XElement(xs_xhtml + "script", new XAttribute("src", this.m_href), new XAttribute("type", "text/javascript"), new XText("// Script Reference")) };
            }
        }
    }
    
    /// <summary>
    /// Style bundle content
    /// </summary>
    public class StyleBundleContent : BundleContent
    {
        // The HREF
        private string m_href = null;

        /// <summary>
        /// Creates a new style bundle content
        /// </summary>
        public StyleBundleContent(String href)
        {
            this.m_href = href;
        }

        /// <summary>
        /// Header element
        /// </summary>
        public override XElement[] HeaderElement
        {
            get
            {
                return new XElement[] { new XElement(xs_xhtml + "link", new XAttribute("href", this.m_href), new XAttribute("rel", "stylesheet")) };
            }
        }
    }
}
