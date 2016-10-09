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
 * Date: 2016-10-6
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Messaging.HAPI;
using MARC.HI.EHRS.SVC.Messaging.HAPI.TransportProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHapi.Model.V25.Message;

namespace OpenIZ.Messaging.HL7.Test
{
	/// <summary>
	/// Contains tests for the <see cref="AdtMessageHandler"/> class.
	/// </summary>
	[TestClass]
	public class AdtMessageHandlerTest
	{
		/// <summary>
		/// The internal reference to the <see cref="Hl7MessageReceivedEventArgs"/> instance.
		/// </summary>
		private Hl7MessageReceivedEventArgs args;

		/// <summary>
		/// The internal reference to the <see cref="ADT_A01"/> instance.
		/// </summary>
		private ADT_A01 message = null;

		private TestContext context;

		public TestContext TestContext
		{
			get { return this.context; }
			set { this.context = value; }
		}

		[ClassCleanup]
		public static void ClassCleanup()
		{
			ApplicationContext.Current.Dispose();
		}

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
			this.args = null;
			this.message = null;
		}

		/// <summary>
		/// Runs initialization before each test execution.
		/// </summary>
		[TestInitialize]
		public void Initialize()
		{
			this.message = new ADT_A01();
			this.args = new Hl7MessageReceivedEventArgs(this.message, new Uri("llp://localhost:2100"), new Uri("llp://localhost:2100"), DateTime.Now);
		}

		[TestMethod]
		public void TestValidMessage()
		{
			var messageHandler = ApplicationContext.Current.GetService<IHL7MessageHandler>();

			Assert.IsNotNull(messageHandler);

			var actual = messageHandler.HandleMessage(this.args);
		}

	}
}
