/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-4
 */
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
using OpenIZ.Core.Services.Impl;
using System.Threading;
using System.ComponentModel;

namespace OizDevTool
{
    /// <summary>
    /// Represents a fake data generator
    /// </summary>
    [Description("Tooling to create fake data")]
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
            [Description("Population size of the generated dataset")]
            public String PopulationSize { get; set; }

            /// <summary>
            /// Gets or sets the population size
            /// </summary>
            [Parameter("maxage")]
            [Description("The maximum age of patients to generate")]
            public String MaxAge { get; set; }

            /// <summary>
            /// Barcode auth
            /// </summary>
            [Parameter("auth")]
            [Description("The assigning authority from which a random ID should be generated")]
            public String BarcodeAuth { get; set; }
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
                foreach (var f in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Protocols")))
                {
                    using (FileStream fs = File.OpenRead(f))
                    {
                        ProtocolDefinition pdf = ProtocolDefinition.Load(fs);
                        XmlClinicalProtocol xcp = new XmlClinicalProtocol(pdf);
                        this.m_protocols.Add(xcp.GetProtocolData());
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
        /// Generate stock
        /// </summary>
        [Description("Generates fake stock levels by assigning random Materials to random Places")]
        public static void GenerateStock(String[] args)
        {
            ApplicationServiceContext.Current = ApplicationContext.Current;
            //cp.Repository = new SeederProtocolRepositoryService();
            ApplicationContext.Current.Start();
            var idp = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            WaitThreadPool wtp = new WaitThreadPool();
            var mat = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>().Query(o => o.ClassConceptKey == EntityClassKeys.Material, AuthenticationContext.SystemPrincipal);
            Console.WriteLine("Database has {0} materials", mat.Count());

            int tr = 0, ofs = 0;
            Console.WriteLine("Querying for places");
            var results = idp.Query(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, ofs, 25, AuthenticationContext.SystemPrincipal, out tr);
            Console.WriteLine("Will create fake stock for {0} places", tr);
            var r = new Random();

            while (ofs < tr)
            {
                foreach (var p in results)
                {
                    wtp.QueueUserWorkItem((parm) =>
                    {
                        try
                        {
                            Place target = parm as Place;
                            Console.WriteLine("Starting seeding for {0} currently {1} relationships", target.Names.FirstOrDefault().Component.FirstOrDefault().Value, target.Relationships.Count);

                            // Add some stock!!! :)
                            foreach (var m in mat)
                            {
                                var mmats = m.Relationships.Where(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct).OrderBy(o => r.Next()).FirstOrDefault();
                                Console.WriteLine("Selected {0} out of {1} materials", mmats, m.Relationships.Count);
                                var rdp = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();
                                if (mmats != null)
                                {

                                    var er = new EntityRelationship(EntityRelationshipTypeKeys.OwnedEntity, mmats.TargetEntityKey) { Quantity = r.Next(0, 100), SourceEntityKey = target.Key, EffectiveVersionSequenceId = target.VersionSequence };
                                    Console.WriteLine("{0} > {1} {2}", target.Names.FirstOrDefault().Component.FirstOrDefault().Value, er.Quantity, m.Names.FirstOrDefault().Component.FirstOrDefault().Value);
                                    rdp.Insert(er, AuthenticationContext.SystemPrincipal, TransactionMode.Commit);
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }

                    }, p);
                }
                wtp.WaitOne();
                ofs += 25;
                results = idp.Query(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, ofs, 25, AuthenticationContext.SystemPrincipal, out tr);

            }
        }

        /// <summary>
        /// Generate patients
        /// </summary>
        [Description("Generates a randomized patient population")]
        [Example("Generate a fake patient population with 1000 patients, at most 30 days old", "--popsize=1000 --maxage=30 --auth=TEST_AUTH")]
        [ParameterClass(typeof(ConsoleParameters))]
        public static void GeneratePatients(String[] args)
        {
            var parameters = new ParameterParser<ConsoleParameters>().Parse(args);
            int populationSize = Int32.Parse(parameters.PopulationSize ?? "10");
            int maxAge = Int32.Parse(parameters.MaxAge ?? "500");


            ApplicationContext.Current.AddServiceProvider(typeof(SimpleCarePlanService));
            ApplicationContext.Current.AddServiceProvider(typeof(SeederProtocolRepositoryService));
            ApplicationContext.Current.AddServiceProvider(typeof(LocalPlaceRepositoryService));
            ApplicationContext.Current.AddServiceProvider(typeof(LocalActRepositoryService));
            ApplicationServiceContext.Current = ApplicationContext.Current;
            //cp.Repository = new SeederProtocolRepositoryService();
            ApplicationContext.Current.Start();

            int tr = 0;
            var places = ApplicationContext.Current.GetService<IPlaceRepositoryService>().Find(o => o.StatusConceptKey == StatusKeys.Active && o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, 0, 20 ,out tr);
            places = places.Union(ApplicationContext.Current.GetService<IPlaceRepositoryService>().Find(o => o.StatusConceptKey == StatusKeys.Active && o.ClassConceptKey != EntityClassKeys.ServiceDeliveryLocation, 0, 20, out tr));
            WaitThreadPool wtp = new WaitThreadPool(4);
            Random r = new Random();

            WaitCallback genFunc = (s) =>
            {
                AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

                var patient = GeneratePatient(maxAge, parameters.BarcodeAuth, places, r);
                var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
                // Insert
                patient = persistence.Insert(patient, AuthenticationContext.SystemPrincipal, TransactionMode.Commit);
                Console.WriteLine("Generated Patient: {0} ({1} mo)", patient, DateTime.Now.Subtract(patient.DateOfBirth.Value).TotalDays / 30);

                // Schedule
                var acts = ApplicationContext.Current.GetService<ICarePlanService>().CreateCarePlan(patient).Action.Where(o => o.ActTime <= DateTime.Now);
                foreach (var act in acts)
                {
                    act.MoodConceptKey = ActMoodKeys.Eventoccurrence;
                    act.StatusConceptKey = StatusKeys.Completed;
                    act.ActTime = act.ActTime.AddDays(r.Next(0, 5));

                    if (act is QuantityObservation)
                        (act as QuantityObservation).Value = (r.Next((int)(act.ActTime - patient.DateOfBirth.Value).TotalDays, (int)(act.ActTime - patient.DateOfBirth.Value).TotalDays + 10) / 10) + 4;
                    // Persist the act
                    ApplicationContext.Current.GetService<IActRepositoryService>().Insert(act);
                }
            };
            genFunc(null);

            for (int i = 0; i < populationSize; i++)
            {

                wtp.QueueUserWorkItem(genFunc);

            }
            wtp.WaitOne();
        }

        /// <summary>
        /// Generate the patient with schedule
        /// </summary>
        private static Patient GeneratePatient(int maxAge, string barcodeAuth, IEnumerable<Place> places, Random r)
        {

            var placeService = ApplicationContext.Current.GetService<IPlaceRepositoryService>();

            Person mother = new Person()
            {
                Key = Guid.NewGuid(),
                Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, SeedData.SeedData.Current.PickRandomFamilyName(), SeedData.SeedData.Current.PickRandomGivenName("Female").Name) },
                Telecoms = new List<EntityTelecomAddress>() { new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, String.Format("+255 {0:000} {1:000} {2:000}", Guid.NewGuid().ToByteArray()[0], Guid.NewGuid().ToString()[1], Guid.NewGuid().ToByteArray()[2])) },
                Identifiers = new List<OpenIZ.Core.Model.DataTypes.EntityIdentifier>() { new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("NID", "National Identifier", "1.2.3.4.5.6"), BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0).ToString()) }
            };

            String gender = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 1) % 2 == 0 ? "Male" : "Female";

