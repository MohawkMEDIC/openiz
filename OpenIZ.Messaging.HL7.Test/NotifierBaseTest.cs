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
 * User: khannan
 * Date: 2016-11-12
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Model.V231.Datatype;
using NHapi.Model.V231.Message;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.HL7.Notifier;
using NHapi.Model.V231.Segment;
using OpenIZ.Core.Model.Roles;

namespace OpenIZ.Messaging.HL7.Test
{
	[TestClass]
	public class NotifierBaseTest
	{
		/// <summary>
		/// The internal reference to the <see cref="Configuration.TargetConfiguration"/> instance.
		/// </summary>
		private Configuration.TargetConfiguration configuration;

		/// <summary>
		/// The internal reference to the <see cref="EntityAddress"/> instance.
		/// </summary>
		private EntityAddress entityAddress;

		/// <summary>
		/// The internal reference to the <see cref="EntityName"/> instance.
		/// </summary>
		private EntityName entityName;

		/// <summary>
		/// The internal reference to the <see cref="GenericMessage.V25"/> instance.
		/// </summary>
		private GenericMessage.V25 genericMessage;

		/// <summary>
		/// The internal reference to the <see cref="Patient"/> instance.
		/// </summary>
		private Patient patient;

		/// <summary>
		/// Runs cleanup after each test execution.
		/// </summary>
		[TestCleanup]
		public void Cleanup()
		{
			this.configuration = null;
			this.entityAddress = null;
			this.entityName = null;
			this.genericMessage = null;
			this.patient = null;
		}

