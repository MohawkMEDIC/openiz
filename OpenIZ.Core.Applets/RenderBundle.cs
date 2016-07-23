/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-6-14
 */
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
