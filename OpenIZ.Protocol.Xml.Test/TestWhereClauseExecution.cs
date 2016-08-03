using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using System.Collections.Generic;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Protocol.Xml.Model;
using System.Linq.Expressions;
using OpenIZ.Protocol.Xml.Model.XmlLinq;

namespace OpenIZ.Protocol.Xml.Test
{
    [TestClass]
    public class TestWhereClauseExecution
    {
        /// <summary>
        /// Test patient 
        /// </summary>
        private Patient m_patientUnderTest = new Patient()
        {
            Key = Guid.NewGuid(),
            VersionKey = Guid.NewGuid(),
            VersionSequence = 1,
            CreatedBy = new Core.Model.Security.SecurityUser()
            {
                Key = Guid.NewGuid(),
                UserName = "bob",
                SecurityHash = Guid.NewGuid().ToString(),
                Email = "bob@bob.com",
                InvalidLoginAttempts = 2,
                UserClass = UserClassKeys.HumanUser
            },
            StatusConceptKey = StatusKeys.Active,
            Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Johnson", "William")
                },
            Addresses = new List<EntityAddress>()
                {
                    new EntityAddress(AddressUseKeys.HomeAddress, "123 Main Street West", "Hamilton", "ON", "CA", "L8K5N2")
                },
            Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(new AssigningAuthority() { Name = "OHIPCARD", DomainName = "OHIPCARD", Oid = "1.2.3.4.5.6" }, "12343120423")
                },
            Telecoms = new List<EntityTelecomAddress>()
                {
                    new EntityTelecomAddress(AddressUseKeys.WorkPlace, "mailto:will@johnson.com")
                },
            Tags = new List<EntityTag>()
                {
                    new EntityTag("hasBirthCertificate", "true")
                },
            Notes = new List<EntityNote>()
                {
                    new EntityNote(Guid.Empty, "William is a test patient")
                    {
                        Author = new Person()
                    }
                },
            GenderConceptKey = Guid.Parse("f4e3a6bb-612e-46b2-9f77-ff844d971198"),
            DateOfBirth = new DateTime(1984, 03, 22),
            MultipleBirthOrder = 2,
            DeceasedDate = new DateTime(2016, 05, 02),
            DeceasedDatePrecision = DatePrecision.Day,
            DateOfBirthPrecision = DatePrecision.Day,
            CreationTime = DateTimeOffset.Now
        };

        /// <summary>
        /// Tests the where clause matches LINQ
        /// </summary>
        [TestMethod]
        public void TestShouldMatchLinq()
        {
            ProtocolWhenClauseCollection when = new ProtocolWhenClauseCollection()
            {
                Clause = new List<object>() { "!DeceasedDate.HasValue" }
            };
            Assert.IsFalse(when.Evaluate(this.m_patientUnderTest));
        }

        /// <summary>
        /// Tests the where clause matches LINQ
        /// </summary>
        [TestMethod]
        public void TestShouldMatchSimpleImsi()
        {
            ProtocolWhenClauseCollection when = new ProtocolWhenClauseCollection()
            {
                Clause = new List<Object>() {
                    new WhenClauseImsiExpression() {
                        Expression = "deceasedDate=null"
                    }
                }
            };
            Assert.IsFalse(when.Evaluate(this.m_patientUnderTest));
        }

        /// <summary>
        /// Tests the where clause matches LINQ
        /// </summary>
        [TestMethod]
        public void TestShouldMatchSimpleXmlLinq()
        {
            Expression<Func<Patient, bool>> filterCondition = (data) => data.DeceasedDate == null;

            ProtocolWhenClauseCollection when = new ProtocolWhenClauseCollection()
            {
                Clause = new List<Object>() {
                    XmlExpression.FromExpression(filterCondition)
                }
            };
            Assert.IsFalse(when.Evaluate(this.m_patientUnderTest));
        }

        /// <summary>
        /// Tests the where clause matches LINQ
        /// </summary>
        [TestMethod]
        public void TestShouldMatchAllCondition()
        {
            Expression<Func<Patient, bool>> filterCondition = (data) => data.DateOfBirth <= DateTime.Now;

            ProtocolWhenClauseCollection when = new ProtocolWhenClauseCollection()
            {
                Operator = BinaryOperatorType.AndAlso,
                Clause = new List<Object>() {
                    XmlExpression.FromExpression(filterCondition),
                    new WhenClauseImsiExpression() { Expression = "tag[hasBirthCertificate].value=true" },
                    "StatusConceptKey.Value == Guid.Parse(\"" + StatusKeys.Active + "\")"
                }
            };
            Assert.IsTrue(when.Evaluate(this.m_patientUnderTest));

            when.Clause.Add("Tags.Count == 0");
            when.Compile<Patient>();
            Assert.IsFalse(when.Evaluate(this.m_patientUnderTest));
        }

    }
}
