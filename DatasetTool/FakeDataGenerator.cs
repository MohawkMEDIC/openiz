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
using MARC.Everest.Threading;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Protocol;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;
using OpenIZ.Core.Services.Impl;
using OpenIZ.Protocol.Xml;
using OpenIZ.Protocol.Xml.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace OizDevTool
{
	/// <summary>
	/// Represents a fake data generator.
	/// </summary>
	[Description("Tooling to create fake data")]
	public static class FakeDataGenerator
	{
		/// <summary>
		/// Generate patients
		/// </summary>
		/// <param name="args">The arguments.</param>
		[Description("Generates a randomized patient population")]
		[Example("Generate a fake patient population with 1000 patients, at most 30 days old", "--popsize=1000 --maxage=30 --auth=TEST_AUTH")]
		[ParameterClass(typeof(ConsoleParameters))]
		public static void GeneratePatients(String[] args)
		{
			var parameters = new ParameterParser<ConsoleParameters>().Parse(args);
			int populationSize = Int32.Parse(parameters.PopulationSize ?? "10");
			int maxAge = Int32.Parse(parameters.MaxAge ?? "500");

			Console.WriteLine("Adding minimal service providers...");
			ApplicationContext.Current.AddServiceProvider(typeof(SimpleCarePlanService));
			ApplicationContext.Current.AddServiceProvider(typeof(LocalPlaceRepositoryService));
			ApplicationContext.Current.AddServiceProvider(typeof(LocalActRepositoryService));
			ApplicationServiceContext.Current = ApplicationContext.Current;
			//cp.Repository = new SeederProtocolRepositoryService();
			ApplicationContext.Current.Start();

			int tr = 0;
			Console.WriteLine("Adding minimal loading places...");

            IEnumerable<Place> places = null;

            Guid facId = Guid.Empty;
            if (!String.IsNullOrEmpty(parameters.Facility) && Guid.TryParse(parameters.Facility, out facId))
            {
                places = (ApplicationContext.Current.GetService<IPlaceRepositoryService>() as IFastQueryRepositoryService).FindFast<Place>(o => o.Key == facId, 0, 1, out tr, Guid.Empty);
            }
            else
            {
                places = (ApplicationContext.Current.GetService<IPlaceRepositoryService>() as IFastQueryRepositoryService).FindFast<Place>(o => o.StatusConceptKey == StatusKeys.Active && o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, 0, Int32.Parse(parameters.FacilityCount ?? "1000"), out tr, Guid.Empty);
            };

            places = places.Union((ApplicationContext.Current.GetService<IPlaceRepositoryService>() as IFastQueryRepositoryService).FindFast<Place>(o => o.StatusConceptKey == StatusKeys.Active && o.ClassConceptKey != EntityClassKeys.ServiceDeliveryLocation, 0, Int32.Parse(parameters.FacilityCount ?? "1000"), out tr, Guid.Empty));

            WaitThreadPool wtp = new WaitThreadPool(Environment.ProcessorCount * 4);
			Random r = new Random();

			int npatients = 0;
			Console.WriteLine("Generating Patients...");

            DateTime startTime = DateTime.Now;

			WaitCallback genFunc = (s) =>
			{
				AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

				var patient = GeneratePatient(maxAge, parameters.BarcodeAuth, places, r);

				if (patient == null)
					return;

				var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
				// Insert
				int pPatient = Interlocked.Increment(ref npatients);

                var ips = (((double)(DateTime.Now - startTime).Ticks / pPatient) * (populationSize - pPatient));
                var remaining = new TimeSpan((long)ips);

                patient = persistence.Insert(patient, AuthenticationContext.SystemPrincipal, TransactionMode.Commit);
				Console.WriteLine("#{2:#,###,###}({4:0%} - ETA:{5}): {0} ({1:#0} mo) [{3}]", patient.Identifiers.First().Value, DateTime.Now.Subtract(patient.DateOfBirth.Value).TotalDays / 30, pPatient, places.FirstOrDefault(p => p.Key == patient.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).TargetEntityKey).Names.FirstOrDefault().ToString(), (float)pPatient / populationSize, remaining.ToString("hh'h 'mm'm 'ss's'"));

				// Schedule
				if (!parameters.PatientOnly)
				{
					var acts = ApplicationContext.Current.GetService<ICarePlanService>().CreateCarePlan(patient).Action.Where(o => o.ActTime <= DateTime.Now).Select(o => o.Copy() as Act);

					Bundle bundle = new Bundle();

					foreach (var act in acts)
					{
						if (act.Key.Value.ToByteArray()[0] > 200) continue;
						act.MoodConceptKey = ActMoodKeys.Eventoccurrence;
						act.StatusConceptKey = StatusKeys.Completed;
						act.ActTime = act.ActTime.AddDays(r.Next(0, 5));
						act.StartTime = null;
						act.StopTime = null;
						if (act is QuantityObservation)
							(act as QuantityObservation).Value = (r.Next((int)(act.ActTime - patient.DateOfBirth.Value).TotalDays, (int)(act.ActTime - patient.DateOfBirth.Value).TotalDays + 10) / 10) + 4;
						else
						{
							act.Tags.AddRange(new ActTag[]
							{
								new ActTag("catchmentIndicator", "True"),
								new ActTag("hasRunAdjustment", "True")
							});
							act.Participations.Add(new ActParticipation(ActParticipationKey.Location, patient.Relationships.First(l => l.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).TargetEntityKey));
						}

						// Persist the act
						bundle.Item.Add(act);
					}
					Console.WriteLine("\t {0} acts", bundle.Item.Count());

					ApplicationContext.Current.GetService<IBatchRepositoryService>().Insert(bundle);
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
		/// Generate stock
		/// </summary>
		/// <param name="args">The arguments.</param>
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
			var results = idp.Query(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, ofs, 1000, AuthenticationContext.SystemPrincipal, out tr);
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
								var mmats = m.Relationships.Where(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance).OrderBy(o => r.Next()).FirstOrDefault();
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
						catch (Exception e)
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
		/// Generate the patient with schedule
		/// </summary>
		internal static Patient GeneratePatient(int maxAge, string barcodeAuth, IEnumerable<Place> places, Random r)
		{
			var mother = new Person()
			{
				Key = Guid.NewGuid(),
				Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, SeedData.SeedData.Current.PickRandomFamilyName(), SeedData.SeedData.Current.PickRandomGivenName("Female").Name) },
				Telecoms = new List<EntityTelecomAddress>() { new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, String.Format("+255 {0:000} {1:000} {2:000}", Guid.NewGuid().ToByteArray()[0], Guid.NewGuid().ToString()[1], Guid.NewGuid().ToByteArray()[2])) },
				Identifiers = new List<OpenIZ.Core.Model.DataTypes.EntityIdentifier>() { new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("NID", "National Identifier", "1.2.3.4.5.6"), BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0).ToString()) }
			};

			var gender = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 1) % 2 == 0 ? "Male" : "Female";

			EntityAddress address = null;

			try
			{
				var villageId = places.Where(o => o.ClassConceptKey != EntityClassKeys.ServiceDeliveryLocation).OrderBy(o => r.Next()).FirstOrDefault().Addresses.First();
				address = new EntityAddress
				{
					AddressUseKey = AddressUseKeys.HomeAddress,
					Component = new List<EntityAddressComponent>(villageId.Component.Select(o => new EntityAddressComponent(o.ComponentTypeKey.Value, o.Value)))
				};
			}
			catch
			{
				Console.WriteLine("Unable to determine address for patient");
			}


			// Child
			Patient child = new Patient()
			{
				Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, mother.Names[0].Component[0].Value, SeedData.SeedData.Current.PickRandomGivenName(gender).Name, SeedData.SeedData.Current.PickRandomGivenName(gender).Name) },
				DateOfBirth = DateTime.Now.AddDays(-Math.Abs(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0) % maxAge)),
				GenderConcept = new Concept() { Mnemonic = gender },
				Identifiers = new List<EntityIdentifier>() { new EntityIdentifier(barcodeAuth, BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", "").Replace("A","0").Replace("B","4").Replace("C", "6").Replace("D", "8").Replace("E", "9").Replace("F", "5").Substring(0, 10)) }
			};

			if (address != null)
				child.Addresses.Add(address);

			// Associate
			child.Relationships = new List<EntityRelationship>() {
				new EntityRelationship(EntityRelationshipTypeKeys.Mother, mother),
				new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, places.Where(o=>o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation).OrderBy(o=>r.Next()).FirstOrDefault().Key)
			};

			return child;
		}

		/// <summary>
		/// Console parameters
		/// </summary>
		public class ConsoleParameters
		{
			/// <summary>
			/// Barcode auth
			/// </summary>
			[Parameter("auth")]
			[Description("The assigning authority from which a random ID should be generated")]
			public String BarcodeAuth { get; set; }

			/// <summary>
			/// Gets or sets the population size
			/// </summary>
			[Parameter("maxage")]
			[Description("The maximum age of patients to generate")]
			public String MaxAge { get; set; }

			/// <summary>
			/// Generate patients only
			/// </summary>
			[Parameter("patonly")]
			[Description("Only generate patients, not acts")]
			public bool PatientOnly { get; internal set; }

			/// <summary>
			/// Gets or sets the population size
			/// </summary>
			[Parameter("popsize")]
			[Description("Population size of the generated dataset")]
			public String PopulationSize { get; set; }

            [Parameter("facilities")]
            [Description("Dictates the number of facilities to place the patients in")]
            public string FacilityCount { get; set; }

            [Parameter("facility")]
            [Description("Generates all the patients in the specified facility")]
            public string Facility { get; set; }
        }

		
	}
}