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
            this.m_appletCollection.AssetBase = "http://test.com/assets/";
            this.m_appletCollection.AppletBase = "http://test.com/applets/";
            var asset = this.m_appletCollection.ResolveAsset("app://openiz.org/applet/org.openiz.sample.helloworld/index");
            Assert.IsNotNull(asset);
            var render = this.m_appletCollection.RenderAssetContent(asset);
            String renderString = Encoding.UTF8.GetString(render);
            Assert.IsTrue(renderString.Contains("http://test.com/assets/css/bootstrap.css"));
            Assert.IsTrue(renderString.Contains("http://test.com/applets/org.openiz.sample.helloworld/index-controller"));
        }
    }
}
