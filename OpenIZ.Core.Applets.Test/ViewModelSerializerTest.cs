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
 * Date: 2016-7-6
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Applets.ViewModel.Description;

namespace OpenIZ.Core.Applets.Test
{
	/// <summary>
	/// Represents a view model serialization test
	/// </summary>
	[TestClass]
	public class ViewModelSerializerTest
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
		/// Doesn't desearialize key for some reason
		/// </summary>
		[TestMethod]
		public void TestBugfixDoesntDeserializeKey()
		{
			using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.DoesntSerializeKey.json")))
			{
				var json = sr.ReadToEnd();
				var patient = JsonViewModelSerializer.DeSerialize<Patient>(json);
				Assert.IsNotNull(patient.Key);
			}
		}

		/// <summary>
		/// Test de-serialization of the IMS patient object
		/// </summary>
		[TestMethod]
		public void TestDeSerializeComplexIMSObject()
		{
			using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.SimpleModel.json")))
			{
				var json = sr.ReadToEnd();
				var patient = JsonViewModelSerializer.DeSerialize<Patient>(json);
				Assert.AreEqual(this.m_patientUnderTest.ClassConceptKey, patient.ClassConceptKey);
			}
		}

		/// <summary>
		/// Test parsing of a form submission
		/// </summary>
		[TestMethod]
		public void TestParseBundle()
		{
			using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.SimpleBundle.json")))
			{
				var json = sr.ReadToEnd();
				var bundle = JsonViewModelSerializer.DeSerialize<Bundle>(json);
				json = JsonViewModelSerializer.Serialize(bundle);
				Assert.IsNotNull(bundle.EntryKey);

				Assert.IsInstanceOfType(bundle.Item[0], typeof(Patient));
				Assert.IsInstanceOfType(bundle.Item[1], typeof(Patient));

				Assert.AreEqual(this.m_patientUnderTest.ClassConceptKey, (bundle.Item[0] as Patient).ClassConceptKey);
				Assert.AreEqual(this.m_patientUnderTest.ClassConceptKey, (bundle.Item[1] as Patient).ClassConceptKey);
			}
		}

		/// <summary>
		/// Test parsing of a form submission
		/// </summary>
		[TestMethod]
		public void TestParseFormSubmission()
		{
			using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.RegistrationForm.json")))
			{
				var json = sr.ReadToEnd();
				var patient = JsonViewModelSerializer.DeSerialize<Patient>(json);
				json = JsonViewModelSerializer.Serialize(patient);
				Assert.IsNotNull(patient.Tags[0].TagKey);
				Assert.AreEqual(this.m_patientUnderTest.ClassConceptKey, patient.ClassConceptKey);
			}
		}

		/// <summary>
		/// Tests the parsing of a quantity observation.
		/// </summary>
		[TestMethod]
		public void TestParseQuantityObservation()
		{
			using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.QuantityObservation.json")))
			{
				var json = sr.ReadToEnd();
				var quantityObservation = JsonViewModelSerializer.DeSerialize<QuantityObservation>(json);

				Assert.IsNotNull(quantityObservation);
				Assert.AreEqual(0, quantityObservation.Value);
				Assert.AreEqual(4, quantityObservation.Participations.Count);

				Assert.AreEqual(1, quantityObservation.Participations.Count(p => p.ParticipationRole.Mnemonic == "Authororiginator"));
				Assert.AreEqual(1, quantityObservation.Participations.Count(p => p.ParticipationRole.Mnemonic == "Location"));
				Assert.AreEqual(1, quantityObservation.Participations.Count(p => p.ParticipationRole.Mnemonic == "Performer"));
				Assert.AreEqual(1, quantityObservation.Participations.Count(p => p.ParticipationRole.Mnemonic == "RecordTarget"));

				Assert.AreEqual(Guid.Parse("a261f8cd-69b0-49aa-91f4-e6d3e5c612ed"), quantityObservation.TypeConcept.Key);
			}
		}

