using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Services.Impl;
using OpenIZ.Core.Model.Patch;
using OpenIZ.Core.Model.Roles;
using System.IO;
using System.Xml.Serialization;
using OpenIZ.Core.Applets.ViewModel.Json;
using OpenIZ.Core.Model.Constants;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Serialization;
using System.Linq;
using OpenIZ.Core.Model.Acts;

namespace OpenIZ.Core.PCL.Test
{
    /// <summary>
    /// Represents a unit test which tests the patching ability 
    /// </summary>
    [TestClass]
    public class SimplePatchServiceTest
    {

        /// <summary>
        /// Serialize patch
        /// </summary>
        private void SerializePatch(Patch patch)
        {
            String patchXml = String.Empty;
            using (StringWriter sw = new StringWriter())
            {
                var xsz = new XmlSerializer(typeof(Patch));
                xsz.Serialize(sw, patch);
                patchXml = sw.ToString();
            }

            var jser = new JsonViewModelSerializer();
            string patchJson = JsonConvert.SerializeObject(patch, Formatting.Indented, new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = new ModelSerializationBinder()
            });
        }

        /// <summary>
        /// Tests that the diff method does not generate a patch for the same object
        /// </summary>
        [TestMethod]
        public void DiffShouldNotGeneratePatchForIdentical()
        {
            SecurityUser a = new SecurityUser()
            {
                Key = Guid.Empty,
                UserName = "pepe",
                PasswordHash = "pepelepew",
                SecurityHash = Guid.NewGuid().ToString(),
                Email = "pepe@acme.com"
            };

            var patchService = new SimplePatchService();
            var patch = patchService.Diff(a, a);
            Assert.IsNotNull(patch);
            Assert.AreEqual(0, patch.Operation.Count);
        }

        /// <summary>
        /// Tests whether the diff engine detects a simple assignment of a property
        /// </summary>
        [TestMethod]
        public void DiffShouldDetectSimplePropertyAssignment()
        {
            SecurityUser a = new SecurityUser()
            {
                Key = Guid.Empty,
                UserName = "pepe",
                PasswordHash = "pepelepew",
                SecurityHash = Guid.NewGuid().ToString(),
                Email = "pepe@acme.com"
            },
            b = new SecurityUser()
            {
                Key = Guid.Empty,
                UserName = "pepe",
                PasswordHash = "pepelepew",
                SecurityHash = Guid.NewGuid().ToString(),
                Email = "lepew@acme.com"
            };

            // Patch service 
            SimplePatchService patchService = new SimplePatchService();
            var patch = patchService.Diff(a, b);
            var patchString = patch.ToString();
            Assert.IsNotNull(patch);
            Assert.AreEqual(5, patch.Operation.Count);

            // Assert test
            Assert.AreEqual(PatchOperationType.Test, patch.Operation[0].OperationType);
            Assert.AreEqual(PatchOperationType.Test, patch.Operation[1].OperationType);
            Assert.AreEqual("email", patch.Operation[1].Path);
            Assert.AreEqual("pepe@acme.com", patch.Operation[1].Value);

            // Assert replace
            Assert.AreEqual(PatchOperationType.Replace, patch.Operation[2].OperationType);
            Assert.AreEqual("email", patch.Operation[2].Path);
            Assert.AreEqual("lepew@acme.com", patch.Operation[2].Value);
        }

        /// <summary>
        /// Tests that patch cascades to sub object
        /// </summary>
        [TestMethod]
        public void DiffShouldGenerateForSubMembers()
        {

            Patient a = new Patient()
            {
                Key = Guid.Empty,
                VersionKey = Guid.NewGuid(),
                DateOfBirth = DateTime.MinValue,
                DateOfBirthPrecision = Model.DataTypes.DatePrecision.Full,
                Identifiers = new System.Collections.Generic.List<Model.DataTypes.EntityIdentifier>()
                {
                    new Model.DataTypes.EntityIdentifier(Guid.Empty, "1234") { Key = Guid.NewGuid() }
                }
            },
            b = new Patient()
            {
                Key = Guid.Empty,
                VersionKey = Guid.NewGuid(),
                DateOfBirth = DateTime.MaxValue,
                DateOfBirthPrecision = Model.DataTypes.DatePrecision.Day,
                Identifiers = new System.Collections.Generic.List<Model.DataTypes.EntityIdentifier>()
                {
                    new Model.DataTypes.EntityIdentifier(Guid.Empty, "1234") { Key = a.Identifiers[0].Key },
                    new Model.DataTypes.EntityIdentifier(Guid.NewGuid(), "3245") { Key = Guid.NewGuid() }
                },
                Names = new System.Collections.Generic.List<Model.Entities.EntityName>()
                {
                    new Model.Entities.EntityName(NameUseKeys.Legal, "Smith", "Joe") { NameUse = new Model.DataTypes.Concept() { Key = NameUseKeys.Legal, Mnemonic = "Legal" } }
                }
            };

            var patchService = new SimplePatchService();
            var patch = patchService.Diff(a, b);
            var patchString = patch.ToString();

            // Assert that there is a patch
            Assert.IsNotNull(patch);
            Assert.AreEqual(11, patch.Operation.Count);
            Assert.AreEqual(PatchOperationType.Add, patch.Operation[7].OperationType);
            Assert.AreEqual(b.Identifiers[1], patch.Operation[7].Value);
            Assert.AreEqual("identifier", patch.Operation[7].Path);

            this.SerializePatch(patch);            
        }

