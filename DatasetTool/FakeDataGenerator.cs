using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Acts;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using OpenIZ.Protocol.Xml;
using OpenIZ.Protocol.Xml.Model;
using OpenIZ.Core.Protocol;
using OpenIZ.Persistence.Data.MSSQL.Services;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using MARC.Everest.Threading;
using OpenIZ.Core;

namespace OizDevTool
{
    /// <summary>
    /// Represents a fake data generator
    /// </summary>
    public static class FakeDataGenerator
    {
        /// <summary>
        /// Console parameters
        /// </summary>
        public class ConsoleParameters
        {
            /// <summary>
            /// Gets or sets the population size
            /// </summary>
            [Parameter("popsize")]
            public String PopulationSize { get; set; }

            /// <summary>
            /// Gets or sets the population size
            /// </summary>
            [Parameter("maxage")]
            public String MaxAge { get; set; }
        }

        /// <summary>
        /// Seeder protocol repository service
        /// </summary>
        public class SeederProtocolRepositoryService : IClinicalProtocolRepositoryService
        {
            private List<Protocol> m_protocols = new List<Protocol>();

            /// <summary>
            /// Load the protocols into memory
            /// </summary>
            public SeederProtocolRepositoryService()
            {
                foreach(var f in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Protocols")))
                {
                    using (FileStream fs = File.OpenRead(f))
                    {
                        ProtocolDefinition pdf = ProtocolDefinition.Load(fs);
                        XmlClinicalProtocol xcp = new XmlClinicalProtocol(pdf);
                        this.m_protocols.Add(xcp.GetProtcolData());
                    }
                }
            }
            /// <summary>
            /// Find the specified protocol
            /// </summary>
            /// <param name="predicate"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            /// <param name="totalResults"></param>
            /// <returns></returns>
            public IEnumerable<Protocol> FindProtocol(Expression<Func<Protocol, bool>> predicate, int offset, int? count, out int totalResults)
            {
                var retVal = this.m_protocols.Where(predicate.Compile());
                totalResults = retVal.Count();
                return retVal;
            }

            public Protocol InsertProtocol(Protocol data)
            {
                throw new NotImplementedException();
            }
        }

        private static SimpleCarePlanService scp = new SimpleCarePlanService();
        
        /// <summary>
        /// Generate patients
        /// </summary>
        public static void GeneratePatients (String[] args)
        {
            var parameters = new ParameterParser<ConsoleParameters>().Parse(args);
            int populationSize = Int32.Parse(parameters.PopulationSize ?? "10");
            int maxAge = Int32.Parse(parameters.MaxAge ?? "500");
            ApplicationContext.Current.AddServiceProvider(typeof(SimpleCarePlanService));
            ApplicationContext.Current.AddServiceProvider(typeof(SeederProtocolRepositoryService));

            ApplicationServiceContext.Current = ApplicationContext.Current;
            //cp.Repository = new SeederProtocolRepositoryService();
            ApplicationContext.Current.Start();

            WaitThreadPool wtp = new WaitThreadPool();

            for(int i = 0; i < populationSize; i++)
            {
                wtp.QueueUserWorkItem((s) =>
                {
                    var patient = GeneratePatient(maxAge);
                    var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
                    var actPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SubstanceAdministration>>();

                    // Insert
                    patient = persistence.Insert(patient, AuthenticationContext.SystemPrincipal, TransactionMode.Commit);

                    // Forecast 
                    var plan = scp.CreateCarePlan(patient);

                    foreach (var itm in plan.OfType<SubstanceAdministration>())
                        actPersistence.Insert(itm, AuthenticationContext.SystemPrincipal, TransactionMode.Commit);

                    foreach (var itm in plan.Where(o => o.StartTime < DateTime.Now).OfType<SubstanceAdministration>())
                    {
                        var flfls = new SubstanceAdministration()
                        {
                            ActTime = itm.ActTime,
                            MoodConceptKey = ActMoodKeys.Eventoccurrence,
                            RouteKey = itm.RouteKey,
                            DoseUnitKey = itm.DoseUnitKey,
                            Protocols = itm.Protocols,
                            DoseQuantity = 1,
                            IsNegated = false,
                            SequenceId = itm.SequenceId,
                            StatusConceptKey = StatusKeys.Active,
                            Relationships = new List<ActRelationship>() { new ActRelationship(ActRelationshipTypeKeys.Fulfills, itm.Key) },
                            Participations = new List<ActParticipation>()
                        {
                            new ActParticipation(ActParticipationKey.Consumable, itm.Participations.FirstOrDefault(o=>o.ParticipationRoleKey == ActParticipationKey.Product).PlayerEntity),
                            new ActParticipation(ActParticipationKey.RecordTarget, patient.Key)
                        }
                        };
                        actPersistence.Insert(flfls, AuthenticationContext.SystemPrincipal, TransactionMode.Commit);
                    }
                });
                
            }
            wtp.WaitOne();
        }

        /// <summary>
        /// Generate the patient with schedule
        /// </summary>
        private static Patient GeneratePatient(int maxAge)
        {
            
            Person mother = new Person()
            {
                Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, SeedData.SeedData.Current.PickRandomFamilyName(), SeedData.SeedData.Current.PickRandomGivenName("Female").Name) },
                Telecoms = new List<EntityTelecomAddress>() {  new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, String.Format("+255 {0:000} {1:000} {2:000}", Guid.NewGuid().ToByteArray()[0], Guid.NewGuid().ToString()[1], Guid.NewGuid().ToByteArray()[2])) },
                Identifiers = new List<OpenIZ.Core.Model.DataTypes.EntityIdentifier>() {  new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("NID", "National Identifier", "1.2.3.4.5.6"), BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0).ToString()) }
            };

            String gender = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 1) % 2 == 0 ? "Male" : "Female";

            // Child
            Patient child = new Patient()
            {
                Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, mother.Names[0].Component[0].Value, SeedData.SeedData.Current.PickRandomGivenName(gender).Name, SeedData.SeedData.Current.PickRandomGivenName(gender).Name) },
                DateOfBirth = DateTime.Now.AddDays(-Math.Abs(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0) % maxAge)),
                Addresses = new List<EntityAddress>() { new EntityAddress(AddressUseKeys.HomeAddress, null, "Arusha DC", "Arusha", "TZ", "TZ.NT.AS.AS") },
                GenderConcept = new Concept() { Mnemonic = gender },
                Identifiers = new List<EntityIdentifier>() {  new EntityIdentifier(new AssigningAuthority("TIIS_BARCODE", "TImR Barcode IDs", "2.3.4.5.6.7"), BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace(":","")) }
            };

            // Associate
            child.Relationships = new List<EntityRelationship>() { new EntityRelationship(EntityRelationshipTypeKeys.Mother, mother) };

            return child;

        }
    }
}
