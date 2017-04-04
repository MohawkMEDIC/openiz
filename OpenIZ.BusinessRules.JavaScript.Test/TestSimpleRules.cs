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
 * Date: 2016-11-8
 */
using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;

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
            (ApplicationServiceContext.Current as IServiceManager).AddServiceProvider(typeof(TestDataReferenceResolver));

            var names = typeof(TestSimpleRules).Assembly.GetManifestResourceNames();

            var streams = names.Select(n => typeof(TestSimpleRules).Assembly.GetManifestResourceStream(n));


            foreach (var stream in streams)
            {
                using (stream)
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    JavascriptBusinessRulesEngine.Current.AddRules(streamReader);
                }
            }
        }

        /// <summary>
        /// Ensures that the reference range is set properly
        /// </summary>
        [TestMethod]
        public void ObservationShouldSetReferenceRange ()
        {
            QuantityObservation qobs = new QuantityObservation()
            {
                Value = (Decimal)2.3,
                Participations = new Core.Model.Collection.VersionedAssociationCollection<ActParticipation>()
                {
                    new ActParticipation()
                    {
                        ParticipationRole = new Core.Model.DataTypes.Concept() { Mnemonic = "RecordTarget" },
                        PlayerEntity = new Patient()
                        {
                            DateOfBirth = DateTime.Now,
                            GenderConcept = new Core.Model.DataTypes.Concept()
                            {
                                Key = Guid.Parse("094941e9-a3db-48b5-862c-bc289bd7f86c"),
                                Mnemonic = "Female"
                            }
                        }
                    }
                },
                TypeConceptKey = Guid.Parse("a261f8cd-69b0-49aa-91f4-e6d3e5c612ed")
            };

            var breService = ApplicationServiceContext.Current.GetService(typeof(IBusinessRulesService<QuantityObservation>)) as IBusinessRulesService<QuantityObservation>;
            Assert.IsNotNull(breService);
            qobs = breService.BeforeInsert(qobs);
            Assert.IsNotNull(qobs.InterpretationConceptKey);
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
        public void TestActBusinessRule()
        {
            Assert.IsNotNull(JavascriptBusinessRulesEngine.Current.GetCallList<Act>("AfterInsert"));
        }

        [TestMethod]
        public void TestManufacturedMaterialBusinessRule()
        {
            Assert.IsNotNull(JavascriptBusinessRulesEngine.Current.GetCallList<ManufacturedMaterial>("AfterInsert"));
        }

        /// <summary>
        /// Test the act returns a complex object
        /// </summary>
		[TestMethod]
        public void TestShouldReturnComplexObject()
        {
            Func<String, ExpandoObject> callback = null;
            Func<Func<string, ExpandoObject>, object> registerCallback = (o) => callback = o;
            Engine engine = new Engine()
                .SetValue("registerCallback", registerCallback)
                .Execute("registerCallback(function(parameter) { return { foo : 1 }; });");

            Assert.IsNotNull(callback);
            var result = callback("test");
        }

        /// <summary>
        /// Tests that the business rule is able to convert a model object
        /// </summary>
        [TestMethod]
        public void TestShouldConvertModelObject()
        {

            var breService = ApplicationServiceContext.Current.GetService(typeof(IBusinessRulesService<Patient>)) as IBusinessRulesService<Patient>;
            Assert.IsNotNull(breService);

            var patient = breService.AfterInsert(new Patient()
            {
                GenderConcept = new Core.Model.DataTypes.Concept()
                {
                    Mnemonic = "Female"
                },
                Names = new System.Collections.Generic.List<EntityName>()
                {
                    new EntityName()
                    {
                        NameUse = new Core.Model.DataTypes.Concept() { Mnemonic = "Legal", Key = NameUseKeys.Legal },
                        Component = new System.Collections.Generic.List<EntityNameComponent>()
                        {
                            new EntityNameComponent()
                            {
                                ComponentType = new Core.Model.DataTypes.Concept() { Mnemonic = "Family" }, Value = "Smith"
                            },
                            new EntityNameComponent()
                            {
                                ComponentType = new Core.Model.DataTypes.Concept() { Mnemonic = "Given" }, Value = "James"
                            }

                        }
                    }
                },
                Participations = new System.Collections.Generic.List<ActParticipation>()
                {
                    new ActParticipation()
                    {
                        ParticipationRole = new Core.Model.DataTypes.Concept() { Mnemonic = "RecordTarget", Key = ActParticipationKey.RecordTarget },
                        Act = new QuantityObservation()
                        {
                            Value = (decimal)1.2,
                            MoodConceptKey = ActMoodKeys.Eventoccurrence,
                            UnitOfMeasure = new Core.Model.DataTypes.Concept() {Mnemonic = "UnitOfMeasure_Kilograms" },
                            InterpretationConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "Interpretation_Normal" }
                        }
                    }
                }
            });

            Assert.IsInstanceOfType(patient, typeof(Patient));
            Assert.AreEqual(1, patient.Participations.Count);
            Assert.AreEqual("Female", patient.GenderConcept.Mnemonic);
            Assert.AreEqual(2, patient.Names.FirstOrDefault().Component.Count);
            Assert.AreEqual("RecordTarget", patient.Participations[0].ParticipationRole.Mnemonic);
            Assert.AreEqual("UnitOfMeasure_Kilograms", (patient.Participations[0].Act as QuantityObservation).UnitOfMeasure.Mnemonic);
            Assert.IsTrue(patient.DateOfBirth.HasValue);
        }

    }
}