using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using System.Linq;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Model.Security;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.ADO.Test
{
    [TestClass]
    public class AdoPolicyProviderTest : DataTest
    {

        /// <summary>
        /// Test that the service gets all policies
        /// </summary>
        [TestMethod]
        public void TestGetAllPolicies()
        {

            var ipoli = ApplicationContext.Current.GetService<IPolicyInformationService>();
            Assert.IsNotNull(ipoli);
            var policies = ipoli.GetPolicies();
            Assert.AreNotEqual(0, policies.Count());

            // Assert that default policies
            foreach(var fi in typeof(PermissionPolicyIdentifiers).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                var fiv = fi.GetValue(null);
                Assert.IsTrue(policies.Any(o => o.Oid == fiv.ToString()), $"Missing policy {fiv}");
            }
        }

        /// <summary>
        /// Test that the service gets all policies
        /// </summary>
        [TestMethod]
        public void TestGetRolePolicies()
        {

            var ipoli = ApplicationContext.Current.GetService<IPolicyInformationService>();

            Assert.IsNotNull(ipoli);

            var policies = ipoli.GetActivePolicies(new SecurityRole() { Name = "Administrators", Key = Guid.Parse("f6d2ba1d-5bb5-41e3-b7fb-2ec32418b2e1") });
            Assert.AreNotEqual(0, policies.Count());

        }

        /// <summary>
        /// Test that the service gets all policies
        /// </summary>
        [TestMethod]
        public void TestGetUserPolicies()
        {

            var ipoli = ApplicationContext.Current.GetService<IPolicyInformationService>();

            Assert.IsNotNull(ipoli);

            var policies = ipoli.GetActivePolicies(new GenericIdentity("system"));
            Assert.AreNotEqual(0, policies.Count());

        }

    }
}
