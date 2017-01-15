using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Persistence.Data.ADO.Util;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using System.Linq;
using System.Diagnostics;
using OpenIZ.Core.Model.Acts;

namespace OpenIZ.Persistence.Data.ADO.Test
{
    [TestClass]
    public class QueryExpressionWriterTest
    {
        /// <summary>
        /// Test that the function constructs an empty select statement
        /// </summary>
        [TestMethod]
        public void TestConstructsEmptySelectStatement()
        {
            SqlStatement sql = new SqlStatement<DbAct>().SelectFrom("foo").Build();
            Assert.IsTrue(sql.SQL.Contains("AS foo"));
            Assert.IsTrue(sql.SQL.Contains("SELECT * FROM act_tbl"));
            Assert.AreEqual(0, sql.Arguments.Count());
        }

        /// <summary>
        /// Tests that the function constructs parameters
        /// </summary>
        [TestMethod]
        public void TestConstructsParameters()
        {
            SqlStatement sql = new SqlStatement<DbAct>().SelectFrom("foo").Where("act_id = ?", Guid.NewGuid()).Build();
            Assert.IsTrue(sql.SQL.Contains("AS foo"));
            Assert.IsTrue(sql.SQL.Contains("SELECT * FROM act_tbl"));
            Assert.AreEqual(1, sql.Arguments.Count());
        }


        /// <summary>
        /// Tests that the function constructs parameters
        /// </summary>
        [TestMethod]
        public void TestConstructLocalParameters()
        {
            SqlStatement sql = new SqlStatement<DbActVersion>().SelectFrom("foo").Where("act_id = ?", Guid.NewGuid()).And("act_utc < ?", DateTime.Now).Build();
            Assert.IsTrue(sql.SQL.Contains("AS foo"));
            Assert.IsTrue(sql.SQL.Contains("AND"));
            Assert.IsTrue(sql.SQL.Contains("act_id"));
            Assert.IsTrue(sql.SQL.Contains("act_utc"));
            Assert.IsTrue(sql.SQL.Contains("SELECT * FROM act_vrsn_tbl"));
            Assert.AreEqual(2, sql.Arguments.Count());
            Assert.IsInstanceOfType(sql.Arguments.First(), typeof(Guid));
            Assert.IsInstanceOfType(sql.Arguments.Last(), typeof(DateTime));
        }

        /// <summary>
        /// Test re-writing of simple LINQ
        /// </summary>
        [TestMethod]
        public void TestRewriteSimpleLinq()
        {
            Guid mg = Guid.NewGuid();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SqlStatement sql = new SqlStatement<DbActVersion>().SelectFrom("foo").Where(o => o.IsNegated == true).Build();
            sw.Stop();
            Assert.IsTrue(sql.SQL.Contains("AS foo"));
            Assert.IsTrue(sql.SQL.Contains("neg_ind =  ?"));
            Assert.AreEqual(1, sql.Arguments.Count());

        }

        /// <summary>
        /// Test re-writing of simple LINQ
        /// </summary>
        [TestMethod]
        public void TestRewriteComplexLinq()
        {
            Guid mg = Guid.NewGuid();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SqlStatement sql = new SqlStatement<DbActVersion>().SelectFrom("foo").Where(o=>o.Key == mg || o.Key == Guid.NewGuid() && o.CreationTime <= DateTime.Now).Build();
            sw.Stop();

            Assert.IsTrue(sql.SQL.Contains("AS foo"));
            Assert.IsTrue(sql.SQL.Contains("AND"));
            Assert.IsTrue(sql.SQL.Contains("act_id"));
            Assert.IsTrue(sql.SQL.Contains("crt_utc"));
            Assert.IsTrue(sql.SQL.Contains("SELECT * FROM act_vrsn_tbl"));
            Assert.AreEqual(1, sql.Arguments.Count());
            Assert.IsInstanceOfType(sql.Arguments.First(), typeof(Guid));
        }

        /// <summary>
        /// Test re
        /// </summary>
        [TestMethod]
        public void TestRewriteQueryParameter()
        {

            var query = QueryBuilder.CreateQuery<Act>(o => o.ActTime == DateTime.Now);


        }

    }
}
