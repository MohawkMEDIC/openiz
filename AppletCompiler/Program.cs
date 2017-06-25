/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-2
 */
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Applets;
using OpenIZ.Core.Applets.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        static int Main(string[] args)
        {
            int retVal = 0;
            ParameterParser<ConsoleParameters> parser = new ParameterParser<ConsoleParameters>();
            var parameters = parser.Parse(args);

            if (parameters.Help)
            {
                parser.WriteHelp(Console.Out);
                return 0;
            }
            else if (parameters.Compile)
                return Compile(parameters);
            else if (parameters.Sign)
                return Sign(parameters);
            else
            {
                Console.WriteLine("Nothing to do!");
                return 0;
            }
        }

        /// <summary>
        /// Sign an existing package
        /// </summary>
        private static int Sign(ConsoleParameters parameters)
        {
            try
            {
                AppletPackage pkg = null;
                using (FileStream fs = File.OpenRead(parameters.Source))
                    pkg = AppletPackage.Load(fs);

                Console.WriteLine("Will sign package {0}", pkg.Meta);
                pkg = CreateSignedPackage(pkg.Unpack(), parameters);
                using (FileStream fs = File.Create(parameters.Output ?? Path.ChangeExtension(parameters.Source, ".signed.pak")))
                    pkg.Save(fs);
                return 0;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine("Cannot sign package: {0}", e);
                return -0232;
            }
        }



        /// <summary>
        /// Compile
        /// </summary>
        static int Compile(ConsoleParameters parameters)
        {
            int retVal = 0;
            // First is there a Manifest.xml?
            if (!Path.IsPathRooted(parameters.Source))
                parameters.Source = Path.Combine(Environment.CurrentDirectory, parameters.Source);

            // Applet collection
            AppletCollection ac = new AppletCollection();
            XmlSerializer xsz = new XmlSerializer(typeof(AppletManifest));
            XmlSerializer xpz = new XmlSerializer(typeof(AppletPackage));
            if (parameters.References != null)
                foreach (var itm in parameters.References)
                {
                    if (File.Exists(itm))
                    {
                        using (var fs = File.OpenRead(itm))
                        {
                            if (Path.GetExtension(itm) == ".pak")
                                using (var gzs = new GZipStream(fs, CompressionMode.Decompress))
                                {
                                    var pack = xpz.Deserialize(gzs) as AppletPackage;
                                    var mfst = pack.Unpack();
                                    mfst.Initialize();
                                    ac.Add(mfst);
                                    Console.WriteLine("Added reference to {0}; v={1}", mfst.Info.Id, mfst.Info.Version);
                                }
                            else
                            {
                                var mfst = xsz.Deserialize(fs) as AppletManifest;
                                mfst.Initialize();
                                ac.Add(mfst);
                                Console.WriteLine("Added reference to {0}; v={1}", mfst.Info.Id, mfst.Info.Version);

                            }
                        }
                    }
                }

            Console.WriteLine("Processing {0}...", parameters.Source);
            String manifestFile = Path.Combine(parameters.Source, "Manifest.xml");
            if (!File.Exists(manifestFile))
                Console.WriteLine("Directory must have Manifest.xml");
            else
            {
                Console.WriteLine("\t Reading Manifest...", parameters.Source);

                using (var fs = File.OpenRead(manifestFile))
                {
                    AppletManifest mfst = xsz.Deserialize(fs) as AppletManifest;
                    mfst.Assets.AddRange(ProcessDirectory(parameters.Source, parameters.Source, parameters));
                    foreach (var i in mfst.Assets)
                        i.Name = i.Name.Substring(1);

                    if (mfst.Info.Version.Contains("*"))
                        mfst.Info.Version = mfst.Info.Version.Replace("*", (((DateTime.Now.Subtract(new DateTime(DateTime.Now.Year, 1, 1)).Ticks >> 24) % 10000)).ToString("0000"));

                    if (!Directory.Exists(Path.GetDirectoryName(parameters.Output)) && !String.IsNullOrEmpty(Path.GetDirectoryName(parameters.Output)))
                        Directory.CreateDirectory(Path.GetDirectoryName(parameters.Output));

                    AppletPackage pkg = null;

                    // Is there a signature?
                    if (!String.IsNullOrEmpty(parameters.SignKey))
                    {
                        pkg = CreateSignedPackage(mfst, parameters);
                        if (pkg == null) return -102;
                    }
                    else
                    {
                        Console.WriteLine("WARNING:>>> THIS PACKAGE IS NOT SIGNED - MOST OPEN IZ TOOLS WILL NOT LOAD IT");
                        mfst.Info.PublicKeyToken = null;
                        pkg = mfst.CreatePackage();
                        //pkg.Meta.PublicKeyToken = null;
                    }
                    pkg.Meta.Hash = SHA256.Create().ComputeHash(pkg.Manifest);

                    using (var ofs = File.Create(Path.ChangeExtension(parameters.Output ?? "out.pak", ".pak")))
                    {
                        pkg.Save(ofs);
                    }
                    // Render the build directory

                    if (!String.IsNullOrEmpty(parameters.Deploy))
                    {
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

                            foreach (var m in ac)
                                foreach (var itm in m.Assets)
                                {
                                    try
                                    {
                                        String fn = Path.Combine(wd, m.Info.Id, itm.Name.Replace("/", "\\"));
                                        Console.WriteLine("\tRendering {0}...", fn);
                                        if (!Directory.Exists(Path.GetDirectoryName(fn)))
                                            Directory.CreateDirectory(Path.GetDirectoryName(fn));
                                        File.WriteAllBytes(fn, ac.RenderAssetContent(itm, lang.Language));
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("E: {0}: {1} {2}", itm, e.GetType().Name, e);
                                        retVal = -1000;
                                    }
                                }
                        }
                    }

                }
            }

            return retVal;
        }

        /// <summary>
        /// Create a signed package
        /// </summary>
        private static AppletPackage CreateSignedPackage(AppletManifest mfst, ConsoleParameters parameters)
        {
            try
            {
                if (String.IsNullOrEmpty(parameters.SignPassword))
                {
                    using (var frmKey = new frmKeyPassword(parameters.SignKey))
                        if (frmKey.ShowDialog() == DialogResult.OK)
                            parameters.SignPassword = frmKey.Password;
                }
                else if (File.Exists(parameters.SignPassword))
                    parameters.SignPassword = File.ReadAllText(parameters.SignPassword);

                X509Certificate2 signCert = new X509Certificate2(parameters.SignKey, parameters.SignPassword);

                mfst.Info.PublicKeyToken = signCert.Thumbprint;
                var retVal = mfst.CreatePackage();
                retVal.Meta.Hash = SHA256.Create().ComputeHash(retVal.Manifest);
                retVal.Meta.PublicKeyToken = signCert.Thumbprint;

                if (parameters.EmbedCertificate)
                    retVal.PublicKey = signCert.Export(X509ContentType.Cert);

                if (!signCert.HasPrivateKey)
                    throw new SecurityException($"Provided key {parameters.SignKey} has no private key");
                RSACryptoServiceProvider rsa = signCert.PrivateKey as RSACryptoServiceProvider;
                retVal.Meta.Signature = rsa.SignData(retVal.Manifest, CryptoConfig.MapNameToOID("SHA1"));
                return retVal;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error signing package: {0}", e);
                return null;
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
            { ".bmp", "image/bmp" },
            { ".json", "application/json" }

        };

        /// <summary>
        /// Process the specified directory
        /// </summary>
        private static IEnumerable<AppletAsset> ProcessDirectory(string source, String path, ConsoleParameters parms)
        {
            List<AppletAsset> retVal = new List<AppletAsset>();
            foreach (var itm in Directory.GetFiles(source))
            {
                Console.WriteLine("\t Processing {0}...", itm);

                if (Path.GetFileName(itm).ToLower() == "manifest.xml")
                    continue;
                else
                    switch (Path.GetExtension(itm))
                    {
                        case ".html":
                        case ".htm":
                        case ".xhtml":
                            XElement xe = XElement.Load(itm);

                            // Now we have to iterate throuh and add the asset\
                            AppletAssetHtml htmlAsset = new AppletAssetHtml();
                            htmlAsset.Layout = ResolveName(xe.Attribute(xs_openiz + "layout")?.Value);
                            htmlAsset.Titles = new List<LocaleString>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "title").Select(o => new LocaleString() { Language = o.Attribute("lang")?.Value, Value = o.Value }));
                            htmlAsset.Bundle = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "bundle").Select(o => ResolveName(o.Value)));
                            htmlAsset.Script = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "script").Select(o => ResolveName(o.Value)));
                            htmlAsset.Style = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "style").Select(o => ResolveName(o.Value)));
                            htmlAsset.Static = xe.Attribute(xs_openiz + "static")?.Value == "true";
                            htmlAsset.ViewState = xe.Elements().OfType<XElement>().Where(o => o.Name == xs_openiz + "state").Select(o => new AppletViewState()
                            {
                                Name = o.Attribute("name")?.Value,
                                Route = o.Elements().OfType<XElement>().FirstOrDefault(r => r.Name == xs_openiz + "url" || r.Name == xs_openiz + "route")?.Value,
                                IsAbstract = Boolean.Parse(o.Attribute("abstract")?.Value ?? "False"),
                                View = o.Elements().OfType<XElement>().Where(v => v.Name == xs_openiz + "view")?.Select(v => new AppletView()
                                {
                                    Name = v.Attribute("name")?.Value,
                                    Title = v.Elements().OfType<XElement>().Where(t => t.Name == xs_openiz + "title")?.Select(t => new LocaleString()
                                    {
                                        Language = t.Attribute("lang")?.Value,
                                        Value = t?.Value
                                    }).ToList(),
                                    Controller = v.Element(xs_openiz + "controller")?.Value
                                }).ToList()
                            }).FirstOrDefault();
                            var demand = xe.DescendantNodes().OfType<XElement>().Where(o => o.Name == xs_openiz + "demand").Select(o => o.Value).ToList();

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
                            if (xel != null)
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
                                Content = parms.Optimize && !itm.Contains("rules") ? new Microsoft.Ajax.Utilities.Minifier().MinifyJavaScript(File.ReadAllText(itm), new Microsoft.Ajax.Utilities.CodeSettings() { MinifyCode = false, StripDebugStatements = true, LocalRenaming = Microsoft.Ajax.Utilities.LocalRenaming.KeepAll, PreserveFunctionNames = true }) : File.ReadAllText(itm)
                            });
                            break;
                        case ".json":
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "application/json",
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
                if (!Path.GetFileName(dir).StartsWith("."))
                    retVal.AddRange(ProcessDirectory(dir, path, parms));
                else
                    Console.WriteLine("Skipping directory {0}", dir);

            return retVal;
        }

        /// <summary>
        /// Resolve the specified applet name
        /// </summary>
        private static String ResolveName(string value)
        {

            return value?.ToLower().Replace("\\", "/");
        }
    }
}
