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
 * Date: 2016-10-1
 */

using MARC.HI.EHRS.SVC.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHapi.Base.Model;
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Message;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using System;
using System.Linq;

namespace OpenIZ.Messaging.HL7.Test
{
	/// <summary>
	/// Contains tests for the <see cref="MessageUtil"/> class.
	/// </summary>
	[TestClass]
	public class MessageUtilTest
	{
		/// <summary>
		/// The internal reference to the <see cref="EntityTelecomAddress"/> instance.
		/// </summary>
		private EntityTelecomAddress entityTelecomAddress;

		/// <summary>
		/// The internal reference to the <see cref="XAD"/> instance.
		/// </summary>
		private XAD xad;

		/// <summary>
		/// The internal reference to the <see cref="XPN"/> instance.
		/// </summary>
		private XPN xpn;

		/// <summary>
		/// The internal reference to the <see cref="XTN"/> instance.
		/// </summary>
		private XTN xtn;

		/// <summary>
		/// Runs cleanup after all tests have been completed.
		/// </summary>
		[ClassCleanup]
		public static void ClassCleanup()
		{
			ApplicationContext.Current.Dispose();
		}

		/// <summary>
		/// Runs initialization before any tests have started.
		/// </summary>
		/// <param name="context"></param>
		[ClassInitialize]
		public static void ClassStartup(TestContext context)
		{
			ApplicationContext.Current.Start();
		}

		/// <summary>
		/// Runs cleanup after each test execution.
		/// </summary>
		[TestCleanup]
		public void Cleanup()
		{
			this.entityTelecomAddress = null;
			this.xad = null;
			this.xpn = null;
			this.xtn = null;
		}

		/// <summary>
		/// Runs initialization before each test execution.
		/// </summary>
		[TestInitialize]
		public void Initialize()
		{
			this.entityTelecomAddress = new EntityTelecomAddress
			{
				AddressUse = new Core.Model.DataTypes.Concept
				{
					Key = TelecomAddressUseKeys.Public
				},
				Value = "9055751212"
			};

			this.xad = new XAD(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);

			this.xad.AddressType.Value = "L";
			this.xad.City.Value = "Hamilton";
			this.xad.Country.Value = "Canada";
			this.xad.StateOrProvince.Value = "Ontario";
			this.xad.StreetAddress.StreetOrMailingAddress.Value = "123 Main street west";
			this.xad.StreetAddress.StreetName.Value = "Main St";
			this.xad.ZipOrPostalCode.Value = "L8N3T2";

			this.xpn = new XPN(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);

			this.xpn.DegreeEgMD.Value = "MD";
			this.xpn.FamilyName.Surname.Value = "Khanna";
			this.xpn.GivenName.Value = "Nityan";
			this.xpn.PrefixEgDR.Value = "Dr.";
			this.xpn.SecondAndFurtherGivenNamesOrInitialsThereof.Value = "Dave";

			this.xtn = new XTN(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);
		}

		/// <summary>
		/// Test the conversion of an address.
		/// </summary>
		[TestMethod]
		public void TestConvertAddress()
		{
			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of an address with an empty city value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressEmptyCity()
		{
			this.xad.City.Value = string.Empty;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.City)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of an address with an empty country value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressEmptyCountry()
		{
			this.xad.Country.Value = string.Empty;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.Country)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of an address with an empty postal code value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressEmptyPostalCode()
		{
			this.xad.ZipOrPostalCode.Value = string.Empty;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode)?.Value);
		}

