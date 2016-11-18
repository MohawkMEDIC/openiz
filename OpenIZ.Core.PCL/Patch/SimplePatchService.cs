using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Patch;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Interfaces;
using System.Collections;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a simple patch service which can calculate patches and apply them
    /// </summary>
    public class SimplePatchService : IPatchService
    {

        // Property information
        private Dictionary<Type, IEnumerable<PropertyInfo>> m_properties = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        // Lock object
        private object m_lockObject = new object();

        /// <summary>
        /// Perform a diff using a simple .compare() method
        /// </summary>
        /// <remarks>This method only performs a diff on the root object passed and does not cascade to collections</remarks>
        public Patch Diff(IdentifiedData existing, IdentifiedData updated)
        {
            return new Patch()
            {
                Key = Guid.NewGuid(),
                CreationTime = DateTimeOffset.Now,
                Operation = this.DiffInternal(existing, updated, null)
            };
        }

        /// <summary>
        /// Difference internal
        /// </summary>
        private List<PatchOperation> DiffInternal(IdentifiedData existing, IdentifiedData updated, String path)
        {
            // First are they the same?
            var retVal = new List<PatchOperation>();
            if (!existing.SemanticEquals(updated) && existing.Type == updated.Type)
            {
                // Get the properties
                IEnumerable<PropertyInfo> properties = null;
                if (!this.m_properties.TryGetValue(existing.GetType(), out properties))
                    lock (this.m_lockObject)
                        if (!this.m_properties.ContainsKey(existing.GetType()))
                        {
                            properties = existing.GetType().GetRuntimeProperties().Where(o => o.CanRead && o.CanWrite && o.GetCustomAttribute<JsonPropertyAttribute>() != null);
                            this.m_properties.Add(existing.GetType(), properties);
                        }

                // First, test that we're updating the right object
                retVal.Add(new PatchOperation(PatchOperationType.Test, $"{path}id", existing.Key));

                if (existing is IVersionedEntity)
                    retVal.Add(new PatchOperation(PatchOperationType.Test, $"{path}version", (existing as IVersionedEntity).VersionKey));
                // Iterate through properties and determine changes
                foreach (var pi in properties)
                {
                    var serializationName = pi.GetCustomAttribute<JsonPropertyAttribute>().PropertyName;
                    object existingValue = pi.GetValue(existing),
                        updatedValue = pi.GetValue(updated);


                    // Test
                    if (existingValue == updatedValue)
                        continue; // same 
                    else
                    {
                        if (existingValue != null && updatedValue == null) // remove
                        {
                            // Generate tests
                            retVal.AddRange(this.GenerateTests(existingValue, $"{path}{serializationName}"));
                            retVal.Add(new PatchOperation(PatchOperationType.Remove, $"{path}{serializationName}", null));
                        }
                        else if ((existingValue as IdentifiedData)?.SemanticEquals(updatedValue as IdentifiedData) == false) // They are different
                        {
                            // Generate tests
                            retVal.AddRange(this.GenerateTests(existingValue, $"{path}{serializationName}"));
                        }
                        else if (existingValue is IList && !existingValue.GetType().GetTypeInfo().IsArray)
                        {
                            // Generate tests
                            retVal.AddRange(this.GenerateTests(existingValue, $"{path}{serializationName}"));

                            // Simple or complex list?
                            if (typeof(IIdentifiedEntity).GetTypeInfo().IsAssignableFrom(existingValue.GetType().StripGeneric().GetTypeInfo()))
                            {
                                IEnumerable<IdentifiedData> updatedList = (updatedValue as IEnumerable).OfType<IdentifiedData>(),
                                    existingList = (existingValue as IEnumerable).OfType<IdentifiedData>();

                                // Removals
                                retVal.AddRange(existingList.Where(e => !updatedList.Any(u => e.SemanticEquals(u))).Select(c => new PatchOperation(PatchOperationType.Remove, $"{path}{serializationName}[{c.Key}]", null)));

                                // Additions 
                                retVal.AddRange(updatedList.Where(u => !existingList.Any(e => u.SemanticEquals(e))).Select(c => new PatchOperation(PatchOperationType.Add, $"{path}{serializationName}", c)));

                            }
                            else
                            {
                                IEnumerable<Object> updatedList = (updatedValue as IEnumerable).OfType<Object>(),
                                    existingList = (existingValue as IEnumerable).OfType<Object>();

                                // Removals
                                retVal.AddRange(existingList.Where(e => !updatedList.Any(u => e.Equals(u))).Select(c => new PatchOperation(PatchOperationType.Remove, $"{path}{serializationName}[{c}]", null)));

                                // Additions 
                                retVal.AddRange(updatedList.Where(u => !existingList.Any(e => u.Equals(e))).Select(c => new PatchOperation(PatchOperationType.Add, $"{path}{serializationName}", c)));

                            }
                        }
                        else if (existingValue?.Equals(updatedValue) == false)// simple value has changed
                        {
                            // Generate tests
                            retVal.AddRange(this.GenerateTests(existingValue, $"{path}{serializationName}"));
                            retVal.Add(new PatchOperation(PatchOperationType.Replace, $"{path}{serializationName}", updatedValue));
                        }
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Generate a test operation in the patch
        /// </summary>
        private IEnumerable<PatchOperation> GenerateTests(object existingValue, string path)
        {
            if (existingValue is IVersionedEntity)
                return new PatchOperation[]
                {
                    new PatchOperation(PatchOperationType.Test, $"{path}.version", (existingValue as IVersionedEntity).VersionKey),
                    new PatchOperation(PatchOperationType.Test, $"{path}.id", (existingValue as IVersionedEntity).Key)
                };
            else if (existingValue is IIdentifiedEntity)
                return new PatchOperation[]
                {
                    new PatchOperation(PatchOperationType.Test, $"{path}.id", (existingValue as IIdentifiedEntity).Key)
                };
            else if (existingValue is IList && !existingValue.GetType().GetTypeInfo().IsArray)
            {
                var values = existingValue as IList;
                var retVal = new List<PatchOperation>(values.Count);
                foreach (var itm in values)
                    retVal.AddRange(this.GenerateTests(itm, path));
                return retVal;
            }
            else
                return new PatchOperation[]
                {
                    new PatchOperation(PatchOperationType.Test, path, existingValue)
                };
    }

    public IdentifiedData Patch(Patch patch, IdentifiedData data)
    {
        throw new NotImplementedException();
    }
}
}