		/// <summary>
		/// Runs initialization before each test execution.
		/// </summary>
		[TestInitialize]
		public void Initialize()
		{
			this.configuration = new Configuration.TargetConfiguration("Test", "llp://localhost:2100", "PAT_IDENTITY_SRC", "UnitTestDevice");

			this.configuration.NotificationDomainConfigurations.Add(new Configuration.NotificationDomainConfiguration("TestNotificationDomain"));

			this.entityAddress = new EntityAddress
			{
				AddressUse = new Concept
				{
					Key = AddressUseKeys.HomeAddress
				}
			};

			this.entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.City, "Hamilton"));
			this.entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.Country, "Canada"));
			this.entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.PostalCode, "L8N3T2"));
			this.entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.State, "Ontario"));
			this.entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.StreetAddressLine, "123 Main street west"));

			this.entityName = new EntityName(NameUseKeys.OfficialRecord, "Khanna", "Nityan David");

			this.genericMessage = new GenericMessage.V25(new DefaultModelClassFactory());

			this.patient = new Patient
			{
				Addresses = new List<EntityAddress>
				{
					this.entityAddress
				},
				DateOfBirth = new DateTime(1970, 01, 01),
				DateOfBirthPrecision = DatePrecision.Day,
				GenderConcept = new Concept
				{
					Mnemonic = "male"
				},
				Names = new List<EntityName>()
				{
					this.entityName
				},
				Relationships = new List<EntityRelationship>
				{
					new EntityRelationship(EntityRelationshipTypeKeys.Mother, new Person
					{
						Names = new List<EntityName>
						{
							new EntityName(NameUseKeys.OfficialRecord, "Smith", "Mary L A")
						}
					})
				}
			};
		}

		/// <summary>
		/// Tests the updating of an <see cref="XAD" /> type.
		/// </summary>
		[TestMethod]
		public void TestUpdateAD()
		{
			var actual = new XAD(this.genericMessage);

			NotifierBase.UpdateAD(entityAddress, actual);

			Assert.AreEqual("Hamilton", actual.City.Value);
			Assert.AreEqual("Canada", actual.Country.Value);
			Assert.AreEqual("L8N3T2", actual.ZipOrPostalCode.Value);
			Assert.AreEqual("Ontario", actual.StateOrProvince.Value);
			Assert.AreEqual("123 Main street west", actual.StreetAddress.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="XAD"/> type with a city only.
		/// </summary>
		[TestMethod]
		public void TestUpdateADCityOnly()
		{
			var actual = new XAD(this.genericMessage);

			this.entityAddress.Component = new List<EntityAddressComponent>
			{
				new EntityAddressComponent(AddressComponentKeys.City, "Hamilton")
			};

			NotifierBase.UpdateAD(entityAddress, actual);

			Assert.AreEqual("Hamilton", actual.City.Value);
			Assert.AreEqual(string.Empty, actual.Country.Value);
			Assert.AreEqual(string.Empty, actual.ZipOrPostalCode.Value);
			Assert.AreEqual(string.Empty, actual.StateOrProvince.Value);
			Assert.AreEqual(string.Empty, actual.StreetAddress.Value);
		}

		/// <summary>
		/// Tests the update of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateGenderFemaleConcept()
		{
			var actual = new ADT_A01().PID;

			NotifierBase.UpdateGender("female", actual);

			Assert.AreEqual("F", actual.Sex.Value);
		}

		/// <summary>
		/// Tests the update of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateGenderFemaleConceptKey()
		{
			var actual = new ADT_A01().PID;

			NotifierBase.UpdateGender("094941e9-a3db-48b5-862c-bc289bd7f86c", actual);

			Assert.AreEqual("F", actual.Sex.Value);
		}

		/// <summary>
		/// Tests the update of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateGenderMaleConcept()
		{
			var actual = new ADT_A01().PID;

			NotifierBase.UpdateGender("male", actual);

			Assert.AreEqual("M", actual.Sex.Value);
		}

		/// <summary>
		/// Tests the update of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateGenderMaleConceptKey()
		{
			var actual = new ADT_A01().PID;

			NotifierBase.UpdateGender("f4e3a6bb-612e-46b2-9f77-ff844d971198", actual);

			Assert.AreEqual("M", actual.Sex.Value);
		}

		/// <summary>
		/// Tests the update of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateGenderUndifferentiatedConcept()
		{
			var actual = new ADT_A01().PID;

			NotifierBase.UpdateGender("undifferentiated", actual);

			Assert.AreEqual("U", actual.Sex.Value);
		}

		/// <summary>
		/// Tests the update of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateGenderUndifferentiatedConceptKey()
		{
			var actual = new ADT_A01().PID;

			NotifierBase.UpdateGender("ae94a782-1485-4241-9bca-5b09db2156bf", actual);

			Assert.AreEqual("U", actual.Sex.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="MSH"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateMSH()
		{
			var actual = this.genericMessage.GetStructure("MSH") as MSH;

			NotifierBase.UpdateMSH(actual, null, this.configuration);

			Assert.AreEqual("AL", actual.AcceptAcknowledgmentType.Value);
			Assert.AreEqual("UnitTestDevice", actual.ReceivingApplication.NamespaceID.Value);
			Assert.AreEqual("TestNotificationDomain", actual.ReceivingFacility.NamespaceID.Value);
			Assert.AreEqual("OpenIZ", actual.SendingApplication.NamespaceID.Value);
			Assert.AreEqual("OpenIZ", actual.SendingFacility.NamespaceID.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="MSH"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateMSHNoReceivingFacility()
		{
			var actual = this.genericMessage.GetStructure("MSH") as MSH;

			this.configuration.NotificationDomainConfigurations.Clear();
			NotifierBase.UpdateMSH(actual, null, this.configuration);

			Assert.AreEqual("AL", actual.AcceptAcknowledgmentType.Value);
			Assert.AreEqual("UnitTestDevice", actual.ReceivingApplication.NamespaceID.Value);
			Assert.IsNull(actual.ReceivingFacility.NamespaceID.Value);
			Assert.AreEqual("OpenIZ", actual.SendingApplication.NamespaceID.Value);
			Assert.AreEqual("OpenIZ", actual.SendingFacility.NamespaceID.Value);
		}

		/// <summary>
		/// Tests the updating of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdatePID()
		{
			var actual = new ADT_A01().PID;

			Configuration.TargetConfiguration configuration = new Configuration.TargetConfiguration("Test", "llp://localhost:2100", "PAT_IDENTITY_SRC", "UnitTestDevice");

			configuration.NotificationDomainConfigurations.Add(new Configuration.NotificationDomainConfiguration("TestNotificationDomain"));

			NotifierBase.UpdatePID(this.patient, actual, configuration);

			Assert.AreEqual("M", actual.Sex.Value);
			Assert.AreEqual("19700101000000.000-0500", actual.DateTimeOfBirth.TimeOfAnEvent.Value);

			var mothersName = actual.GetMotherSMaidenName(0);

			Assert.AreEqual("Smith", mothersName.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Mary L A", mothersName.GivenName.Value);

			var name = actual.GetPatientName(0);

			Assert.AreEqual("Khanna", name.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Nityan David", name.GivenName.Value);
		}

		/// <summary>
		/// Tests the updating of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdatePIDGenderConceptKeyFemale()
		{
			var actual = new ADT_A01().PID;

			this.patient.GenderConcept = null;
			this.patient.GenderConceptKey = Guid.Parse("094941e9-a3db-48b5-862c-bc289bd7f86c");

			this.patient.Names.Add(new EntityName(NameUseKeys.Search, "Norgate", "Andrew"));

			NotifierBase.UpdatePID(this.patient, actual, this.configuration);

			Assert.AreEqual("F", actual.Sex.Value);
			Assert.AreEqual("19700101000000.000-0500", actual.DateTimeOfBirth.TimeOfAnEvent.Value);

			var mothersName = actual.GetMotherSMaidenName(0);

			Assert.AreEqual("Smith", mothersName.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Mary L A", mothersName.GivenName.Value);

			var nameRepOne = actual.GetPatientName(0);

			Assert.AreEqual("Khanna", nameRepOne.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Nityan David", nameRepOne.GivenName.Value);

			var nameRepTwo = actual.GetPatientName(1);

			Assert.AreEqual("Norgate", nameRepTwo.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Andrew", nameRepTwo.GivenName.Value);
		}

		/// <summary>
		/// Tests the updating of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdatePIDGenderConceptKeyMale()
		{
			var actual = new ADT_A01().PID;

			this.patient.GenderConcept = null;
			this.patient.GenderConceptKey = Guid.Parse("f4e3a6bb-612e-46b2-9f77-ff844d971198");

			this.patient.Names.Add(new EntityName(NameUseKeys.Search, "Norgate", "Andrew"));

			NotifierBase.UpdatePID(this.patient, actual, this.configuration);

			Assert.AreEqual("M", actual.Sex.Value);
			Assert.AreEqual("19700101000000.000-0500", actual.DateTimeOfBirth.TimeOfAnEvent.Value);

			var mothersName = actual.GetMotherSMaidenName(0);

			Assert.AreEqual("Smith", mothersName.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Mary L A", mothersName.GivenName.Value);

			var nameRepOne = actual.GetPatientName(0);

			Assert.AreEqual("Khanna", nameRepOne.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Nityan David", nameRepOne.GivenName.Value);

			var nameRepTwo = actual.GetPatientName(1);

			Assert.AreEqual("Norgate", nameRepTwo.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Andrew", nameRepTwo.GivenName.Value);
		}

		/// <summary>
		/// Tests the updating of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdatePIDGenderConceptKeyUndifferentiated()
		{
			var actual = new ADT_A01().PID;

			this.patient.GenderConcept = null;
			this.patient.GenderConceptKey = Guid.Parse("ae94a782-1485-4241-9bca-5b09db2156bf");

			this.patient.Names.Add(new EntityName(NameUseKeys.Search, "Norgate", "Andrew"));

			NotifierBase.UpdatePID(this.patient, actual, this.configuration);

			Assert.AreEqual("U", actual.Sex.Value);
			Assert.AreEqual("19700101000000.000-0500", actual.DateTimeOfBirth.TimeOfAnEvent.Value);

			var mothersName = actual.GetMotherSMaidenName(0);

			Assert.AreEqual("Smith", mothersName.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Mary L A", mothersName.GivenName.Value);

			var nameRepOne = actual.GetPatientName(0);

			Assert.AreEqual("Khanna", nameRepOne.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Nityan David", nameRepOne.GivenName.Value);

			var nameRepTwo = actual.GetPatientName(1);

			Assert.AreEqual("Norgate", nameRepTwo.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Andrew", nameRepTwo.GivenName.Value);
		}

		/// <summary>
		/// Tests the updating of a <see cref="PID"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdatePIDMultipleNames()
		{
			var actual = new ADT_A01().PID;

			this.patient.Names.Add(new EntityName(NameUseKeys.Search, "Norgate", "Andrew"));

			NotifierBase.UpdatePID(this.patient, actual, this.configuration);

			Assert.AreEqual("M", actual.Sex.Value);
			Assert.AreEqual("19700101000000.000-0500", actual.DateTimeOfBirth.TimeOfAnEvent.Value);

			var mothersName = actual.GetMotherSMaidenName(0);

			Assert.AreEqual("Smith", mothersName.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Mary L A", mothersName.GivenName.Value);

			var nameRepOne = actual.GetPatientName(0);

			Assert.AreEqual("Khanna", nameRepOne.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Nityan David", nameRepOne.GivenName.Value);

			var nameRepTwo = actual.GetPatientName(1);

			Assert.AreEqual("Norgate", nameRepTwo.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Andrew", nameRepTwo.GivenName.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="XPN"/> type.
		/// </summary>
		[TestMethod]
		public void TestUpdateXPN()
		{
			var actual = new XPN(this.genericMessage);

			NotifierBase.UpdateXPN(entityName, actual);

			Assert.AreEqual("Khanna", actual.FamilyLastName.FamilyName.Value);
			Assert.AreEqual("Nityan David", actual.GivenName.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="XPN"/> type with a first name only.
		/// </summary>
		[TestMethod]
		public void TestUpdateXPNFirstNameOnly()
		{
			var actual = new XPN(this.genericMessage);

			this.entityName.Component = new List<EntityNameComponent>
			{
				new EntityNameComponent(NameComponentKeys.Given, "Nityan")
			};

			NotifierBase.UpdateXPN(this.entityName, actual);

			Assert.AreEqual("Nityan", actual.GivenName.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="XPN"/> type with a last name only.
		/// </summary>
		[TestMethod]
		public void TestUpdateXPNLastNameOnly()
		{
			var actual = new XPN(this.genericMessage);

			this.entityName.Component = new List<EntityNameComponent>
			{
				new EntityNameComponent(NameComponentKeys.Family, "Khanna")
			};

			NotifierBase.UpdateXPN(this.entityName, actual);

			Assert.AreEqual("Khanna", actual.FamilyLastName.FamilyName.Value);
		}
	}
}
