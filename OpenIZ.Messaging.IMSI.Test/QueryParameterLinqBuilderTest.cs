using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Messaging.IMSI.Util;
using System.Collections.Specialized;
using OpenIZ.Core.Model.DataTypes;
using System.Linq.Expressions;

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

            var dtString = DateTime.Now;
            Expression<Func<Concept, bool>> expected = (o => o.Mnemonic == "EVN" && o.CreationTime == dtString);

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("mnemonic", "EVN");
            httpQueryParameters.Add("creationTime", dtString.ToString("o"));
            var expr = builder.BuildLinqExpression<Concept>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }


        /// <summary>
        /// Test tht building of a simple AND & OR method
        /// </summary>
        [TestMethod]
        public void TestBuildSimpleAndOrLinqMethod()
        {

            var dtString = DateTime.Now;
            Expression<Func<Concept, bool>> expected = (o => (o.Mnemonic == "EVN" || o.Mnemonic == "INT") && o.CreationTime == dtString);

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("mnemonic", "EVN");
            httpQueryParameters.Add("mnemonic", "INT");
            httpQueryParameters.Add("creationTime", dtString.ToString("o"));
            var expr = builder.BuildLinqExpression<Concept>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }

        /// <summary>
        /// Test tht building of a simple AND & OR method
        /// </summary>
        [TestMethod]
        public void TestLessThanCreation()
        {

            var dtString = DateTime.Now;
            Expression<Func<Concept, bool>> expected = (o => o.CreationTime <= dtString);

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("creationTime", "<" + dtString.ToString("o"));
            var expr = builder.BuildLinqExpression<Concept>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }



        /// <summary>
        /// Test tht building of a simple by key method
        /// </summary>
        [TestMethod]
        public void TestSimpleKeyLinqExpression()
        {

            var dtString = DateTime.Now;
            Expression<Func<Concept, bool>> expected = (o => o.CreatedByKey == Guid.Empty);

            var builder = new QueryParameterLinqExpressionBuilder();
            NameValueCollection httpQueryParameters = new NameValueCollection();
            httpQueryParameters.Add("createdBy", Guid.Empty.ToString());
            var expr = builder.BuildLinqExpression<Concept>(httpQueryParameters);
            Assert.AreEqual(expected.ToString(), expr.ToString());

        }


    }
}