        /// <summary>
        /// Tests that the Diff method removes items from a collection where the key is the same but the value is different
        /// </summary>
        [TestMethod]
        public void DiffShouldRemoveNameWithSameValues()
        {
            Patient a = new Patient()
            {
                Key = Guid.Empty,
                VersionKey = Guid.NewGuid(),
                DateOfBirth = DateTime.MaxValue,
                DateOfBirthPrecision = Model.DataTypes.DatePrecision.Full,
                Identifiers = new System.Collections.Generic.List<Model.DataTypes.EntityIdentifier>()
                {
                    new Model.DataTypes.EntityIdentifier(Guid.Empty, "1234") { Key = Guid.NewGuid() },
                    new Model.DataTypes.EntityIdentifier(Guid.NewGuid(), "3245") { Key = Guid.NewGuid() }
                },
                Names = new System.Collections.Generic.List<Model.Entities.EntityName>()
                {
                    new Model.Entities.EntityName(NameUseKeys.Legal, "Smith", "Joe") { Key = Guid.NewGuid() }
                },
                Tags = new System.Collections.Generic.List<Model.DataTypes.EntityTag>()
                {
                    new Model.DataTypes.EntityTag("KEY", "VALUE")
                }
            },
            b = new Patient()
            {
                Key = Guid.Empty,
                VersionKey = Guid.NewGuid(),
                DateOfBirth = DateTime.MaxValue,
                DateOfBirthPrecision = Model.DataTypes.DatePrecision.Full,
                Identifiers = new System.Collections.Generic.List<Model.DataTypes.EntityIdentifier>()
                {
                    new Model.DataTypes.EntityIdentifier(Guid.Empty, "1234") { Key = a.Identifiers[0].Key },
                    new Model.DataTypes.EntityIdentifier(Guid.NewGuid(), "3245") { Key = a.Identifiers[1].Key }
                },
                Names = new System.Collections.Generic.List<Model.Entities.EntityName>()
                {
                    new Model.Entities.EntityName(NameUseKeys.Legal, "Smith", "Joseph") { Key = a.Names[0].Key }
                },
                Addresses = new System.Collections.Generic.List<Model.Entities.EntityAddress>()
                {
                    new Model.Entities.EntityAddress(AddressUseKeys.HomeAddress, "123 Main Street West", "Hamilton", "ON", "CA", "L8K5N2")
                },
                Tags = new System.Collections.Generic.List<Model.DataTypes.EntityTag>()
                {
                    new Model.DataTypes.EntityTag("KEY", "VALUE2")
                }
            };

            var patchService = new SimplePatchService();
            var patch = patchService.Diff(a, b);
            var patchString = patch.ToString();
            Assert.IsNotNull(patch);
            Assert.AreEqual(15, patch.Operation.Count);

            // Assert there is a remove operation for a name
            Assert.IsTrue(patch.Operation.Any(o => o.OperationType == PatchOperationType.Remove && o.Path.Contains(a.Names[0].Key.ToString())));
            Assert.IsTrue(patch.Operation.Any(o => o.OperationType == PatchOperationType.Remove && o.Path.Contains("tag.key = KEY")));
            this.SerializePatch(patch);
        }

        /// <summary>
        /// Tests that the diff function cascades to a nested single object
        /// </summary>
        [TestMethod]
        public void DiffShouldCascadeToNestedSingleObjectRef()
        {
            Act a = new QuantityObservation()
            {
                Template = new Model.DataTypes.TemplateDefinition()
                {
                    Mnemonic = "TESTTEMPLATE",
                    Key = Guid.NewGuid(),
                    Description = "This is a test"
                }
            },
            b = new QuantityObservation()
            {
                Template = new Model.DataTypes.TemplateDefinition()
                {
                    Mnemonic = "OBSERVATION",
                    Key = Guid.NewGuid(),
                    Description = "This is a different template"
                }
            };

            var patchService = new SimplePatchService();
            var patch = patchService.Diff(a, b);
            var patchString = patch.ToString();

        }
    }
}
