/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Messaging.HAPI.TransportProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHapi.Model.V25.Message;
using System;

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
		/// The internal reference to the <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestContext"/> instance.
		/// </summary>
		private TestContext context;

		/// <summary>
		/// The internal reference to the <see cref="ADT_A01"/> instance.
		/// </summary>
		private ADT_A01 message = null;

		/// <summary>
		/// The internal reference to the <see cref="AdtMessageHandler"/> instance.
		/// </summary>
		private AdtMessageHandler messageHandler;

		/// <summary>
		/// Gets or sets the test context.
		/// </summary>
		public TestContext TestContext
		{
			get { return this.context; }
			set { this.context = value; }
		}

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
			this.messageHandler = new AdtMessageHandler();
			this.args = new Hl7MessageReceivedEventArgs(this.message, new Uri("llp://localhost:2100"), new Uri("llp://localhost:2100"), DateTime.Now);
		}

		[TestMethod]
		public void TestValidMessage()
		{
			var actual = this.messageHandler.HandleMessage(this.args);

			Assert.IsNotNull(actual);
		}
	}
}