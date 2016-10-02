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
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Message;
using NHapi.Base.Model;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Constants;

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
		/// The internal reference to the <see cref="XTN"/> instance.
		/// </summary>
		private XTN xtn;

		/// <summary>
		/// Runs cleanup after each test execution.
		/// </summary>
		[TestCleanup]
		public void Cleanup()
		{
			this.entityTelecomAddress = null;
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

			this.xtn = new XTN(Activator.CreateInstance(typeof(ADT_A01)) as IMessage);
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
