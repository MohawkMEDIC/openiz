using OpenIZ.Core.Applets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace OpenIZ.Core.Applets
{
    /// <summary>
    /// Represents a collection of applets
    /// </summary>
    public class AppletCollection : IList<AppletManifest>
    {

        public const string APPLET_SCHEME = "app://openiz.org/applet/";
        public const string ASSET_SCHEME = "app://openiz.org/asset/";
        public const string DRAWABLE_SCHEME = "app://openiz.org/drawable/";
        private readonly XNamespace xs_xhtml = "http://www.w3.org/1999/xhtml";

        // Reference bundles
        private List<RenderBundle> m_referenceBundles = new List<RenderBundle>();

        // Applet manifest
        private List<AppletManifest> m_appletManifest = new List<AppletManifest>();
        
        /// <summary>
        /// Gets or sets the item at the specified element
        /// </summary>
        public AppletManifest this[int index] { get { return this.m_appletManifest[index]; } set { this.m_appletManifest[index] = value; } }

        /// <summary>
        /// Return the count of applets in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return this.m_appletManifest.Count;
            }
        }

        /// <summary>
        /// Return true if the collection is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>
        /// Add an applet manifest to the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(AppletManifest item)
        {
            this.m_appletManifest.Add(item);
        }

        /// <summary>
        /// Clear the collection of applets
        /// </summary>
        public void Clear()
        {
            this.m_appletManifest.Clear();
        }

        /// <summary>
        /// Returns true if the collection contains the specified item
        /// </summary>
        public bool Contains(AppletManifest item)
        {
            return this.m_appletManifest.Contains(item);
        }

        /// <summary>
        /// Copies the specified collection to the array
        /// </summary>
        public void CopyTo(AppletManifest[] array, int arrayIndex)
        {
            this.m_appletManifest.CopyTo(array, arrayIndex);
        }
        
        /// <summary>
        /// Get the enumerator
        /// </summary>
        public IEnumerator<AppletManifest> GetEnumerator()
        {
            return this.m_appletManifest.GetEnumerator();
        }

        /// <summary>
        /// Get the index of the specified item
        /// </summary>
        public int IndexOf(AppletManifest item)
        {
            return this.m_appletManifest.IndexOf(item);
        }

        /// <summary>
        /// Inserts the specified item at the specified index
        /// </summary>
        public void Insert(int index, AppletManifest item)
        {
            this.m_appletManifest.Insert(index, item);
        }

        /// <summary>
        /// Remove the specified item from the collection
        /// </summary>
        public bool Remove(AppletManifest item)
        {
            return this.m_appletManifest.Remove(item);
        }
        
        /// <summary>
        /// Removes the specified item
        /// </summary>
        public void RemoveAt(int index)
        {
            this.m_appletManifest.RemoveAt(index);
        }

        /// <summary>
        /// Gets the specified enumerator
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.m_appletManifest.GetEnumerator();
        }

        /// <summary>
        /// Register bundle
        /// </summary>
        /// <param name="bundle"></param>
        public void RegisterBundle(RenderBundle bundle)
        {
            this.m_referenceBundles.Add(bundle);
        }

        /// <summary>
        /// Resolve the asset 
        /// </summary>
        public AppletAsset ResolveAsset(String assetPath, AppletAsset relative = null, String language = null) 
        {

            String targetApplet = assetPath, path = String.Empty;

            // Query? We want to remove those
            if (targetApplet.Contains("?"))
                targetApplet = targetApplet.Substring(0, targetApplet.IndexOf("?"));

            // Starts with app:// so we can find it in here
            if (assetPath.StartsWith(APPLET_SCHEME))
                targetApplet = targetApplet.Substring(APPLET_SCHEME.Length);
            else if (relative == null) // Relative
                throw new KeyNotFoundException("Unbound relative reference");
            else
                return relative.Manifest.Assets.SingleOrDefault(o => o.Name == targetApplet && o.Language == (language ?? relative.Language));

            // Page in the target applet
            if (targetApplet.Contains("/"))
            {
                path = targetApplet.Substring(targetApplet.IndexOf("/") + 1);
                if (String.IsNullOrEmpty(path))
                    path = "index";
                targetApplet = targetApplet.Substring(0, targetApplet.IndexOf("/"));
            }
            else
                path = "index";

            // Now we have the target applet, and path, so retrieve
            return this.m_appletManifest.SingleOrDefault(o => o.Info.Id == targetApplet)?.Assets.SingleOrDefault(o => o.Name == path && o.Language == (language ?? relative?.Language));
        }

        /// <summary>
        /// Render asset content
        /// </summary>
        public byte[] RenderAssetContent(AppletAsset asset)
        {
            if (asset.Content is String) // Content is a string
                return Encoding.UTF8.GetBytes(asset.Content as String);
            else if (asset.Content is byte[]) // Content is a binary asset
                return asset.Content as byte[];
            else if (asset.Content is XElement) // Content is XML
            {
                using (MemoryStream ms = new MemoryStream())
                using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings() { Indent = true }))
                {
                    (asset.Content as XElement).WriteTo(xw);
                    xw.Flush();
                    ms.Flush();
                    return ms.ToArray();
                }
            }
            else if (asset.Content is AppletAssetHtml) // Content is HTML
            {
                // Is the content HTML?
                var htmlAsset = asset.Content as AppletAssetHtml;
                XElement htmlContent = null;

                // Type of tag to render basic content
                switch (htmlAsset.HtmlTag)
                {
                    case HtmlTagName.Html: // The content is a complete HTML page
                        htmlContent = htmlAsset.Html as XElement;
                        break;
                    case HtmlTagName.Body: // The content is an HTML Body element, we must inject the HTML header

                        List<XElement> headerInjection = new List<XElement>();
                        // Inject special headers
                        foreach (var itm in htmlAsset.Bundle)
                        {
                            var bundle = this.m_referenceBundles.Find(o => o.Name == itm);
                            if (bundle == null)
                                throw new FileNotFoundException(String.Format("Bundle {0} not found", itm));
                            headerInjection.AddRange(bundle.Content.SelectMany(o => o.HeaderElement));
                        }
                        foreach (var itm in htmlAsset.Script)
                            headerInjection.AddRange(new ScriptBundleContent(itm).HeaderElement);
                        foreach (var itm in htmlAsset.Style)
                            headerInjection.AddRange(new StyleBundleContent(itm).HeaderElement);

                        if (this.m_referenceBundles.Exists(o => o.Name == RenderBundle.BUNDLE_ANGULAR))
                            headerInjection.Add(new XElement(xs_xhtml + "script", new XAttribute("src", asset.Name + "-controller"), new XAttribute("type", "text/javascript"), new XText("// Imported data")));

                        // Render the bundles
                        var bodyElement = htmlAsset.Html as XElement;
                        
                        // Inject the OpenIZ JS shim
                        var openizJS = new XElement(xs_xhtml + "script", new XAttribute("src", "app://openiz.org/asset/js/openiz.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data"));
                        bodyElement.Add(openizJS);

                        htmlContent = new XElement(xs_xhtml + "html", new XAttribute("ng-app", asset.Name), new XElement(xs_xhtml + "head", headerInjection), bodyElement);

                        break;
                    case HtmlTagName.Div: // The content is a simple DIV

                        if (String.IsNullOrEmpty(htmlAsset.Layout))
                            htmlContent = htmlAsset.Html as XElement;
                        else
                        {

                            // TODO: Rewrite the angular JS reference
                            // Get the layout
                            var layoutAsset = this.ResolveAsset(htmlAsset.Layout, asset);
                            if (layoutAsset == null)
                                throw new FileNotFoundException(String.Format("Layout asset {0} not found", htmlAsset.Layout));

                            using (MemoryStream ms = new MemoryStream(this.RenderAssetContent(layoutAsset)))
                                htmlContent = XDocument.Load(ms).Element(xs_xhtml + "html") as XElement;

                            // Find the <!--#include virtual="content" --> tag
                            var contentNode = htmlContent.DescendantNodes().OfType<XComment>().SingleOrDefault(o => o.Value.Trim() == "#include virtual=\"content\"");
                            if (contentNode != null)
                            {
                                contentNode.AddAfterSelf(htmlAsset.Html as XElement);
                                contentNode.Remove();
                            }
                        }

                        break;
                }

                // Now process SSI directives - <!--#include virtual="XXXXXXX" -->
                var includes = htmlContent.DescendantNodes().OfType<XComment>().Where(o => o.Value.StartsWith("#include virtual=\""));
                foreach (var inc in includes)
                {
                    String assetName = inc.Value.Trim().Substring(18); // HACK: Should be a REGEX
                    if (assetName.EndsWith("\""))
                        assetName = assetName.Substring(0, assetName.Length - 1);
                    var includeAsset = this.ResolveAsset(assetName, asset);
                    using (MemoryStream ms = new MemoryStream(this.RenderAssetContent(includeAsset)))
                    {
                        inc.AddAfterSelf(XDocument.Load(ms).Element(xs_xhtml + "html") as XElement);
                        inc.Remove();
                    }
                }

                // Render out the content
                using (MemoryStream ms = new MemoryStream())
                using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings() { Indent = true }))
                {
                    (asset.Content as XElement).WriteTo(xw);
                    xw.Flush();
                    ms.Flush();
                    return ms.ToArray();
                }

            }
            else
                throw new InvalidOperationException("Unknown asset content type");

            
        }


    }
}