		/// <summary>
		/// Tests the serialization of a simple act.
		/// </summary>
		[TestMethod]
		public void TestSerializeSimpleAct()
		{
			var act = new Act
			{
				ClassConcept = new Concept
				{
					Key = ActClassKeys.Encounter
				},
				CreationTime = DateTimeOffset.Now,
				Key = Guid.NewGuid(),
				MoodConcept = new Concept
				{
					Key = ActMoodKeys.Eventoccurrence
				}
			};

			var actual = JsonViewModelSerializer.Serialize(act);

			Assert.IsTrue(actual.Contains(ActMoodKeys.Eventoccurrence.ToString()));
			Assert.IsTrue(actual.Contains(ActClassKeys.Encounter.ToString()));
		}

		/// <summary>
		/// Tests the serialization of a complex act with participations.
		/// </summary>
		[TestMethod]
		public void TestSerializeActWithParticipations()
		{
			var act = new Act
			{
				ClassConcept = new Concept
				{
					Key = ActClassKeys.Encounter
				},
				CreationTime = DateTimeOffset.Now,
				Key = Guid.NewGuid(),
				MoodConcept = new Concept
				{
					Key = ActMoodKeys.Eventoccurrence
				},
				Participations = new List<ActParticipation>
				{
					new ActParticipation
					{
						ParticipationRole = new Concept
						{
							Key = ActParticipationKey.RecordTarget
						},
						PlayerEntity = this.m_patientUnderTest
					},
					new ActParticipation
					{
						ParticipationRole = new Concept
						{
							Key = ActParticipationKey.Location
						},
						PlayerEntity = new Place
						{
							Key = Guid.Parse("AE2795E7-C40A-41CF-B77D-855EE2C3BF47")
						}
					}
				}
			};

			var actual = JsonViewModelSerializer.Serialize(act);

			Assert.IsTrue(actual.Contains(ActMoodKeys.Eventoccurrence.ToString()));
			Assert.IsTrue(actual.Contains(ActClassKeys.Encounter.ToString()));
			Assert.IsTrue(actual.Contains(ActParticipationKey.RecordTarget.ToString()));
			Assert.IsTrue(actual.Contains(ActParticipationKey.Location.ToString()));
		}

		/// <summary>
		/// Tests the serialization of a complex act with relationships.
		/// </summary>
		[TestMethod]
		public void TestSerializeActWithRelationships()
		{
			var act = new Act
			{
				ClassConcept = new Concept
				{
					Key = ActClassKeys.Encounter
				},
				CreationTime = DateTimeOffset.Now,
				Key = Guid.NewGuid(),
				MoodConcept = new Concept
				{
					Key = ActMoodKeys.Eventoccurrence
				},
				Relationships = new List<ActRelationship>
				{
					new ActRelationship
					{
						RelationshipType = new Concept
						{
							Key = ActRelationshipTypeKeys.HasSubject
						},
						TargetAct = new Act
						{
							Participations = new List<ActParticipation>
							{
								new ActParticipation
								{
									ParticipationRole = new Concept
									{
										Key = ActParticipationKey.RecordTarget
									},
									PlayerEntity = this.m_patientUnderTest
								},
								new ActParticipation
								{
									ParticipationRole = new Concept
									{
										Key = ActParticipationKey.Location
									},
									PlayerEntity = new Place
									{
										Key = Guid.Parse("AE2795E7-C40A-41CF-B77D-855EE2C3BF47")
									}
								}
							}
						}
					}
				}
			};

			var actual = JsonViewModelSerializer.Serialize(act);

			Assert.IsTrue(actual.Contains(ActMoodKeys.Eventoccurrence.ToString()));
			Assert.IsTrue(actual.Contains(ActClassKeys.Encounter.ToString()));
			Assert.IsTrue(actual.Contains(ActParticipationKey.RecordTarget.ToString()));
			Assert.IsTrue(actual.Contains(ActParticipationKey.Location.ToString()));
		}