            var villageId = places.Where(o => o.ClassConceptKey != EntityClassKeys.ServiceDeliveryLocation).OrderBy(o => r.Next()).FirstOrDefault().Addresses.First();
            var addr = new EntityAddress();
            addr.AddressUseKey = AddressUseKeys.HomeAddress;
            addr.Component = new List<EntityAddressComponent>(villageId.Component.Select(o => new EntityAddressComponent(o.ComponentTypeKey.Value, o.Value)));
            // Child
            Patient child = new Patient()
            {
                Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, mother.Names[0].Component[0].Value, SeedData.SeedData.Current.PickRandomGivenName(gender).Name, SeedData.SeedData.Current.PickRandomGivenName(gender).Name) },
                DateOfBirth = DateTime.Now.AddDays(-Math.Abs(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0) % maxAge)),
                Addresses = new List<EntityAddress>() { addr },
                GenderConcept = new Concept() { Mnemonic = gender },
                Identifiers = new List<EntityIdentifier>() { new EntityIdentifier(barcodeAuth, BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", "").Substring(0, 10)) }
            };
            // Associate
            child.Relationships = new List<EntityRelationship>() {
                new EntityRelationship(EntityRelationshipTypeKeys.Mother, mother),
                new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, places.Where(o=>o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation).OrderBy(o=>r.Next()).FirstOrDefault().Key)
            };

            return child;

        }
    }
}
