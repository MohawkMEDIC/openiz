using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Applets;
using OpenIZ.Core.Applets.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AppletCompiler
{
    class Program
    {

        private static readonly XNamespace xs_openiz = "http://openiz.org/applet";

        /// <summary>
        /// The main program
        /// </summary>
        static void Main(string[] args)
        {

            ParameterParser<ConsoleParameters> parser = new ParameterParser<ConsoleParameters>();
            var parameters = parser.Parse(args);

            if (parameters.Help)
            {
                parser.WriteHelp(Console.Out);
                return;
            }

            // First is there a Manifest.xml?
            if (!Path.IsPathRooted(parameters.Source))
                parameters.Source = Path.Combine(Environment.CurrentDirectory, parameters.Source);

            Console.WriteLine("Processing {0}...", parameters.Source);
            String manifestFile = Path.Combine(parameters.Source, "Manifest.xml");
            if (!File.Exists(manifestFile))
                Console.WriteLine("Directory must have Manifest.xml");
            else
            {
                XmlSerializer xsz = new XmlSerializer(typeof(AppletManifest));
                XmlSerializer packXsz = new XmlSerializer(typeof(AppletPackage));
                using (var fs = File.OpenRead(manifestFile))
                {
                    AppletManifest mfst = xsz.Deserialize(fs) as AppletManifest;
                    mfst.Assets.AddRange(ProcessDirectory(parameters.Source, parameters.Source));
                    foreach (var i in mfst.Assets)
                        i.Name = i.Name.Substring(1);
                    using (var ofs = File.Create(parameters.Output ?? "out.xml"))
                        xsz.Serialize(ofs, mfst);

                    var pkg = mfst.CreatePackage();
                    pkg.Meta.Hash = SHA256.Create().ComputeHash(pkg.Manifest);
                    pkg.Meta.PublicKeyToken = pkg.Meta.PublicKeyToken ?? BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", "");
                    if (pkg.Meta.Version.Contains("*"))
                        pkg.Meta.Version = pkg.Meta.Version.Replace("*", DateTime.Now.Subtract(new DateTime(DateTime.Now.Year, 1, 1)).TotalMinutes.ToString("00000"));

                    using (var ofs = File.Create(Path.ChangeExtension(parameters.Output ?? "out.xml", ".pak.raw")))
                        packXsz.Serialize(ofs, pkg);
                    using (var ofs = File.Create(Path.ChangeExtension(parameters.Output ?? "out.xml", ".pak.gz")))
                    using (var gzs = new GZipStream(ofs, CompressionMode.Compress))
                    {
                                                packXsz.Serialize(gzs, pkg);
                    }
                    // Render the build directory


                    var bindir = Path.Combine(Path.GetDirectoryName(parameters.Output), "bin");

                    if (String.IsNullOrEmpty(parameters.Deploy))
                    {
                        if (Directory.Exists(bindir) && parameters.Clean)
                            Directory.Delete(bindir, true);
                        bindir = Path.Combine(bindir, mfst.Info.Id);
                        Directory.CreateDirectory(bindir);
                    }
                    else
                        bindir = parameters.Deploy;

                    AppletCollection ac = new AppletCollection();
                    mfst.Initialize();
                    ac.Add(mfst);

                    foreach (var lang in mfst.Strings)
                    {
                        
                        string wd = Path.Combine(bindir, lang.Language);
                        if (String.IsNullOrEmpty(parameters.Lang))
                            Directory.CreateDirectory(wd);
                        else if (parameters.Lang == lang.Language)
                            wd = bindir;
                        else
                            continue;

                        foreach(var itm in mfst.Assets)
                        {
                            try
                            {
                                String fn = Path.Combine(wd, itm.Name.Replace("/", "\\"));
                                if (!Directory.Exists(Path.GetDirectoryName(fn)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(fn));
                                File.WriteAllBytes(fn, ac.RenderAssetContent(itm, lang.Language));
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("E: {0}: {1} {2}", itm, e.GetType().Name, e.Message);
                            }
                        }
                    }
                }


                

            }
        }

        private static Dictionary<String, String> mime = new Dictionary<string, string>()
        {
            { ".eot", "application/vnd.ms-fontobject" },
            { ".woff", "application/font-woff" },
            { ".woff2", "application/font-woff2" },
            { ".ttf", "application/octet-stream" },
            { ".svg", "image/svg+xml" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".png", "image/png" },
            { ".bmp", "image/bmp" }

        };

        /// <summary>
        /// Process the specified directory
        /// </summary>
        private static IEnumerable<AppletAsset> ProcessDirectory(string source, String path)
        {
            List<AppletAsset> retVal = new List<AppletAsset>();
            foreach(var itm in Directory.GetFiles(source))
            {
                if (Path.GetFileName(itm).ToLower() == "manifest.xml")
                    continue;
                else 
                    switch(Path.GetExtension(itm))
                    {
                        case ".html":
                        case ".htm":
                        case ".xhtml":
                            XElement xe = XElement.Load(itm);

                            // Now we have to iterate throuh and add the asset\
                            AppletAssetHtml htmlAsset = new AppletAssetHtml();
                            htmlAsset.Layout = ResolveName(xe.Attribute(xs_openiz + "layout")?.Value);
                            htmlAsset.Titles = new List<LocaleString>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "title").Select(o=> new LocaleString() { Language = o.Attribute("lang")?.Value, Value = o.Value }));
                            htmlAsset.Bundle = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "bundle").Select(o=> ResolveName(o.Value)));
                            htmlAsset.Script = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "script").Select(o=> ResolveName(o.Value)));
                            htmlAsset.Style = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "style").Select(o => ResolveName(o.Value)));
                            var demand = new List<String>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "demand").Select(o=>o.Value));

                            var includes = xe.DescendantNodes().OfType<XComment>().Where(o => o?.Value?.Trim().StartsWith("#include virtual=\"") == true).ToList();
                            foreach (var inc in includes)
                            {
                                String assetName = inc.Value.Trim().Substring(18); // HACK: Should be a REGEX
                                if (assetName.EndsWith("\""))
                                    assetName = assetName.Substring(0, assetName.Length - 1);
                                if (assetName == "content")
                                    continue;
                                var includeAsset = ResolveName(assetName);
                                inc.AddAfterSelf(new XComment(String.Format("#include virtual=\"{0}\"", includeAsset)));
                                inc.Remove();

                            }

                            var xel = xe.Descendants().OfType<XElement>().Where(o => o.Name.Namespace == xs_openiz).ToList();
                            if(xel != null)
                                foreach (var x in xel)
                                    x.Remove();
                            htmlAsset.Html = xe;
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/html",
                                Content = htmlAsset,
                                Policies = demand
                            });
                            break;
                        case ".css":
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/css",
                                Content = File.ReadAllText(itm)
                            });
                            break;
                        case ".js":
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/javascript",
                                Content = File.ReadAllText(itm)
                            });
                            break;
                        default:
                            string mt = null;
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = mime.TryGetValue(Path.GetExtension(itm), out mt) ? mt : "application/octet-stream",
                                Content = File.ReadAllBytes(itm)
                            });
                            break;

                    }
            }

            // Process sub directories
            foreach (var dir in Directory.GetDirectories(source))
                retVal.AddRange(ProcessDirectory(dir, path));

            return retVal;
        }

        /// <summary>
        /// Resolve the specified applet name
        /// </summary>
        private static String ResolveName(string value)
        {

            return value?.ToLower().Replace("\\","/");
        }
    }
}
