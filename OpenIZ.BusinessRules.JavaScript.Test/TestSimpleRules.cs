using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.IO;

namespace OpenIZ.BusinessRules.JavaScript.Test
{
	/// <summary>
	/// Test simple business rules
	/// </summary>
	[TestClass]
	public class TestSimpleRules
	{
		/// <summary>
		/// Load simple rules
		/// </summary>
		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			ApplicationServiceContext.Current = new SimpleServiceContext();

			// Step 1 : Rules rules rules!
			using (var stream = typeof(TestSimpleRules).Assembly.GetManifestResourceStream("OpenIZ.BusinessRules.JavaScript.Test.TestRules.SimplePatientRule.js"))
			using (StreamReader sr = new StreamReader(stream))
				JavascriptBusinessRulesEngine.Current.AddRules(sr);
		}

		/// <summary>
		/// Tests that a patient should fail validation according to the validation function in the JS file
		/// </summary>
		[TestMethod]
		public void PatientShouldFailValidation()
		{
			var breService = ApplicationServiceContext.Current.GetService(typeof(IBusinessRulesService<Patient>)) as IBusinessRulesService<Patient>;
			Assert.IsNotNull(breService);
			var issues = breService.Validate(new Patient());
			Assert.AreEqual(1, issues.Count);
			Assert.IsTrue(issues.Exists(o => o.Text == "NoGender"));
		}

		/// <summary>
		/// Tests that a patient should fail validation according to the validation function in the JS file
		/// </summary>
		[TestMethod]
		public void PatientShouldPassValidation()
		{
			var breService = ApplicationServiceContext.Current.GetService(typeof(IBusinessRulesService<Patient>)) as IBusinessRulesService<Patient>;
			Assert.IsNotNull(breService);
			var issues = breService.Validate(new Patient() { GenderConceptKey = Guid.NewGuid() });
			Assert.AreEqual(0, issues.Count);
			Assert.IsFalse(issues.Exists(o => o.Text == "NoGender"));
		}

		/// <summary>
		/// Test that the rules engine successfully parses and interprets rules
		/// </summary>
		[TestMethod]
		public void ShouldAddBusinessRuleTest()
		{
			Assert.IsNotNull(JavascriptBusinessRulesEngine.Current.GetValidators<Patient>());
			Assert.IsNotNull(JavascriptBusinessRulesEngine.Current.GetCallList<Patient>("AfterInsert"));
		}

		[TestMethod]
		public void TestShouldReturnComplexObject()
		{
			Func<String, Object[]> callback = null;
			Func<Func<string, Object[]>, object> registerCallback = (o) => callback = o;
			Engine engine = new Engine()
				.SetValue("registerCallback", registerCallback)
				.Execute("registerCallback(function(parameter) { return [ 'a','b','c' ]; });");

			Assert.IsNotNull(callback);
			var result = callback("test");
		}

		private class Convert : IObjectConverter
		{
			/// <summary>
			/// Try convert
			/// </summary>
			/// <param name="value"></param>
			/// <param name="result"></param>
			/// <returns></returns>
			public bool TryConvert(object value, out JsValue result)
			{
				result = null;
				return false;
			}
		}
	}
}