using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    /// <summary>
    /// Person persistence test
    /// </summary>
    [TestClass]
    public class PersonPersistenceServiceTest : PersistenceTest<Person>
    {


        private static IPrincipal s_authorization;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
                           "DataDirectory",
                           Path.Combine(context.TestDeploymentDir, string.Empty));
            s_authorization = AuthenticationContext.SystemPrincipal;

        }

        /// <summary>
        /// Test the persistence of a person
        /// </summary>
        [TestMethod]
        public void TestPersistPerson()
        {
            var v = Convert.FromBase64String("eyJyb2xlIjoiQ0xJTklDQUxfU1RBRkYiLCJpc3MiOiJodHRwOi8vZGVtby5vcGVuaXoub3JnOjgwODAvYXV0aC9vYXV0aDJfdG9rZW4iLCJhdXRoX3RpbWUiOiIyMDE2LTA3LTAxVDIyOjQxOjM5Ljg2MTQ0NzYtMDQ6MDAiLCJhdXRobWV0aG9kIjoiT0F1dGgyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9leHBpcmF0aW9uIjoiMjAxNi0wNy0wMVQyMzo0MTozOS44NjE0NDc2LTA0OjAwIiwidW5pcXVlX25hbWUiOiJMdWN5IiwiaHR0cDovL29wZW5pei5vcmcvY2xhaW1zL2FwcGxpY2F0aW9uLWlkIjoiNGE2ZDNiYmEtY2QzOC1lNjExLTgwYzUtMDA1MDU2OWQ4M2UzIiwiaHR0cDovL29wZW5pei5vcmcvY2xhaW1zL2dyYW50IjpbIjEuMy42LjEuNC4xLjMzMzQ5LjMuMS41LjkuMi4xIiwiMS4zLjYuMS40LjEuMzMzNDkuMy4xLjUuOS4yLjIiXSwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvYXV0aGVudGljYXRpb24iOiJTcWxDbGFpbXNJZGVudGl0eSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2F1dGhvcml6YXRpb25kZWNpc2lvbiI6IkdSQU5UIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMDFjMmI3OTktY2MzOC1lNjExLTgwYzUtMDA1MDU2OWQ4M2UzIiwibmFtZWlkIjoiMDFjMmI3OTktY2MzOC1lNjExLTgwYzUtMDA1MDU2OWQ4M2UzIiwiZW1haWwiOiJtYWlsdG86bHVjeUBtYXJjLWhpLmNhIiwic3ViIjoiMDFjMmI3OTktY2MzOC1lNjExLTgwYzUtMDA1MDU2OWQ4M2UzIiwiYXVkIjoiaHR0cDovL2RlbW8ub3Blbml6Lm9yZzo4MDgwL2ltc2kiLCJleHAiOjE0Njc0MzA4OTksIm5iZiI6MTQ2NzQyNzI5OX0=");
            Person p = new Person()
            {
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Smith", "Johnny")
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
                    new EntityTelecomAddress(AddressUseKeys.WorkPlace, "mailto:bob@joe.com")
                },
                    Tags = new List<EntityTag>()
                {
                    new EntityTag("hasBirthCertificate", "true")
                },
                    Notes = new List<EntityNote>()
                {
                    new EntityNote(Guid.Empty, "Johnny is really allergic to eggs!")
                    {
                        Author = new Person()
                    }
                },
                DateOfBirth = new DateTime(1984, 03, 22),
                DateOfBirthPrecision = DatePrecision.Day
            };
            
            var afterInsert = base.DoTestInsert(p, s_authorization);
            Assert.AreEqual(DatePrecision.Day, afterInsert.DateOfBirthPrecision);
            Assert.AreEqual(new DateTime(1984, 03, 22), afterInsert.DateOfBirth);
            Assert.AreEqual(1, p.Names.Count);
            Assert.AreEqual(1, p.Addresses.Count);
            Assert.AreEqual(1, p.Identifiers.Count);
            Assert.AreEqual(1, p.Telecoms.Count);
            Assert.AreEqual(1, p.Tags.Count);
            Assert.AreEqual(1, p.Notes.Count);
            Assert.AreEqual(EntityClassKeys.Person, p.ClassConceptKey);
            Assert.AreEqual(DeterminerKeys.Specific, p.DeterminerConceptKey);
            Assert.AreEqual(StatusKeys.Active, p.StatusConceptKey);
        }
    }
}
