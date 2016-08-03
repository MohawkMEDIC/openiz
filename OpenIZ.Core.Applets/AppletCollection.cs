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
using System.Text.RegularExpressions;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using System.Reflection;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Reflection;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Map;
using System.Linq.Expressions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace OpenIZ.Core.Applets
{
    /// <summary>
    /// Represents a collection of applets
    /// </summary>
    public class AppletCollection : IList<AppletManifest>
    {

        // A cache of rendered assets
        private static Dictionary<String, Byte[]> s_cache = new Dictionary<string, byte[]>();
        private static Dictionary<String, List<KeyValuePair<String, String>>> s_stringCache = new Dictionary<string, List<KeyValuePair<string, string>>>();
        private static Object s_syncLock = new object();

        public const string APPLET_SCHEME = "app://";
        private string m_baseUrl = null;

        // XMLNS stuff
        private readonly XNamespace xs_xhtml = "http://www.w3.org/1999/xhtml";
        private readonly XNamespace xs_binding = "http://openiz.org/applet/binding";


        // Reference bundles
        private List<RenderBundle> m_referenceBundles = new List<RenderBundle>()
        {
        };

        /// <summary>
        /// Constructs a new instance of the applet collection
        /// </summary>
        public AppletCollection()
        {
        }

        /// <summary>
        /// Applet collection rewrite to alternate url
        /// </summary>
        public AppletCollection(String baseUrl)
        {
            this.m_baseUrl = baseUrl;
        }

        /// <summary>
        /// The current default scope applet
        /// </summary>
        public AppletManifest DefaultApplet { get; set; }

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
            s_stringCache.Clear();
            
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
        /// Get the list of strings from all the loaded applets
        /// </summary>
        public List<KeyValuePair<String, String>> GetStrings(String locale)
        {
            List<KeyValuePair<String, String>> retVal = null;
            if (!s_stringCache.TryGetValue(locale ?? "", out retVal))
                lock (s_syncLock)
                {
                    retVal = this.m_appletManifest.SelectMany(o => o.Strings).
                        Where(o=>o.Language == locale).
                        SelectMany(o => o.String).
                        Select(o => new KeyValuePair<String, String>(o.Key, o.Value)).ToList();
                    s_stringCache.Add(locale, retVal);
                }
            return retVal;
        }

        /// <summary>
        /// Resolve the asset 
        /// </summary>
        public AppletAsset ResolveAsset(String assetPath, AppletAsset relative = null, String language = null) 
        {

            if (assetPath == null)
                return null;

            // Is the asset start with ~
            if (assetPath.StartsWith("~"))
                assetPath = "/" + relative.Manifest.Info.Id + assetPath.Substring(1);

            Uri path = null;
            if (!Uri.TryCreate(assetPath, UriKind.RelativeOrAbsolute, out path))
                return null;
            else
            {

                AppletManifest resolvedManifest = null;
                String pathLeft = path.IsAbsoluteUri ? path.AbsolutePath.Substring(1) : 
                    path.OriginalString.StartsWith("/") ? path.OriginalString.Substring(1) : path.OriginalString;
                // Is the host specified?
                if (path.IsAbsoluteUri && !String.IsNullOrEmpty(path.Host))
                {

                    resolvedManifest = this.FirstOrDefault(o => o.Info.Id == path.Host);
                }
                else
                {
                    // We can accept /org.x.y.z or /org/x/y/z
                    StringBuilder applId = new StringBuilder();
                    while (pathLeft.Contains("/"))
                    {
                        applId.AppendFormat("{0}.", pathLeft.Substring(0, pathLeft.IndexOf("/")));
                        pathLeft = pathLeft.Substring(pathLeft.IndexOf("/") + 1);
                        resolvedManifest = this.FirstOrDefault(o => o.Info.Id == applId.ToString(0, applId.Length - 1));
                        if (resolvedManifest != null) break;
                    }
                }
                if (resolvedManifest == null) resolvedManifest = relative?.Manifest;

                // Is there a resource?
                if (resolvedManifest != null)
                {
                    if (pathLeft.EndsWith("/") || String.IsNullOrEmpty(pathLeft) )
                        pathLeft += "index.html";
                    return resolvedManifest.Assets.FirstOrDefault(o => o.Name == pathLeft);
                }
                

                return null;
            }
        }

        /// <summary>
        /// Render asset content
        /// </summary>
        public byte[] RenderAssetContent(AppletAsset asset, string preProcessLocalization = null)
        {

            // First, is there an object already
            byte[] cacheObject = null;
            string assetPath = String.Format("{0}?lang={1}", asset.ToString(), preProcessLocalization);
            if (s_cache.TryGetValue(assetPath, out cacheObject))
                return cacheObject;

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
                switch (htmlAsset.Html.Name.LocalName)
                {
                    case "html": // The content is a complete HTML page
                        {
                            var headerInjection = this.GetInjectionHeaders(asset);
                            htmlContent = htmlAsset.Html as XElement;

                            // STRIP - OPENIZJS references
                            var xel = htmlContent.Descendants().OfType<XElement>().Where(o => o.Name == xs_xhtml + "script" && o.Attribute("src")?.Value.Contains("openiz") == true).ToArray();
                            var head = htmlContent.DescendantNodes().OfType<XElement>().FirstOrDefault(o => o.Name == xs_xhtml + "head");
                            if (head == null)
                            {
                                head = new XElement(xs_xhtml + "head");
                                htmlContent.Add(head);
                            }

                            head.Add(headerInjection);
                            break;
                        }
                    case "body": // The content is an HTML Body element, we must inject the HTML header
                        {
                            // Inject special headers
                            var headerInjection = this.GetInjectionHeaders(asset);

                            // Render the bundles
                            var bodyElement = htmlAsset.Html as XElement;

                            htmlContent = new XElement(xs_xhtml + "html", new XAttribute("ng-app", asset.Name), new XElement(xs_xhtml + "head", headerInjection), bodyElement);
                        }
                        break;
                    default:
                        {
                            if (String.IsNullOrEmpty(htmlAsset.Layout))
                                htmlContent = htmlAsset.Html as XElement;
                            else
                            {

                                var headerInjection = this.GetInjectionHeaders(asset);

                                // Get the layout
                                var layoutAsset = this.ResolveAsset(htmlAsset.Layout, asset);
                                if (layoutAsset == null)
                                    throw new FileNotFoundException(String.Format("Layout asset {0} not found", htmlAsset.Layout));

                                using (MemoryStream ms = new MemoryStream(this.RenderAssetContent(layoutAsset, preProcessLocalization)))
                                    htmlContent = XDocument.Load(ms).Element(xs_xhtml + "html") as XElement;

                                (htmlContent.Element(xs_xhtml + "head") as XElement).Add(headerInjection);

                                // Find the <!--#include virtual="content" --> tag
                                var contentNode = htmlContent.DescendantNodes().OfType<XComment>().SingleOrDefault(o => o.Value.Trim() == "#include virtual=\"content\"");
                                if (contentNode != null)
                                {
                                    contentNode.AddAfterSelf(htmlAsset.Html as XElement);
                                    contentNode.Remove();
                                }
                            }
                        }
                        break;
                }

                // Process data bindings
                var dataBindings = htmlContent.DescendantNodes().OfType<XElement>().Where(o => o.Name.LocalName == "select" && o.Attributes().Any(a => a.Name.Namespace == xs_binding));
                foreach (var db in dataBindings)
                {

                    // Get the databinding data
                    XAttribute source = db.Attributes(xs_binding + "source").FirstOrDefault(),
                        filter = db.Attributes(xs_binding + "filter").FirstOrDefault(),
                        key = db.Attributes(xs_binding + "key").FirstOrDefault(),
                        value = db.Attributes(xs_binding + "value").FirstOrDefault(),
                        orderByDescending = db.Attributes(xs_binding + "orderByDescending").FirstOrDefault(),
                        orderBy = db.Attributes(xs_binding + "orderByDescending").FirstOrDefault();


                    if (source == null || filter == null)
                        continue;

                    // First we want to build the filter
                    Type imsiType = typeof(Patient).GetTypeInfo().Assembly.ExportedTypes.FirstOrDefault(o => o.GetTypeInfo().GetCustomAttribute<XmlRootAttribute>()?.ElementName == source.Value);
                    if (imsiType == null)
                        continue;

                    var expressionBuilderMethod = typeof(QueryExpressionParser).GetGenericMethod(nameof(QueryExpressionParser.BuildLinqExpression), new Type[] { imsiType }, new Type[] { typeof(NameValueCollection) });
                    var filterList = NameValueCollection.ParseQueryString(filter.Value);
                    var expr = expressionBuilderMethod.Invoke(null, new object[] { filterList });
                    var filterMethod = typeof(IEntitySourceProvider).GetGenericMethod("Query", new Type[] { imsiType }, new Type[] { expr.GetType() });
                    var dataSource = (filterMethod.Invoke(EntitySource.Current.Provider, new object[] { expr }));

                    // Sort expression
                    if (orderBy != null || orderByDescending != null)
                    {
                        var orderProperty = imsiType.GetRuntimeProperties().FirstOrDefault(o => o.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == (orderBy ?? orderByDescending).Value);
                        ParameterExpression orderExpr = Expression.Parameter(dataSource.GetType());
                        var orderBody = orderExpr.Sort(orderBy?.Value ?? orderByDescending?.Value, orderBy == null ? SortOrderType.OrderByDescending : SortOrderType.OrderBy);
                        dataSource = Expression.Lambda(orderBody, orderExpr).Compile().DynamicInvoke(dataSource);
                    }

                    // Render expression
                    Delegate keyExpression = null, valueExpression = null;
                    ParameterExpression parameter = Expression.Parameter(imsiType);
                    if (key == null)
                        keyExpression = Expression.Lambda(Expression.MakeMemberAccess(parameter, imsiType.GetRuntimeProperty(nameof(IIdentifiedEntity.Key))), parameter).Compile();
                    else
                    {
                        var rawExpr = new BindingExpressionVisitor().RewriteLambda(expressionBuilderMethod.Invoke(null, new object[] { NameValueCollection.ParseQueryString(key.Value + "=RemoveMe") }) as LambdaExpression);
                        keyExpression = Expression.Lambda(new BindingExpressionVisitor().Visit(rawExpr.Body), rawExpr.Parameters).Compile();
                    }
                    if (value == null)
                        valueExpression = Expression.Lambda(Expression.Call(parameter, imsiType.GetRuntimeMethod("ToString", new Type[] { })), parameter).Compile();
                    else
                    {
                        var rawExpr = new BindingExpressionVisitor().RewriteLambda(expressionBuilderMethod.Invoke(null, new object[] { NameValueCollection.ParseQueryString(value.Value + "=RemoveMe") }) as LambdaExpression);
                        valueExpression = Expression.Lambda(rawExpr.Body, rawExpr.Parameters).Compile();
                    }

                    // Creation of the options
                    foreach (var itm in dataSource as IEnumerable)
                    {
                        var optAtt = new XElement(xs_xhtml + "option");
                        optAtt.Add(new XAttribute("value", keyExpression.DynamicInvoke(itm)), new XText(valueExpression.DynamicInvoke(itm)?.ToString()));
                        db.Add(optAtt);
                    }

                }

                // Now process SSI directives - <!--#include virtual="XXXXXXX" -->
                var includes = htmlContent.DescendantNodes().OfType<XComment>().Where(o => o?.Value?.Trim().StartsWith("#include virtual=\"") == true).ToList();
                foreach (var inc in includes)
                {
                    String assetName = inc.Value.Trim().Substring(18); // HACK: Should be a REGEX
                    if (assetName.EndsWith("\""))
                        assetName = assetName.Substring(0, assetName.Length - 1);
                    if (assetName == "content")
                        continue;
                    var includeAsset = this.ResolveAsset(assetName, asset);
                    if (includeAsset == null)
                    {
                        inc.AddAfterSelf(new XElement(xs_xhtml + "strong", new XText(String.Format("{0} NOT FOUND", assetName))));
                        inc.Remove();
                    }
                    else
                        using (MemoryStream ms = new MemoryStream(this.RenderAssetContent(includeAsset, preProcessLocalization)))
                        {
                            var xel = XDocument.Load(ms).Elements().First() as XElement;
                            if (xel.Name == xs_xhtml + "html")
                                inc.AddAfterSelf(xel.Element(xs_xhtml + "body").Elements());
                            else
                            {
                                var headerInjection = this.GetInjectionHeaders(includeAsset);
                                htmlContent.Element(xs_xhtml + "head")?.Add(headerInjection);

                                inc.AddAfterSelf(xel);
                            }
                            inc.Remove();
                        }
                }

                // Re-write
                foreach (var itm in htmlContent.DescendantNodes().OfType<XElement>().SelectMany(o => o.Attributes()).Where(o => o.Value.StartsWith("~")))
                {
                    itm.Value = String.Format("/{0}/{1}", asset.Manifest.Info.Id, itm.Value.Substring(2)); 
                    //itm.Value = itm.Value.Replace(APPLET_SCHEME, this.AppletBase).Replace(ASSET_SCHEME, this.AssetBase).Replace(DRAWABLE_SCHEME, this.DrawableBase);
                }

                // Render Title
                var headTitle = htmlContent.DescendantNodes().OfType<XElement>().FirstOrDefault(o => o.Name == xs_xhtml + "head");
                var title = htmlAsset.GetTitle(preProcessLocalization);
                if (headTitle != null && !String.IsNullOrEmpty(title))
                    headTitle.Add(new XElement(xs_xhtml + "title", new XText(title)));

                // Render out the content
                using (StringWriter sw = new StringWriter())
                using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
                {
                    htmlContent.WriteTo(xw);
                    xw.Flush();

                    byte[] renderBuffer = null;

                    // Process localization
                    if (!String.IsNullOrEmpty(preProcessLocalization))
                    {
                        Regex re = new Regex("{{\\s?:?:?'(.*?)'\\s?\\|\\s?i18n\\s?}}");
                        var assetString = this.GetStrings(preProcessLocalization);
                        renderBuffer = Encoding.UTF8.GetBytes(re.Replace(sw.ToString(), (m) => assetString.FirstOrDefault(o => o.Key == m.Groups[1].Value).Value ?? m.Groups[1].Value));
                    }
                    else
                        renderBuffer = Encoding.UTF8.GetBytes(sw.ToString());

                    // Add to cache
                    lock (s_syncLock)
                        if(!s_cache.ContainsKey(assetPath))
                            s_cache.Add(assetPath, renderBuffer);

                    return renderBuffer;
                }

            }
            else
                return null;

            
        }

        /// <summary>
        /// Rewrte the url
        /// </summary>
        private string RewriteUrl(Uri appletUri)
        {
            Uri rewrite = new Uri(this.m_baseUrl);
            return String.Format("{0}/{1}/{2}", rewrite, appletUri.Host, appletUri.PathAndQuery);

        }

        /// <summary>
        /// Injection for HTML headers
        /// </summary>
        private List<XElement> GetInjectionHeaders(AppletAsset asset)
        {
            var htmlAsset = asset.Content as AppletAssetHtml;
            
            // Insert scripts & Styles
            List<XElement> headerInjection = new List<XElement>();
            if (htmlAsset == null)
                return headerInjection;

            // Inject special headers
            foreach (var itm in htmlAsset.Bundle)
            {
                var bundle = this.m_referenceBundles.Find(o => o.Name == itm);
                if (bundle == null)
                    throw new FileNotFoundException(String.Format("Bundle {0} not found", itm));
                headerInjection.AddRange(bundle.Content.SelectMany(o => o.HeaderElement));
            }
            foreach (var itm in htmlAsset.Script)
            {
                var incAsset = this.ResolveAsset(itm, asset);
                if (incAsset != null)
                    headerInjection.AddRange(new ScriptBundleContent(itm).HeaderElement);
                else
                    throw new FileNotFoundException(String.Format("Asset {0} not found", itm));
            }
            foreach (var itm in htmlAsset.Style)
            {
                var incAsset = this.ResolveAsset(itm, asset);
                if (incAsset != null)
                    headerInjection.AddRange(new StyleBundleContent(itm).HeaderElement);
                else
                    throw new FileNotFoundException(String.Format("Asset {0} not found", itm));
            }
            return headerInjection;
        }
    }
}
