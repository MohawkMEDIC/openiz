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

        // Schemes
        public const string BASE_SCHEME = "app://openiz.org/";
        public const string APPLET_SCHEME = BASE_SCHEME + "applet/";
        public const string ASSET_SCHEME = BASE_SCHEME + "asset/";
        public const string DRAWABLE_SCHEME = BASE_SCHEME + "drawable/";
        // XMLNS stuff
        private readonly XNamespace xs_xhtml = "http://www.w3.org/1999/xhtml";
        private readonly XNamespace xs_binding = "http://openiz.org/applet/binding";
        private readonly RenderBundle m_defaultBundle = new RenderBundle(String.Empty, 
            new ScriptBundleContent("app://openiz.org/asset/js/openiz.js"), 
            new ScriptBundleContent("app://openiz.org/asset/js/openiz-model.js")
        );

        // Override schemes
        private string m_appletBase = APPLET_SCHEME;
        private string m_assetBase = ASSET_SCHEME;
        private string m_drawableBase = DRAWABLE_SCHEME;


        // Reference bundles
        private List<RenderBundle> m_referenceBundles = new List<RenderBundle>()
        {
            new RenderBundle(RenderBundle.BUNDLE_JQUERY, new ScriptBundleContent("app://openiz.org/asset/js/jquery.min.js"), new ScriptBundleContent("app://openiz.org/asset/js/jquery.mobile.min.js")),
            new RenderBundle(RenderBundle.BUNDLE_BOOTSTRAP, new ScriptBundleContent("app://openiz.org/asset/js/bootstrap.js"), new StyleBundleContent("app://openiz.org/asset/css/bootstrap.css")),
            new RenderBundle(RenderBundle.BUNDLE_ANGULAR, 
                new ScriptBundleContent("app://openiz.org/asset/js/angular.min.js"), 
                new ScriptBundleContent("app://openiz.org/asset/js/openiz-localize.js")),
            new RenderBundle(RenderBundle.BUNDLE_SELECT2, new StyleBundleContent("app://openiz.org/asset/css/select2.min.css"), new ScriptBundleContent("app://openiz.org/asset/js/select2.min.js")),

        };

        /// <summary>
        /// Constructs a new instance of the applet collection
        /// </summary>
        public AppletCollection()
        {
        }

        // Applet manifest
        private List<AppletManifest> m_appletManifest = new List<AppletManifest>();

        /// <summary>
        /// The asset base to re-write to
        /// </summary>
        public String AssetBase
        {
            get { return this.m_assetBase; }
            set
            {
                this.m_assetBase = value;
                lock (s_syncLock)
                    s_cache.Clear();
            }
        }

        /// <summary>
        /// The applet base to re-write to
        /// </summary>
        public String AppletBase {
            get { return this.m_appletBase; }
            set
            {
                this.m_appletBase = value;
                lock (s_syncLock)
                    s_cache.Clear();
            }
        }

        /// <summary>
        /// The drawable base to rewrite to
        /// </summary>
        public String DrawableBase {
            get { return this.m_drawableBase; }
            set
            {
                this.m_drawableBase = value;
                lock (s_syncLock)
                    s_cache.Clear();
            }
        }

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

            String targetApplet = assetPath, path = String.Empty;

            // Query? We want to remove those
            if (targetApplet.Contains("?"))
                targetApplet = targetApplet.Substring(0, targetApplet.IndexOf("?"));
            if (targetApplet.Contains("#"))
                targetApplet = targetApplet.Substring(0, targetApplet.IndexOf("#"));
            if (targetApplet.StartsWith("/"))
                targetApplet = targetApplet.Substring(1);
            // Starts with app:// so we can find it in here
            if (assetPath.StartsWith(APPLET_SCHEME))
                targetApplet = targetApplet.Substring(APPLET_SCHEME.Length);
            else if (relative == null) // Relative
                throw new KeyNotFoundException("Unbound relative reference");
            else
            {
                var appletCandidate = relative.Manifest.Assets.Where(o => o.Name == targetApplet);
                if (String.IsNullOrEmpty(language))
                    return appletCandidate.FirstOrDefault();
                else {
                    return appletCandidate.FirstOrDefault(o=>o.Language == (language ?? relative?.Language)) ??
                        appletCandidate.FirstOrDefault();
                }
            }

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
            var candidates = this.m_appletManifest.SingleOrDefault(o => o.Info.Id == targetApplet)?.Assets.Where(o => o.Name == path);
            if (String.IsNullOrEmpty(language))
                return candidates?.FirstOrDefault();
            else
            {
                return candidates?.FirstOrDefault(o => o.Language == (language ?? relative?.Language)) ??
                    candidates?.FirstOrDefault();
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
                        htmlContent = htmlAsset.Html as XElement;
                        break;
                    case "body": // The content is an HTML Body element, we must inject the HTML header
                        {
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
                            {
                                var incAsset = this.ResolveAsset(itm, asset);
                                if (incAsset != null)
                                    headerInjection.AddRange(new ScriptBundleContent(String.Format("{0}{1}/{2}", APPLET_SCHEME, incAsset.Manifest.Info.Id, incAsset.Name)).HeaderElement);
                                else if (itm.StartsWith(ASSET_SCHEME))
                                    headerInjection.AddRange(new ScriptBundleContent(itm).HeaderElement);
                                else
                                    throw new FileNotFoundException(String.Format("Asset {0} not found", itm));
                            }
                            foreach (var itm in htmlAsset.Style)
                            {
                                var incAsset = this.ResolveAsset(itm, asset);
                                if (incAsset != null)
                                    headerInjection.AddRange(new StyleBundleContent(String.Format("{0}{1}/{2}", APPLET_SCHEME, incAsset.Manifest.Info.Id, incAsset.Name)).HeaderElement);
                                else if (itm.StartsWith(ASSET_SCHEME))
                                    headerInjection.AddRange(new StyleBundleContent(itm).HeaderElement);
                                else
                                    throw new FileNotFoundException(String.Format("Asset {0} not found", itm));
                            }
                            headerInjection.AddRange(m_defaultBundle.Content.SelectMany(o => o.HeaderElement));
                            // Render the bundles
                            var bodyElement = htmlAsset.Html as XElement;

                            htmlContent = new XElement(xs_xhtml + "html", new XAttribute("ng-app", asset.Name), new XElement(xs_xhtml + "head", headerInjection), bodyElement);
                        }
                        break;
                    case "div": // The content is a simple DIV
                    case "table":
                        {
                            if (String.IsNullOrEmpty(htmlAsset.Layout))
                                htmlContent = htmlAsset.Html as XElement;
                            else
                            {

                                // Insert scripts & Styles
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
                                {
                                    var incAsset = this.ResolveAsset(itm, asset);
                                    if (incAsset != null)
                                        headerInjection.AddRange(new ScriptBundleContent(String.Format("{0}{1}/{2}", APPLET_SCHEME, incAsset.Manifest.Info.Id, incAsset.Name)).HeaderElement);
                                    else if(itm.StartsWith(ASSET_SCHEME))
                                        headerInjection.AddRange(new ScriptBundleContent(itm).HeaderElement);
                                    else
                                        throw new FileNotFoundException(String.Format("Asset {0} not found", itm));
                                }
                                foreach (var itm in htmlAsset.Style)
                                {
                                    var incAsset = this.ResolveAsset(itm, asset);
                                    if (incAsset != null)
                                        headerInjection.AddRange(new StyleBundleContent(String.Format("{0}{1}/{2}", APPLET_SCHEME, incAsset.Manifest.Info.Id, incAsset.Name)).HeaderElement);
                                    else if (itm.StartsWith(ASSET_SCHEME))
                                        headerInjection.AddRange(new StyleBundleContent(itm).HeaderElement);
                                    else
                                        throw new FileNotFoundException(String.Format("Asset {0} not found", itm));
                                }

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
                foreach(var db in dataBindings)
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
                    if(orderBy != null || orderByDescending != null)
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
                        throw new FileNotFoundException(assetName);
                    using (MemoryStream ms = new MemoryStream(this.RenderAssetContent(includeAsset, preProcessLocalization)))
                    {
                        var xel = XDocument.Load(ms).Elements().First() as XElement;
                        if (xel.Name == xs_xhtml + "html")
                            inc.AddAfterSelf(xel.Element(xs_xhtml + "body").Elements());
                        else
                            inc.AddAfterSelf(xel);
                        inc.Remove();
                    }
                }

                // Re-write URLS
                foreach(var itm in htmlContent.DescendantNodes().OfType<XElement>().SelectMany(o => o.Attributes()).Where(o => o.Value.StartsWith(BASE_SCHEME)))
                    itm.Value = itm.Value.Replace(APPLET_SCHEME, this.AppletBase).Replace(ASSET_SCHEME, this.AssetBase).Replace(DRAWABLE_SCHEME, this.DrawableBase);
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
                        Regex re = new Regex("{{\\s?:?:?'(.*)'\\s?\\|\\s?i18n\\s?}}");
                        var assetString = this.GetStrings(preProcessLocalization);
                        renderBuffer = Encoding.UTF8.GetBytes(re.Replace(sw.ToString(), (m) => assetString.FirstOrDefault(o => o.Key == m.Groups[1].Value).Value ?? m.Groups[1].Value));
                    }
                    else
                        renderBuffer = Encoding.UTF8.GetBytes(sw.ToString());

                    // Add to cache
                    lock(s_syncLock)
                        s_cache.Add(assetPath, renderBuffer);

                    return renderBuffer;
                }

            }
            else
                throw new InvalidOperationException("Unknown asset content type");

            
        }


    }
}