        /// <summary>
        /// Tests the serialization of a complex act with relationships.
        /// </summary>
        [TestMethod]
        public void TestSerializeActWithDefinition()
        {
            var act = new Act
            {
                ClassConcept = new Concept
                {
                    Key = ActClassKeys.Encounter
                },
                CreationTime = DateTimeOffset.Now,
                Key = Guid.NewGuid(),
                MoodConcept = new Concept
                {
                    Key = ActMoodKeys.Eventoccurrence
                },
                Relationships = new List<ActRelationship>
                {
                    new ActRelationship
                    {
                        RelationshipType = new Concept
                        {
                            Key = ActRelationshipTypeKeys.HasSubject,
                            Mnemonic = "HasSubject"
                        },
                        TargetAct = new Act
                        {
                            Participations = new List<ActParticipation>
                            {
                                new ActParticipation
                                {
                                    ParticipationRole = new Concept
                                    {
                                        Key = ActParticipationKey.RecordTarget,
                                        Mnemonic = "RecordTarget"
                                    },
                                    PlayerEntity = this.m_patientUnderTest
                                },
                                new ActParticipation
                                {
                                    ParticipationRole = new Concept
                                    {
                                        Key = ActParticipationKey.Location,
                                        Mnemonic = "Location"
                                    },
                                    PlayerEntity = new Place
                                    {
                                        Key = Guid.Parse("AE2795E7-C40A-41CF-B77D-855EE2C3BF47")
                                    }
                                }
                            }
                        }
                    }
                }
            };

            ViewModelDescription vmd = ViewModelDescription.Load(typeof(ViewModelSerializerTest).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.MinActModel.xml"));
            var actual = JsonViewModelSerializer.Serialize(act, vmd);

            Assert.IsTrue(actual.Contains(ActMoodKeys.Eventoccurrence.ToString()));
            Assert.IsTrue(actual.Contains(ActRelationshipTypeKeys.HasSubject.ToString()));

        }

        /// <summary>
        /// Tests the serialization of a complex act with relationships.
        /// </summary>
        [TestMethod]
        public void TestSerializeActWithDeepNestDefinition()
        {
            var act = new Act
            {
                ClassConcept = new Concept
                {
                    Key = ActClassKeys.Encounter
                },
                CreationTime = DateTimeOffset.Now,
                Key = Guid.NewGuid(),
                MoodConcept = new Concept
                {
                    Key = ActMoodKeys.Eventoccurrence
                },
                Relationships = new List<ActRelationship>
                {
                    new ActRelationship
                    {
                        RelationshipType = new Concept
                        {
                            Key = ActRelationshipTypeKeys.HasSubject,
                            Mnemonic = "HasSubject"
                        },
                        TargetAct = new Act
                        {
                            Participations = new List<ActParticipation>
                            {
                                new ActParticipation
                                {
                                    ParticipationRole = new Concept
                                    {
                                        Key = ActParticipationKey.RecordTarget,
                                        Mnemonic = "RecordTarget"
                                    },
                                    PlayerEntity = this.m_patientUnderTest
                                },
                                new ActParticipation
                                {
                                    ParticipationRole = new Concept
                                    {
                                        Key = ActParticipationKey.Location,
                                        Mnemonic = "Location"
                                    },
                                    PlayerEntity = new Place
                                    {
                                        Key = Guid.Parse("AE2795E7-C40A-41CF-B77D-855EE2C3BF47")
                                    }
                                }
                            }
                        }
                    }
                }
            };

            ViewModelDescription vmd = ViewModelDescription.Load(typeof(ViewModelSerializerTest).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.DeepActModel.xml"));
            var actual = JsonViewModelSerializer.Serialize(act, vmd);

            Assert.IsTrue(actual.Contains(ActMoodKeys.Eventoccurrence.ToString()));
            Assert.IsTrue(actual.Contains(ActRelationshipTypeKeys.HasSubject.ToString()));

        }


        /// <summary>
        /// Test serialization of the IMS patient object
        /// </summary>
        [TestMethod]
		public void TestSerializeComplexIMSObject()
		{
			String json = JsonViewModelSerializer.Serialize(this.m_patientUnderTest);
			Assert.IsNotNull(json);
		}

		/// <summary>
		/// Test parsing of a form submission
		/// </summary>
		[TestMethod]
		public void TestShouldParseSimpleArray()
		{
			using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.DoesntInterpretSimpleArray.json")))
			{
				var json = sr.ReadToEnd();
				var bundle = JsonViewModelSerializer.DeSerialize<UserEntity>(json);
				json = JsonViewModelSerializer.Serialize(bundle);
				Assert.IsNotNull(bundle.Names[0].Component[1].ComponentType);
				Assert.AreEqual("Given", bundle.Names[0].Component[0].ComponentType.Mnemonic);
				Assert.AreEqual("Family", bundle.Names[0].Component[1].ComponentType.Mnemonic);
				Assert.AreEqual(EntityClassKeys.Person, bundle.ClassConceptKey);
			}
		}
	}
}