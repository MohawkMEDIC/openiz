using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Applets.Model;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace OpenIZ.Core.Applets.Test
{
    [TestClass]
    public class TestRenderApplets
    {

        // Applet collection
        private AppletCollection m_appletCollection = new AppletCollection();

        /// <summary>
        /// Initialize the test
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.m_appletCollection.Add(AppletManifest.Load(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.HelloWorldApplet.xml")));
            this.m_appletCollection.Add(AppletManifest.Load(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.SettingsApplet.xml")));
            this.m_appletCollection.Add(AppletManifest.Load(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.LocalizationWithJavascript.xml")));
            this.m_appletCollection.Add(AppletManifest.Load(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.LayoutAngularTest.xml")));

        }

        [TestMethod]
        public void TestCreatePackage()
        {
            var package = this.m_appletCollection[1].CreatePackage();
            Assert.IsNotNull(package);

        }

        [TestMethod]
        public void TestResolveAbsolute()
        {
            Assert.IsNotNull(this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.sample.helloworld/layout"));
        }

        [TestMethod]
        public void TestResolveIndex()
        {
            var asset = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.sample.helloworld");
            Assert.IsNotNull(asset);
            Assert.AreEqual("index", asset.Name);
            Assert.AreEqual("en", asset.Language);
        }

        [TestMethod]
        public void TestResolveLanguage()
        {
            var asset = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.sample.helloworld/index", language: "fr");
            Assert.IsNotNull(asset);
            Assert.AreEqual("index", asset.Name);
            Assert.AreEqual("fr", asset.Language);
        }

        [TestMethod]
        public void TestResolveRelative()
        {
            var asset = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.sample.helloworld/index");
            Assert.IsNotNull(asset);
            Assert.IsNotNull(this.m_appletCollection.ResolveAsset("layout", asset));
        }

        [TestMethod]
        public void TestResolveSettingLanguage()
        {
            var asset = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.applets.core.settings", language: "en");
            Assert.IsNotNull(asset);
        }


        [TestMethod]
        public void TestRenderSettingsHtml()
        {
            var asset = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.applets.core.settings");
            var render = this.m_appletCollection.RenderAssetContent(asset);
            Trace.WriteLine(Encoding.UTF8.GetString(render));
        }

        [TestMethod]
        public void TestRenderHtml()
        {
            var asset = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.sample.helloworld/index");
            var render = this.m_appletCollection.RenderAssetContent(asset);
            Trace.WriteLine(Encoding.UTF8.GetString(render));
        }

        /// <summary>
        /// Test re-write of URLS
        /// </summary>
        [TestMethod]
        public void TestRewriteUrl()
        {
            var coll = new AppletCollection();
            coll.Add(AppletManifest.Load(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.HelloWorldApplet.xml")));
            coll.Add(AppletManifest.Load(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.SettingsApplet.xml")));

            coll.AssetBase = "http://test.com/assets/";
            coll.AppletBase = "http://test.com/applets/";
            var asset = coll.ResolveAsset("app://openiz.org/applet/org.openiz.sample.helloworld/index");
            Assert.IsNotNull(asset);
            var render = coll.RenderAssetContent(asset);
            String renderString = Encoding.UTF8.GetString(render);
            Trace.WriteLine(renderString);
            Assert.IsTrue(renderString.Contains("http://test.com/assets/css/bootstrap.css"));
            Assert.IsTrue(renderString.Contains("http://test.com/applets/org.openiz.sample.helloworld/index-controller"));
        }

        /// <summary>
        /// Test pre-processing of localization
        /// </summary>
        [TestMethod]
        public void TestPreProcessLocalization()
        {
            var asset = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.applet.test.layout/index");
            var render = this.m_appletCollection.RenderAssetContent(asset, "en");

            string html = Encoding.UTF8.GetString(render);
            Assert.IsFalse(html.Contains("{{ 'some_string' | i18n }}"));
            Assert.IsTrue(html.Contains("SOME STRING!"));

        }

        /// <summary>
        /// Test rendering
        /// </summary>
        [TestMethod]
        public void TestLayoutBundleReferences()
        {
            var coll = new AppletCollection();
            coll.Add(AppletManifest.Load(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.LayoutAngularTest.xml")));
            coll.AssetBase = "file:///C:/Users/fyfej/Source/Repos/openizdc/OpenIZMobile/Assets/";
            var path = Path.GetDirectoryName(Path.GetTempFileName());
            coll.AppletBase = "file:///" + path.Replace("\\","/") + "/";

            if (!Directory.Exists(Path.Combine(path, "org.openiz.applet.test.layout")))
                Directory.CreateDirectory(Path.Combine(path, "org.openiz.applet.test.layout"));

            File.WriteAllText(Path.Combine(path, "org.openiz.applet.test.layout", "index-controller"), Encoding.UTF8.GetString(coll.RenderAssetContent(coll.ResolveAsset("app://openiz.org/applet/org.openiz.applet.test.layout/index-controller"))));
            File.WriteAllText(Path.Combine(path, "org.openiz.applet.test.layout", "index-style"), Encoding.UTF8.GetString(coll.RenderAssetContent(coll.ResolveAsset("app://openiz.org/applet/org.openiz.applet.test.layout/index-style"))));
            File.WriteAllText(Path.Combine(path, "org.openiz.applet.test.layout", "layout-style"), Encoding.UTF8.GetString(coll.RenderAssetContent(coll.ResolveAsset("app://openiz.org/applet/org.openiz.applet.test.layout/layout-style"))));
            File.WriteAllText(Path.Combine(path, "org.openiz.applet.test.layout", "layout-controller"), Encoding.UTF8.GetString(coll.RenderAssetContent(coll.ResolveAsset("app://openiz.org/applet/org.openiz.applet.test.layout/layout-controller"))));

            var asset = coll.ResolveAsset("app://openiz.org/applet/org.openiz.applet.test.layout/index");
            var render = coll.RenderAssetContent(asset);
            string html = Encoding.UTF8.GetString(render);
            Assert.IsTrue(html.Contains("index-controller"), "Missing index-controller");
            Assert.IsTrue(html.Contains("layout-controller"), "Missing layout-controller");
            Assert.IsTrue(html.Contains("index-style"), "Missing index-style");
            Assert.IsTrue(html.Contains("layout-controller"), "Missing layout-style");
            Assert.IsTrue(html.Contains("chart"), "Missing chart-js");


        }

        /// <summary>
        /// Test localization in javascript
        /// </summary>
        [TestMethod]
        public void TestLocalizationJavascript()
        {
            var assetEn = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.applet.sample.localization.js/strings", language: "en");
            var assetFr = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.applet.sample.localization.js/strings", language: "fr");
            var assetIndex = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.applet.sample.localization.js/index", language: "fr");

            var render = this.m_appletCollection.RenderAssetContent(assetEn);
            Assert.IsTrue(Encoding.UTF8.GetString(render).Contains("Click Me!"));
            render = this.m_appletCollection.RenderAssetContent(assetFr);
            Assert.IsTrue(Encoding.UTF8.GetString(render).Contains("Cliquez Moi!"));
            render = this.m_appletCollection.RenderAssetContent(assetIndex);
            Assert.IsTrue(Encoding.UTF8.GetString(render).Contains("app://openiz.org/applet/org.openiz.applet.sample.localization.js/strings"));
            Trace.WriteLine(Encoding.UTF8.GetString(render));
        }
    }
}
