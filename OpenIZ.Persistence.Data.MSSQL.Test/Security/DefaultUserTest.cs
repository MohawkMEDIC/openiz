/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using System.Linq;
using OpenIZ.Core.Security;
using System.Security.Principal;
using System.Security;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using OpenIZ.Core.Security.Attribute;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Security
{
    /// <summary>
    /// This series of tests ensures that the built in users exist and that the default 
    /// permission set is enforced properly
    /// </summary>
    [TestClass]
    public class DefaultUserTest : DataTest
    {
        /// <summary>
        /// Test that user ANONYMOUS exists
        /// </summary>
        [TestMethod]
        public void TestAnonymousUserExists()
        {

            var idService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var identity = idService.GetIdentity("ANONYMOUS");
            Assert.IsNotNull(identity);
            
        }

        /// <summary>
        /// Test that user SYSTEM exists
        /// </summary>
        [TestMethod]
        public void TestSystemUserExists()
        {
            
            var idService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var identity = idService.GetIdentity("SYSTEM");
            Assert.IsNotNull(identity);

        }

        /// <summary>
        /// Attempt to login the anonymous user
        /// </summary>
        [TestMethod]
        public void TestAnonymousDenyLogin()
        {

            var idService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pdpService = ApplicationContext.Current.GetService<IPolicyDecisionService>();
            var pipService = ApplicationContext.Current.GetService<IPolicyInformationService>();

            var identity = idService.GetIdentity("ANONYMOUS");

            // Get the policies for identity
            var policies = pipService.GetActivePolicies(identity);
            Assert.IsTrue(policies.Any(o => o.Policy.Oid == PermissionPolicyIdentifiers.Login), "User does not carry LOGIN policy");
            Assert.AreEqual(policies.SingleOrDefault(o => o.Policy.Oid == PermissionPolicyIdentifiers.Login).Rule, PolicyDecisionOutcomeType.Deny);

            // Does the PDP enforce this?
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.Login), PolicyDecisionOutcomeType.Deny);

            // Does PP enforce this?
            try {
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.Login, new GenericPrincipal(identity, null)).Demand();
                Assert.Fail();
            }
            catch(PolicyViolationException e)
            {
                Assert.AreEqual(PermissionPolicyIdentifiers.Login, e.PolicyId);
            }
            catch(Exception e)
            {
                Assert.Fail("Expected Policy Violation");
            }
        }

        /// <summary>
        /// Test that user SYSTEM is denied login
        /// </summary>
        [TestMethod]
        public void TestSystemDenyLogin()
        {

            var idService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pdpService = ApplicationContext.Current.GetService<IPolicyDecisionService>();
            var pipService = ApplicationContext.Current.GetService<IPolicyInformationService>();

            var identity = idService.GetIdentity("SYSTEM");

            // Get the policies for identity
            var policies = pipService.GetActivePolicies(identity);
            Assert.IsTrue(policies.Any(o => o.Policy.Oid == PermissionPolicyIdentifiers.Login), "User does not carry LOGIN policy");
            Assert.AreEqual(policies.SingleOrDefault(o => o.Policy.Oid == PermissionPolicyIdentifiers.Login).Rule, PolicyDecisionOutcomeType.Deny);

            // Does the PDP enforce this?
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.Login), PolicyDecisionOutcomeType.Deny);
            // Does PP enforce this?
            try
            {
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.Login, new GenericPrincipal(identity, null)).Demand();
                Assert.Fail();
            }
            catch (PolicyViolationException e)
            {
                Assert.AreEqual(PermissionPolicyIdentifiers.Login, e.PolicyId);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected Policy Violation");
            }

        }

        /// <summary>
        /// Test ANONYMOUS is denied access to all clinical functions
        /// </summary>
        [TestMethod]
        public void TestAnonymousDenyAllClinical()
        {
            var idService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pdpService = ApplicationContext.Current.GetService<IPolicyDecisionService>();

            var identity = idService.GetIdentity("ANONYMOUS");

            // Get the policies for identity
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.UnrestrictedClinicalData), PolicyDecisionOutcomeType.Deny);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.WriteClinicalData), PolicyDecisionOutcomeType.Deny);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.DeleteClinicalData), PolicyDecisionOutcomeType.Deny);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.ReadClinicalData), PolicyDecisionOutcomeType.Deny);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.QueryClinicalData), PolicyDecisionOutcomeType.Deny);

        }

        /// <summary>
        /// Test ANONYMOUS is denied all Administrative functions
        /// </summary>
        [TestMethod]
        public void TestAnonymousDenyAllAdministration()
        {
            var idService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pdpService = ApplicationContext.Current.GetService<IPolicyDecisionService>();

            var identity = idService.GetIdentity("ANONYMOUS");

            // Get the policies for identity
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.UnrestrictedAdministration), PolicyDecisionOutcomeType.Deny);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.AlterRoles), PolicyDecisionOutcomeType.Deny);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.CreateRoles), PolicyDecisionOutcomeType.Deny);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.CreateIdentity), PolicyDecisionOutcomeType.Deny);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.ChangePassword), PolicyDecisionOutcomeType.Deny);

        }

        /// <summary>
        /// Test user SYSTEM is granted all access to administrative functions
        /// </summary>
        [TestMethod]
        public void TestSystemGrantAllAdministrative()
        {
            var idService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pdpService = ApplicationContext.Current.GetService<IPolicyDecisionService>();

            var identity = idService.GetIdentity("SYSTEM");

            // Get the policies for identity
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.UnrestrictedAdministration), PolicyDecisionOutcomeType.Grant);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.AlterRoles), PolicyDecisionOutcomeType.Grant);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.CreateRoles), PolicyDecisionOutcomeType.Grant);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.CreateIdentity), PolicyDecisionOutcomeType.Grant);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.ChangePassword), PolicyDecisionOutcomeType.Grant);

        }
        
        /// <summary>
        /// Test that SYSTEM is granted access to all clinical functions
        /// </summary>
        [TestMethod]
        public void TestSystemGrantAllClinical()
        {
            var idService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pdpService = ApplicationContext.Current.GetService<IPolicyDecisionService>();

            var identity = idService.GetIdentity("SYSTEM");

            // Get the policies for identity
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.UnrestrictedClinicalData), PolicyDecisionOutcomeType.Grant);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.WriteClinicalData), PolicyDecisionOutcomeType.Grant);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.ReadClinicalData), PolicyDecisionOutcomeType.Grant);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.DeleteClinicalData), PolicyDecisionOutcomeType.Grant);
            Assert.AreEqual(pdpService.GetPolicyOutcome(new GenericPrincipal(identity, null), PermissionPolicyIdentifiers.QueryClinicalData), PolicyDecisionOutcomeType.Grant);

        }
    }
}
