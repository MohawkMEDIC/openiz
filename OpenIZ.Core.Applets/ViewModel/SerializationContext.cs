using Newtonsoft.Json;
using OpenIZ.Core.Applets.ViewModel.Description;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel
{
    /// <summary>
    /// Represents a serialization context
    /// </summary>
    internal class SerializationContext
    {

        /// <summary>
        /// Serialization context
        /// </summary>
        public SerializationContext(ViewModelDescription viewModel)
        {
            this.ViewModelDefinition = viewModel;
        }

        /// <summary>
        /// Get the serialization name
        /// </summary>
        protected string GetSerializationName(PropertyInfo propertyInfo)
        {
            var jpa = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
            if (jpa == null && propertyInfo.GetCustomAttribute<SerializationReferenceAttribute>() != null)
                jpa = propertyInfo.DeclaringType.GetRuntimeProperty(propertyInfo.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty)?.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName + "Model";
            if (jpa == null)
                jpa = propertyInfo.Name.ToLower() + "Model";

            if (jpa == null || jpa == "$type")
                return null;
            return jpa;
        }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public Type Type { get; protected set; }

        /// <summary>
        /// Represents the view model definition
        /// </summary>
        public ViewModelDescription ViewModelDefinition { get; protected set; }


        /// <summary>
        /// Description
        /// </summary>
        public PropertyContainerDescription Description { get; set; }
    }

    /// <summary>
    /// Represents root serialization context
    /// </summary>
    internal class RootSerializationContext : SerializationContext
    {

        /// <summary>
        /// Represents the type model description
        /// </summary>
        public RootSerializationContext(Type rootType, ViewModelDescription description) : base(description)
        {
            this.Type = rootType;
            string typeName = rootType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                rootType.Name;
            this.Description = description.Model.FirstOrDefault(o => o.TypeName == typeName);
            // Children from the heirarchy
            while (rootType != typeof(IdentifiedData) && this.Description == null)
            {
                rootType = rootType.GetTypeInfo().BaseType;
                if (rootType == null) break;
                typeName = rootType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                    rootType.Name;
                this.Description = description.Model.FirstOrDefault(o => o.TypeName == typeName);
            }
        }
        

    }

    /// <summary>
    /// Property serialization context
    /// </summary>
    internal class PropertySerializationContext : SerializationContext
    {

        /// <summary>
        /// Serialization context for a property
        /// </summary>
        public PropertySerializationContext(PropertyInfo propertyInfo, SerializationContext parent) : base(parent.ViewModelDefinition)
        {
            this.Parent = parent;
            
            this.Name = this.GetSerializationName(propertyInfo);

            // Find the property information
            this.Description = parent.Description?.Properties.FirstOrDefault(o => o.Name == this.Name);

            // Maybe this can be done via type?
            if(this.Description == null)
            {
                var elementType = propertyInfo.PropertyType;

                if (elementType.GetTypeInfo().IsGenericType)
                    elementType = elementType.GetTypeInfo().GenericTypeArguments[0];

                var rcl = new RootSerializationContext(elementType, parent.ViewModelDefinition);
                this.Description = rcl.Description;
                string typeName = elementType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                    propertyInfo.DeclaringType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                    elementType.Name;

                this.Description = parent.ViewModelDefinition.Model.FirstOrDefault(o => o.TypeName == typeName);

            }

        }

        /// <summary>
        /// Parent
        /// </summary>
        public SerializationContext Parent { get; private set; }

        /// <summary>
        /// Gets the serialization name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Property information
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

    }
}