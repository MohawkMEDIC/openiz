using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Messaging.IMSI.Util;
using System.Collections.Specialized;
using OpenIZ.Core.Model.DataTypes;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Model.Roles;

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

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("mnemonic", "ENT");
            var expr = builder.BuildLinqExpression<Concept>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test tht building of a simple OR method
        /// </summary>
        [TestMethod]
        public void TestBuildSimpleOrLinqMethod()
        {

            Expression<Func<Concept, bool>> expected = (o => o.Mnemonic == "EVN" || o.Mnemonic == "INT");

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("mnemonic", "EVN");
            httpQueryParameters.Add("mnemonic", "INT");
            var expr = builder.BuildLinqExpression<Concept>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }


        /// <summary>
        /// Test tht building of a simple AND method
        /// </summary>
        [TestMethod]
        public void TestBuildSimpleAndLinqMethod()
        {

            
            Expression<Func<SecurityUser, bool>> expected = (o => o.UserName == "Charles" && o.PasswordHash == "20329132");

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("userName", "Charles");
            httpQueryParameters.Add("passwordHash", "20329132");
            var expr = builder.BuildLinqExpression<SecurityUser>(httpQueryParameters);
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

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("userName", "Charles");
            httpQueryParameters.Add("userName", "Charles2");
            httpQueryParameters.Add("passwordHash", "XXX");
            var expr = builder.BuildLinqExpression<SecurityUser>(httpQueryParameters);
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

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("invalidLoginAttempts", "<4");
            var expr = builder.BuildLinqExpression<SecurityUser>(httpQueryParameters);
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

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("name.use.mnemonic", "L");
            var expr = builder.BuildLinqExpression<Patient>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test tht building of a simple AND & OR method
        /// </summary>
        [TestMethod]
        public void TestGuardCondition()
        {

            var dtString = DateTime.Now;
            String expected = "o => o.Names.Where(guard => (guard.NameUse.Mnemonic == \"L\")).Any(name => name.Component.Where(guard => (guard.Type == \"GIV\")).Any(component => (component.Value == \"John\")))";

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("name[L].component[GIV].value", "John");
            var expr = builder.BuildLinqExpression<Patient>(httpQueryParameters);
            Assert.AreEqual(expected, expr.ToString());

        }


    }
}
