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