		/// <summary>
		/// Tests the conversion of an address with an empty state value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressEmptyState()
		{
			this.xad.StateOrProvince.Value = string.Empty;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.State)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of an address with an empty street value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressEmptyStreet()
		{
			this.xad.StreetAddress.StreetName.Value = string.Empty;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.StreetName)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of an address with a null city value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressNullCity()
		{
			this.xad.City.Value = null;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.City)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of an address with a null country value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressNullCountry()
		{
			this.xad.Country.Value = null;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.Country)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of an address with a null postal code value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressNullPostalCode()
		{
			this.xad.ZipOrPostalCode.Value = null;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode)?.Value);
		}

		/// <summary>
		/// Tests the conversion of an address with a null state value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressNullState()
		{
			this.xad.StateOrProvince.Value = null;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.State)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.AreEqual("Main St", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of an address with a null street value.
		/// </summary>
		[TestMethod]
		public void TestConvertAddressNullStreet()
		{
			this.xad.StreetAddress.StreetName.Value = null;

			var actual = MessageUtil.ConvertAddress(xad);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.City));
			Assert.AreEqual("Hamilton", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.City).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.Country));
			Assert.AreEqual("Canada", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.Country).Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.State));
			Assert.AreEqual("Ontario", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.State).Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.StreetName));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == AddressComponentKeys.StreetName)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode));
			Assert.AreEqual("L8N3T2", actual.Component.First(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Value);
		}

		/// <summary>
		/// Tests the conversion of a name.
		/// </summary>
		[TestMethod]
		public void TestConvertName()
		{
			this.xpn = new XPN(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);

			this.xpn.FamilyName.Surname.Value = "Khanna";
			this.xpn.GivenName.Value = "Nityan";

			var actual = MessageUtil.ConvertName(this.xpn);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == NameComponentKeys.Given));
			Assert.AreEqual("Nityan", actual.Component.FirstOrDefault(c => c.ComponentTypeKey == NameComponentKeys.Given)?.Value);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == NameComponentKeys.Family));
			Assert.AreEqual("Khanna", actual.Component.FirstOrDefault(c => c.ComponentTypeKey == NameComponentKeys.Family)?.Value);
		}

		/// <summary>
		/// Tests the conversion of a name with a famiy name only.
		/// </summary>
		[TestMethod]
		public void TestConvertNameFamilyNameOnly()
		{
			this.xpn = new XPN(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);

			this.xpn.FamilyName.Surname.Value = "Khanna";

			var actual = MessageUtil.ConvertName(this.xpn);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == NameComponentKeys.Family));
			Assert.AreEqual("Khanna", actual.Component.FirstOrDefault(c => c.ComponentTypeKey == NameComponentKeys.Family)?.Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == NameComponentKeys.Given));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == NameComponentKeys.Given)?.Value);
		}

		/// <summary>
		/// Tests the conversion of a name with a given name only.
		/// </summary>
		[TestMethod]
		public void TestConvertNameGivenNameOnly()
		{
			this.xpn = new XPN(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);

			this.xpn.GivenName.Value = "Nityan";

			var actual = MessageUtil.ConvertName(this.xpn);

			Assert.AreEqual(1, actual.Component.Count(c => c.ComponentTypeKey == NameComponentKeys.Given));
			Assert.AreEqual("Nityan", actual.Component.FirstOrDefault(c => c.ComponentTypeKey == NameComponentKeys.Given)?.Value);

			Assert.AreEqual(0, actual.Component.Count(c => c.ComponentTypeKey == NameComponentKeys.Family));
			Assert.IsNull(actual.Component.FirstOrDefault(c => c.ComponentTypeKey == NameComponentKeys.Family)?.Value);
		}

		/// <summary>
		/// Test the conversion of multiple names.
		/// </summary>
		[TestMethod]
		public void TestConvertNameMultipleNames()
		{
			var adt = new ADT_A01();

			var name1 = adt.PID.GetPatientName(0);

			name1.FamilyName.Surname.Value = "Khanna";
			name1.GivenName.Value = "Nityan";
			name1.SecondAndFurtherGivenNamesOrInitialsThereof.Value = "Dave";

			var name2 = adt.PID.GetPatientName(1);

			name2.FamilyName.Surname.Value = "Smith";
			name2.GivenName.Value = "II";
			name2.SecondAndFurtherGivenNamesOrInitialsThereof.Value = "Capitano";

			var names = new XPN[2];

			names[0] = name1;
			names[1] = name2;

			var actual = MessageUtil.ConvertNames(names);

			Assert.AreEqual(2, actual.Count(n => n.Component.Any(c => c.ComponentTypeKey == NameComponentKeys.Given)));
			Assert.AreEqual(2, actual.Count(n => n.Component.Any(c => c.ComponentTypeKey == NameComponentKeys.Family)));
		}

		/// <summary>
		/// Tests the conversion of a date time.
		/// </summary>
		[TestMethod]
		public void TestConvertTSDateTime()
		{
			var ts = new TS(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);

			Assert.IsNotNull(ts);

			ts.Time.Value = new DateTime(1970, 01, 01).ToString("yyyyMMddHHmmss");

			var actual = MessageUtil.ConvertTS(ts);

			Assert.IsTrue(actual.HasValue);
			Assert.AreEqual(new DateTime(1970, 01, 01), actual.Value);
		}

		/// <summary>
		/// Tests the conversion of a date time offset.
		/// </summary>
		[TestMethod]
		public void TestConvertTSDateTimeOffset()
		{
			var ts = new TS(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);

			Assert.IsNotNull(ts);

			ts.Time.Value = new DateTimeOffset(1970, 01, 01, 0, 0, 0, 0, TimeSpan.FromHours(12)).ToString("yyyyMMddHHmmss");

			var actual = MessageUtil.ConvertTS(ts);

			Assert.IsTrue(actual.HasValue);
			Assert.AreEqual(new DateTime(1970, 01, 01), actual.Value);
		}

		/// <summary>
		/// Tests that the AnyText value property is set, when "tel:" is not provided.
		/// </summary>
		[TestMethod]
		public void TestEntityTelecomToXTN()
		{
			MessageUtil.XTNFromTel(this.entityTelecomAddress, this.xtn);

			Assert.AreEqual("9055751212", this.xtn.AnyText.Value);
		}
	}
}