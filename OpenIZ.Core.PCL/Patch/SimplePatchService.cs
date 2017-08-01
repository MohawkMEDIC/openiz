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
 * Date: 2016-11-30
 */
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
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Map;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a simple patch service which can calculate patches and apply them
    /// </summary>
    public class SimplePatchService : IPatchService
    {
        // Ignore properties
        private readonly string[] ignoreProperties = {
            "previousVersion",
            "version",
            "id"
        };

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SimplePatchService));

        // Property information
        private Dictionary<Type, IEnumerable<PropertyInfo>> m_properties = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        // Lock object
        private object m_lockObject = new object();

        /// <summary>
        /// Perform a diff using a simple .compare() method
        /// </summary>
        /// <remarks>This method only performs a diff on the root object passed and does not cascade to collections</remarks>
        public Patch Diff(IdentifiedData existing, IdentifiedData updated, params string[] ignoreProperties)
        {
            var retVal = new Patch()
            {
                Key = Guid.NewGuid(),
                CreationTime = DateTimeOffset.Now,
                Operation = this.DiffInternal(existing, updated, null, ignoreProperties),
                AppliesTo = new PatchTarget(existing)
            };
            this.m_tracer.TraceVerbose("-->> DIFF {0} > {1}\r\n{2}", existing, updated, retVal);
            return retVal;
        }

        /// <summary>
        /// Difference internal
        /// </summary>
        private List<PatchOperation> DiffInternal(IdentifiedData existing, IdentifiedData updated, String path, String[] ignoreProperties)
        {
            // First are they the same?
            this.m_tracer.TraceVerbose("Generating DIFF: {0} > {1}", existing, updated);

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
                    if (ignoreProperties?.Contains($"{path}{serializationName}") == true) continue;

                    object existingValue = existing.LoadProperty(pi.Name),
                        updatedValue = pi.GetValue(updated);

                    // Skip ignore properties
                    if (ignoreProperties.Contains(serializationName)) continue;

                    // Test
                    if (existingValue == updatedValue)
                        continue; // same 
                    else
                    {
                        if (existingValue != null && updatedValue == null) // remove
                        {
                            // Generate tests
                            if(typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(pi.PropertyType.GetTypeInfo()))
                                retVal.AddRange(this.GenerateTests(existingValue, $"{path}{serializationName}"));
                            retVal.Add(new PatchOperation(PatchOperationType.Remove, $"{path}{serializationName}", null));
                        }
                        else if ((existingValue as IdentifiedData)?.SemanticEquals(updatedValue as IdentifiedData) == false) // They are different
                        {
                            // Generate tests
                            IdentifiedData existingId = existingValue as IdentifiedData,
                                updatedId = updatedValue as IdentifiedData;
                            if (existingId.Key == updatedId.Key)
                                retVal.AddRange(this.DiffInternal(existingId, updatedId, $"{path}{serializationName}.", ignoreProperties));
                            else
                            {
                                retVal.AddRange(this.GenerateTests(existingValue, $"{path}{serializationName}"));
                                retVal.Add(new PatchOperation(PatchOperationType.Replace, $"{path}{serializationName}", updatedValue));
                            }
                        }
                        else if (existingValue is IList && !existingValue.GetType().GetTypeInfo().IsArray)
                        {

                            // Simple or complex list?
                            if (typeof(IIdentifiedEntity).GetTypeInfo().IsAssignableFrom(existingValue.GetType().StripGeneric().GetTypeInfo()))
                            {
                                IEnumerable<IdentifiedData> updatedList = (updatedValue as IEnumerable).OfType<IdentifiedData>(),
                                    existingList = (existingValue as IEnumerable).OfType<IdentifiedData>();

                                // Removals
                                retVal.AddRange(existingList.Where(e => !updatedList.Any(u => e.SemanticEquals(u))).Select(c => this.BuildRemoveQuery(path + serializationName, c)));

                                // Additions 
                                retVal.AddRange(updatedList.Where(u => !existingList.Any(e => u.SemanticEquals(e))).Select(c => new PatchOperation(PatchOperationType.Add, $"{path}{serializationName}", c)));

                            }
                            else
                            {
                                IEnumerable<Object> updatedList = (updatedValue as IEnumerable).OfType<Object>(),
                                    existingList = (existingValue as IEnumerable).OfType<Object>();

                                // Removals
                                retVal.AddRange(existingList.Where(e => !updatedList.Any(u => e.Equals(u))).Select(c => new PatchOperation(PatchOperationType.Remove, $"{path}{serializationName}", c)));

                                // Additions 
                                retVal.AddRange(updatedList.Where(u => !existingList.Any(e => u.Equals(e))).Select(c => new PatchOperation(PatchOperationType.Add, $"{path}{serializationName}", c)));

                            }
                        }
                        else if (updatedValue?.Equals(existingValue) == false)// simple value has changed
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
        /// Build removal query
        /// </summary>
        private PatchOperation BuildRemoveQuery(String path, IdentifiedData c)
        {
            PatchOperation retval = new PatchOperation(PatchOperationType.Remove, path, null);
            if (c.Key.HasValue)
            {
                retval.Path += ".id";
                retval.Value = c.Key;
            }
            else
            {
                var classAtt = c.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                Object cvalue = c;
                // Build path
                var serializationName = "";
                while (classAtt != null && c != null)
                {
                    var pi = cvalue.GetType().GetRuntimeProperty(classAtt.ClassifierProperty);
                    var redirectProperty = pi.GetCustomAttribute<SerializationReferenceAttribute>();
                    if (redirectProperty != null)
                        serializationName += "." + cvalue.GetType().GetRuntimeProperty(redirectProperty.RedirectProperty).GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
                    else
                        serializationName += "." + pi.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;

                    cvalue = pi.GetValue(cvalue);
                    classAtt = cvalue?.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                }
                retval.Path += serializationName;
                retval.Value = cvalue;
            }

            return retval;
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

        /// <summary>
        /// Applies the specified <paramref name="patch"/> onto <paramref name="data"/> to derive the return value.
        /// </summary>
        public IdentifiedData Patch(Patch patch, IdentifiedData data, bool force = false)
        {

            this.m_tracer.TraceVerbose("-->> {0} patch with:\r\n{1}", data, patch);

            var retVal = Activator.CreateInstance(data.GetType());
            retVal.CopyObjectData(data);

            // We want to run through the patch operations and execute them
            foreach (var op in patch.Operation)
            {
                // Grab the right property
                var propertyName = op.Path.Split('.');
                String pathName = String.Empty;
                PropertyInfo property = null;
                object applyTo = retVal, applyParent = null;
                foreach (var itm in propertyName)
                {
                    // Get the properties
                    IEnumerable<PropertyInfo> properties = null;
                    if (!this.m_properties.TryGetValue(applyTo.GetType(), out properties))
                        lock (this.m_lockObject)
                            if (!this.m_properties.ContainsKey(applyTo.GetType()))
                            {
                                properties = applyTo.GetType().GetRuntimeProperties().Where(o => o.CanRead && o.CanWrite && o.GetCustomAttribute<JsonPropertyAttribute>() != null);
                                this.m_properties.Add(applyTo.GetType(), properties);
                            }

                    var subProperty = properties.FirstOrDefault(o => o.GetCustomAttribute<JsonPropertyAttribute>().PropertyName == itm);
                    if (subProperty != null)
                    {
                        applyParent = applyTo;
                        applyTo = subProperty.GetValue(applyTo);
                        if (applyTo is IList)
                        {
                            applyTo = Activator.CreateInstance(subProperty.PropertyType, applyTo);
                            subProperty.SetValue(applyParent, applyTo);
                        }
                        property = subProperty;
                        pathName += itm + ".";
                    }
                    else
                        break;
                }

                // Operation type
                switch (op.OperationType)
                {
                    case PatchOperationType.Add:
                        // We add the value!!! Yay!
                        if (applyTo is IList)
                            (applyTo as IList).Add(op.Value);
                        else
                            throw new PatchException("Add can only be applied to an IList instance");
                        break;
                    case PatchOperationType.Remove:
                        // We add the value!!! Yay!
                        if (applyTo is IList)
                        {
                            var instance = this.ExecuteLambda("FirstOrDefault", applyTo, property, pathName, op);
                            if (instance != null)
                                (applyTo as IList).Remove(instance);
                            else
                            {
                                // HACK: Patches with no version code don't adhere to ths
                                if (String.IsNullOrEmpty(patch.Version) && force)
                                    this.m_tracer.TraceWarning("Patch specifies removal of non-existing relationship {0} -> Ignoring", op);
                                else
                                    throw new PatchAssertionException("Cannot remove a non-existing relationship");
                            }
                        }
                        else if (op.Value == null)
                            property.SetValue(applyParent, null);
                        else
                            throw new PatchException("Remove can only be applied to an IList instance or to remove a value");

                        break;
                    case PatchOperationType.Replace:
                        {
                            Object val = null;
                            if (MapUtil.TryConvert(op.Value, property.PropertyType.StripNullable(), out val))
                                property.SetValue(applyParent, val);
                            else
                                property.SetValue(applyParent, op.Value);
                            break;
                        }
                    case PatchOperationType.Test:
                        if (force) continue;
                        // We test the value! Also pretty cool
                        if (applyTo is IdentifiedData && !(applyTo as IdentifiedData).SemanticEquals(op.Value as IdentifiedData))
                            throw new PatchAssertionException(op.Value, applyTo, op);
                        else if (applyTo is IList)
                        {
                            // Identified data
                            if (typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(property.PropertyType.StripGeneric().GetTypeInfo()))
                            {
                                var result = this.ExecuteLambda("Any", applyTo, property, pathName, op);
                                if (!(bool)result)
                                    throw new PatchAssertionException($"Could not find instance matching {op.Path.Replace(pathName, "")} = {op.Value} in collection {applyTo} at {op}");
                            }
                            else if (!(applyTo as IList).OfType<Object>().Any(o => o.Equals(op.Value)))
                                throw new PatchAssertionException($"Assertion failed: {op.Value} could not be found in list {applyTo} at {op}");

                        }
                        else if (applyTo?.Equals(op.Value) == false && applyTo != op.Value)
                            throw new PatchAssertionException(op.Value, applyTo, op);
                        break;
                }
            }
            return retVal as IdentifiedData;
        }

        /// <summary>
        /// Execute a lambda
        /// </summary>
        private object ExecuteLambda(string action, object source, PropertyInfo property, string pathName, PatchOperation op)
        {
            var mi = typeof(QueryExpressionParser).GetGenericMethod("BuildLinqExpression", new Type[] { property.PropertyType.StripGeneric() }, new Type[] { typeof(NameValueCollection) });
            var lambda = mi.Invoke(null, new object[] { NameValueCollection.ParseQueryString($"{op.Path.Replace(pathName, "")} = {op.Value}") });
            lambda = lambda.GetType().GetRuntimeMethod("Compile", new Type[] { }).Invoke(lambda, new object[] { });
            var filterMethod = typeof(Enumerable).GetGenericMethod(action, new Type[] { property.PropertyType.StripGeneric() }, new Type[] { typeof(IEnumerable<>).MakeGenericType(property.PropertyType.StripGeneric()), lambda.GetType() });
            if (filterMethod == null)
                throw new PatchException($"Cannot locate instance of {action}() method on collection type {source.GetType()} at {op}");
            return filterMethod.Invoke(null, new object[] { source, lambda });

        }

        /// <summary>
        /// Test that a patch can be applied
        /// </summary>
        public bool Test(Patch patch, IdentifiedData data)
        {

            this.m_tracer.TraceVerbose("-->> {0} test with:\r\n{1}", data, patch);
            bool retVal = true;

            // We want to run through the patch operations and execute them
            foreach (var op in patch.Operation)
            {
                // Grab the right property
                var propertyName = op.Path.Split('.');
                String pathName = String.Empty;
                PropertyInfo property = null;
                object applyTo = data, applyParent = null;

                foreach (var itm in propertyName)
                {
                    // Get the properties
                    IEnumerable<PropertyInfo> properties = null;
                    if (!this.m_properties.TryGetValue(applyTo.GetType(), out properties))
                        lock (this.m_lockObject)
                            if (!this.m_properties.ContainsKey(applyTo.GetType()))
                            {
                                properties = applyTo.GetType().GetRuntimeProperties().Where(o => o.CanRead && o.CanWrite && o.GetCustomAttribute<JsonPropertyAttribute>() != null);
                                this.m_properties.Add(applyTo.GetType(), properties);
                            }

                    var subProperty = properties.FirstOrDefault(o => o.GetCustomAttribute<JsonPropertyAttribute>().PropertyName == itm);
                    if (subProperty != null)
                    {
                        applyParent = applyTo;
                        applyTo = subProperty.GetValue(applyTo);
                        if (applyTo is IList)
                            applyTo = Activator.CreateInstance(subProperty.PropertyType, applyTo);
                        property = subProperty;
                        pathName += itm + ".";
                    }
                    else
                        break;
                }

                if (this.ignoreProperties.Contains(op.Path))
                    continue;

                // Operation type
                switch (op.OperationType)
                {
                    case PatchOperationType.Remove:
                        // We add the value!!! Yay!
                        if (applyTo is IList)
                        {
                            var instance = this.ExecuteLambda("FirstOrDefault", applyTo, property, pathName, op);
                            retVal &= instance != null;
                        }
                        break;
                    case PatchOperationType.Test:
                        // We test the value! Also pretty cool
                        if (applyTo is IdentifiedData && !(applyTo as IdentifiedData).SemanticEquals(op.Value as IdentifiedData))
                            retVal = false;
                        else if (applyTo is IList)
                        {
                            // Identified data
                            if (typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(property.PropertyType.StripGeneric().GetTypeInfo()))
                            {
                                var result = this.ExecuteLambda("Any", applyTo, property, pathName, op);
                                if (!(bool)result)
                                    retVal = false;
                            }
                            else if (!(applyTo as IList).OfType<Object>().Any(o => o.Equals(op.Value)))
                                retVal = false;

                        }
                        else if (applyTo?.Equals(op.Value) == false && applyTo != op.Value)
                            retVal = false;
                        break;
                }
            }
            return retVal;
        }
    }
}
