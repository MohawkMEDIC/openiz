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
using OpenIZ.Core.Applets.ViewModel.Description;
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Applets.ViewModel
{
    /// <summary>
    /// Represents a serialization context
    /// </summary>
    public abstract class SerializationContext
    {
        // Object identifier
        private int m_objectId = 0;
        private int m_masterObjectId = 0;

        // Element description
        private PropertyContainerDescription m_elementDescription;

        /// <summary>
        /// Gets the serialization context
        /// </summary>
        public SerializationContext(String propertyName, IViewModelSerializer context, Object instance)
        {
            this.PropertyName = propertyName;
            this.Parent = null;
            this.Instance = instance;
            this.Context = context;
            this.ViewModelDescription = this.Context.ViewModel;
            this.LoadedProperties = new Dictionary<Guid, HashSet<string>>();
        }

        /// <summary>
        /// Gets the serialization context
        /// </summary>
        public SerializationContext(String propertyName, IViewModelSerializer context, Object instance, SerializationContext parent) : this(propertyName, context, instance)
        {
            this.Parent = parent;
            this.m_objectId = this.Root.m_masterObjectId++;
            this.LoadedProperties = parent?.LoadedProperties ?? new Dictionary<Guid, HashSet<string>>();
        }

        /// <summary>
        /// Get the debug view
        /// </summary>
        public string DebugView {
            get
            {
                var c = this;
                String retVal = String.Empty;
                while(c != null)
                {
                    retVal = "." + c.ToString() + retVal;
                    c = c.Parent;
                }
                return retVal;
            }
        }

        /// <summary>
        /// Gets the name of the element
        /// </summary>
        public String PropertyName { get; private set; }

        /// <summary>
        /// Gets the view model serializer in context
        /// </summary>
        public IViewModelSerializer Context { get; private set; }

        /// <summary>
        /// Gets or sets the view model description of the current element
        /// </summary>
        public PropertyContainerDescription ElementDescription {
            get
            {
                if (this.m_elementDescription == null)
                {
                    var elementDescription = this.ViewModelDescription?.FindDescription(this.PropertyName, this.Parent?.ElementDescription);
                    if (elementDescription == null)
                        elementDescription = this.ViewModelDescription?.FindDescription(this.Instance?.GetType().StripGeneric());
                    if (!String.IsNullOrEmpty(elementDescription?.Ref))
                        elementDescription = this.ViewModelDescription?.FindDescription(elementDescription.Ref) ?? elementDescription;
                    this.m_elementDescription = elementDescription;
                }
                return this.m_elementDescription;
            }
        }
        
        /// <summary>
        /// Gets or sets the root view model description
        /// </summary>
        public ViewModelDescription ViewModelDescription { get; private set; }

        /// <summary>
        /// Gets the parent of the current serialization context
        /// </summary>
        public SerializationContext Parent { get; private set; }

        /// <summary>
        /// Gets or sets the instance value
        /// </summary>
        public Object Instance { get; private set; }

        /// <summary>
        /// Gets the root context
        /// </summary>
        public SerializationContext Root {
            get
            {
                var idx = this;
                while (idx.Parent != null)
                    idx = idx.Parent;
                return idx;
            }
        }

        /// <summary>
        /// Returns true when child property information should be force loaded
        /// </summary>
        public bool ShouldForceLoad(string childProperty, Guid key)
        {
            var propertyDescription = this.ElementDescription?.FindProperty(childProperty) as PropertyModelDescription;
            if(propertyDescription?.Action != SerializationBehaviorType.Always)
                return false;

            // Known miss targets
            HashSet<String> missProp = null;
            if (this.LoadedProperties.TryGetValue(key, out missProp))
            {
                if (missProp.Contains(childProperty))
                    return false;
            }
            else
                this.LoadedProperties.Add(key, new HashSet<string>() { });
            return true;
        }

        /// <summary>
        /// Register that this property was missed
        /// </summary>
        public void RegisterMissTarget(String childProperty, Guid key)
        {
            HashSet<String> missProp = null;
            if (this.LoadedProperties.TryGetValue(key, out missProp))
            {
                if (!missProp.Contains(childProperty))
                    missProp.Add(childProperty);
            }
            else
                this.LoadedProperties.Add(key, new HashSet<string>() { childProperty });

        }
        /// <summary>
        /// Gets the current object identifier (from a JSON property perspective
        /// </summary>
        public int ObjectId {  get { return this.m_objectId; } }


        /// <summary>
        /// Loaded properties
        /// </summary>
        public Dictionary<Guid, HashSet<String>> LoadedProperties { get; private set; }

        /// <summary>
        /// Gets the object id of the specified object from the parent instance if it exists 
        /// </summary>
        public int? GetParentObjectId(IdentifiedData data)
        {

            var idx = this;
            while(idx != null)
            {
                if ((idx.Instance as IdentifiedData)?.Key.HasValue == true &&
                    data.Key.HasValue &&
                    (idx.Instance as IdentifiedData)?.Key.Value == data.Key.Value ||
                    idx.Instance == data)
                    return idx.ObjectId;
                idx = idx.Parent;
            }
            return null;
        }

        /// <summary>
        /// Returns true if the object should be serialized based on the data at hand
        /// </summary>
        public bool ShouldSerialize(String childProperty)
        {
            if (childProperty == "id") return true;
            var propertyDescription = this.ElementDescription?.FindProperty(childProperty) as PropertyModelDescription;
            if (propertyDescription?.Action == SerializationBehaviorType.Never || // Never serialize
                this.ElementDescription == null ||
                (!this.ElementDescription.All && propertyDescription == null))
            {
                // Parent is not set to all and does not explicitly call this property out
                return (this.ElementDescription == null && this.Parent?.ElementDescription?.All == true);
            }
            return true;
        }

        /// <summary>
        /// Property string
        /// </summary>
        public override string ToString()
        {
            return this.PropertyName;
        }
    }
}
