using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Applets.Model;
using System.Diagnostics;
using System.Text;

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
