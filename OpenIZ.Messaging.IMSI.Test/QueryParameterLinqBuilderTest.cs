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
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Messaging.IMSI.Util;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.DataTypes;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Messaging.IMSI.Test
{
    [TestClass]
    public class QueryParameterLinqBuilderTest
    {
        /// <summary>
        /// Test the building of a simple LINQ expression
        /// </summary>
        [TestMethod]
        public void TestBuildSimpleLinqMethod()
        {

            Expression<Func<Concept, bool>> expected = (o=>o.Mnemonic == "ENT");

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("mnemonic", "ENT");
            var expr = QueryExpressionParser.BuildLinqExpression<Concept>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test tht building of a simple OR method
        /// </summary>
        [TestMethod]
        public void TestBuildSimpleOrLinqMethod()
        {

            Expression<Func<Concept, bool>> expected = (o => o.Mnemonic == "EVN" || o.Mnemonic == "INT");

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("mnemonic", "EVN");
            httpQueryParameters.Add("mnemonic", "INT");
            var expr = QueryExpressionParser.BuildLinqExpression<Concept>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test tht building of a fuzzy date match
        /// </summary>
        [TestMethod]
        public void TestBuildFuzzyDate()
        {

            String expected = "o => (((o.DateOfBirth.Value >= 2015-01-01 12:00:00 AM) AndAlso (o.DateOfBirth.Value <= 2015-12-31 11:59:59 PM)) == True)";

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("dateOfBirth", "~2015");
            var expr = QueryExpressionParser.BuildLinqExpression<Patient>(httpQueryParameters);
            //Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test tht building of a simple AND method
        /// </summary>
        [TestMethod]
        public void TestBuildSimpleAndLinqMethod()
        {

            
            Expression<Func<SecurityUser, bool>> expected = (o => o.UserName == "Charles" && o.PasswordHash == "20329132");

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("userName", "Charles");
            httpQueryParameters.Add("passwordHash", "20329132");
            var expr = QueryExpressionParser.BuildLinqExpression<SecurityUser>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }


        /// <summary>
        /// Test tht building of a simple AND & OR method
        /// </summary>
        [TestMethod]
        public void TestBuildSimpleAndOrLinqMethod()
        {

            var dtString = DateTime.Now;
            Expression<Func<SecurityUser, bool>> expected = (o => (o.UserName == "Charles" || o.UserName == "Charles2") && o.PasswordHash == "XXX");

          
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("userName", "Charles");
            httpQueryParameters.Add("userName", "Charles2");
            httpQueryParameters.Add("passwordHash", "XXX");
            var expr = QueryExpressionParser.BuildLinqExpression<SecurityUser>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test tht building of a simple AND & OR method
        /// </summary>
        [TestMethod]
        public void TestLessThanCreation()
        {

            var dtString = DateTime.Now;
            Expression<Func<SecurityUser, bool>> expected = (o => o.InvalidLoginAttempts < 4);

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("invalidLoginAttempts", "<4");
            var expr = QueryExpressionParser.BuildLinqExpression<SecurityUser>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test tht building of a simple AND & OR method
        /// </summary>
        [TestMethod]
        public void TestAnyCondition()
        {

            var dtString = DateTime.Now;
            Expression<Func<Patient, bool>> expected = (o => o.Names.Any(name => name.NameUse.Mnemonic == "L"));

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("name.use.mnemonic", "L");
            var expr = QueryExpressionParser.BuildLinqExpression<Patient>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test query by entity identifier
        /// </summary>
        [TestMethod]
        public void TestEntityIdentifierChain()
        {
            var dtString = DateTime.Now;
            Expression<Func<Entity, bool>> expected = (o => o.Identifiers.Any(identifier => identifier.Authority.Oid == "1.2.3.4" && identifier.Value == "123"));

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("identifier.authority.oid", "1.2.3.4");
            httpQueryParameters.Add("identifier.value", "123");
            var expr = QueryExpressionParser.BuildLinqExpression<Patient>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());
        }

        /// <summary>
        /// Test tht building of a simple AND & OR method
        /// </summary>
        [TestMethod]
        public void TestGuardCondition()
        {

            var dtString = DateTime.Now;
            String expected = "o => o.Names.Where(guard => (guard.NameUse.Mnemonic == \"L\")).Any(name => name.Component.Where(guard => (guard.ComponentType.Mnemonic == \"GIV\")).Any(component => (component.Value == \"John\")))";

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("name[L].component[GIV].value", "John");
            var expr = QueryExpressionParser.BuildLinqExpression<Patient>(httpQueryParameters);
            Assert.AreEqual(expected, expr.ToString());

        }

        /// <summary>
        /// Test tht building of a simple AND & OR method
        /// </summary>
        [TestMethod]
        public void TestGuardAndCondition()
        {

            var dtString = DateTime.Now;
            String expected = "o => o.Names.Where(guard => (guard.NameUse.Mnemonic == \"L\")).Any(name => (name.Component.Where(guard => (guard.ComponentType.Mnemonic == \"GIV\")).Any(component => (component.Value == \"John\")) AndAlso name.Component.Where(guard => (guard.ComponentType.Mnemonic == \"FAM\")).Any(component => (component.Value == \"Smith\"))))";

            
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("name[L].component[GIV].value", "John");
            httpQueryParameters.Add("name[L].component[FAM].value", "Smith");
            var expr = QueryExpressionParser.BuildLinqExpression<Patient>(httpQueryParameters);
            Assert.AreEqual(expected, expr.ToString());

        }


    }
}
