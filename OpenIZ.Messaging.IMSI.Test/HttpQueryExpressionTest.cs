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
 * Date: 2016-8-2
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.Test
{
    /// <summary>
    /// Tests for the HTTP expression writer
    /// </summary>
    [TestClass]
    public class HttpQueryExpressionTest
    {

        /// <summary>
        /// Turn an array from the builder into a query string
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static String CreateQueryString(params KeyValuePair<String, Object>[] query)
        {
            String queryString = String.Empty;
            foreach (var kv in query)
            {
                queryString += String.Format("{0}={1}", kv.Key, Uri.EscapeDataString(kv.Value.ToString()));
                if (!kv.Equals(query.Last()))
                    queryString += "&";
            }
            return queryString;
        }
        /// <summary>
        /// Test query by key
        /// </summary>
        [TestMethod]
        public void TestWriteLookupByKey()
        {
            Guid id = Guid.Empty;
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.Key == id);
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("id=00000000-0000-0000-0000-000000000000", expression);
        }

        /// <summary>
        /// Test query by key
        /// </summary>
        [TestMethod]
        public void TestChainedParse()
        {
            Guid id = Guid.Empty;
            var qstr = "classConcept.mnemonic=GenderCode&statusConcept.mnemonic=ACTIVE";
            
            var query = QueryExpressionParser.BuildLinqExpression<Place>(NameValueCollection.ParseQueryString(qstr));



        }
        /// <summary>
        /// Test query by key
        /// </summary>
        [TestMethod]
        public void TestChainedWriter()
        {
            Guid id = Guid.Empty;
            var query = QueryExpressionBuilder.BuildQuery<Place>(o => o.ClassConcept.Mnemonic == "GenderCode" && o.StatusConcept.Mnemonic =="ACTIVE");
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("classConcept.mnemonic=GenderCode&statusConcept.mnemonic=ACTIVE", expression);


        }
        /// <summary>
        /// Test query by key
        /// </summary>
        [TestMethod]
        public void TestChainedWriter2()
        {
            Guid id = Guid.Empty;
            var query = QueryExpressionBuilder.BuildQuery<Concept>(o => o.ConceptSets.Any(p=>p.Mnemonic == "GenderCode"));
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("conceptSet.mnemonic=GenderCode", expression);

        }
        /// <summary>
        /// Test query by key
        /// </summary>
        [TestMethod]
        public void TestWriteLookupAnd()
        {
            Guid id = Guid.Empty;
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.Key == id && o.GenderConcept.Mnemonic == "Male");
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("id=00000000-0000-0000-0000-000000000000&genderConcept.mnemonic=Male", expression);
        }

        /// <summary>
        /// Test query by or
        /// </summary>
        [TestMethod]
        public void TestWriteLookupOr()
        {
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.GenderConcept.Mnemonic == "Male" || o.GenderConcept.Mnemonic == "Female");
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("genderConcept.mnemonic=Male&genderConcept.mnemonic=Female", expression);
        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupGreaterThanEqual()
        {
            DateTime dt = DateTime.MinValue;
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.DateOfBirth >= dt);
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("dateOfBirth=%3E%3D0001-01-01T00%3A00%3A00.0000000", expression);

        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupGreaterThan()
        {
            DateTime dt = DateTime.MinValue;
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.DateOfBirth > dt);
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("dateOfBirth=%3E0001-01-01T00%3A00%3A00.0000000", expression);

        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupLessThanEqual()
        {
            DateTime dt = DateTime.MinValue;
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.DateOfBirth <= dt);
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("dateOfBirth=%3C%3D0001-01-01T00%3A00%3A00.0000000", expression);

        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupLessThan()
        {
            DateTime dt = DateTime.MinValue;
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.DateOfBirth < dt);
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("dateOfBirth=%3C0001-01-01T00%3A00%3A00.0000000", expression);

        }
       

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupNotEqual()
        {
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.GenderConcept.Mnemonic != "Male");
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("genderConcept.mnemonic=%21Male", expression);
        }


        /// <summary>
        /// Test write of lookup approximately
        /// </summary>
        [TestMethod]
        public void TestWriteLookupApproximately()
        {
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.GenderConcept.Mnemonic.Contains("M"));
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("genderConcept.mnemonic=~M", expression);
        }

        /// <summary>
        /// Test write of Any correctly
        /// </summary>
        [TestMethod]
        public void TestWriteLookupAny()
        {
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.Names.Any(p=>p.NameUse.Mnemonic == "Legal"));
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("name.use.mnemonic=Legal", expression);
        }

        /// <summary>
        /// Test write of Any correctly
        /// </summary>
        [TestMethod]
        public void TestWriteLookupAnyAnd()
        {
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.Names.Any(p => p.NameUse.Mnemonic == "Legal" && p.Component.Any(n=>n.Value == "Smith")));
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("name.use.mnemonic=Legal&name.component.value=Smith", expression);
        }

        /// <summary>
        /// Test write of Any correctly
        /// </summary>
        [TestMethod]
        public void TestWriteLookupWhereAnd()
        {
            var query = QueryExpressionBuilder.BuildQuery<Patient>(o => o.Names.Where(p => p.NameUse.Mnemonic == "Legal").Any(p => p.Component.Any(n => n.Value == "Smith")));
            var expression = CreateQueryString(query.ToArray());
            Assert.AreEqual("name[Legal].component.value=Smith", expression);
        }
    }
}
