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
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Message;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.HL7.Notifier;
using NHapi.Model.V25.Segment;

namespace OpenIZ.Messaging.HL7.Test
{
	[TestClass]
	public class NotifierBaseTest
	{
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
		/// Runs cleanup after each test execution.
		/// </summary>
		[TestCleanup]
		public void Cleanup()
		{
			this.entityAddress = null;
			this.entityName = null;
			this.genericMessage = null;
		}

		/// <summary>
		/// Runs initialization before each test execution.
		/// </summary>
		[TestInitialize]
		public void Initialize()
		{
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
			Assert.AreEqual("123 Main street west", actual.StreetAddress.StreetOrMailingAddress.Value);
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
			Assert.AreEqual(string.Empty, actual.StreetAddress.StreetOrMailingAddress.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="MSH"/> segment.
		/// </summary>
		[TestMethod]
		public void TestUpdateMSH()
		{
			var actual = this.genericMessage.GetStructure("MSH") as MSH;

			Configuration.TargetConfiguration configuration = new Configuration.TargetConfiguration("Test", "llp://localhost:2100", "PAT_IDENTITY_SRC", "UnitTestDevice");

			configuration.NotificationDomainConfigurations.Add(new Configuration.NotificationDomainConfiguration("TestNotificationDomain"));

			NotifierBase.UpdateMSH(actual, configuration);

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

			Configuration.TargetConfiguration configuration = new Configuration.TargetConfiguration("Test", "llp://localhost:2100", "PAT_IDENTITY_SRC", "UnitTestDevice");

			NotifierBase.UpdateMSH(actual, configuration);

			Assert.AreEqual("AL", actual.AcceptAcknowledgmentType.Value);
			Assert.AreEqual("UnitTestDevice", actual.ReceivingApplication.NamespaceID.Value);
			Assert.IsNull(actual.ReceivingFacility.NamespaceID.Value);
			Assert.AreEqual("OpenIZ", actual.SendingApplication.NamespaceID.Value);
			Assert.AreEqual("OpenIZ", actual.SendingFacility.NamespaceID.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="XPN"/> type.
		/// </summary>
		[TestMethod]
		public void TestUpdateXPN()
		{
			var actual = new XPN(this.genericMessage);

			NotifierBase.UpdateXPN(entityName, actual);

			Assert.AreEqual("Khanna", actual.FamilyName.Surname.Value);
			Assert.AreEqual("Nityan David", actual.GivenName.Value);
		}

		/// <summary>
		/// Tests the updating of an <see cref="XPN"/> type with a first name only.
		/// </summary>
		[TestMethod]
		public void TestUpdateXPNFirstNameOnly()
		{
			var actual = new XPN(this.genericMessage);

			this.entityName.Component = new List<EntityNameComponent>();

			this.entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Given, "Nityan"));

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

			this.entityName.Component = new List<EntityNameComponent>();

			this.entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Family, "Khanna"));

			NotifierBase.UpdateXPN(this.entityName, actual);

			Assert.AreEqual("Khanna", actual.FamilyName.Surname.Value);
		}
	}
}
