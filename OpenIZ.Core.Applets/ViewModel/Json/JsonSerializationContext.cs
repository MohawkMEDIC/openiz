using OpenIZ.Core.Applets.ViewModel.Description;
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents a JSON serialization context
    /// </summary>
    public class JsonSerializationContext : SerializationContext
    {
        /// <summary>
        /// Creates a new JSON Serialization context
        /// </summary>
        public JsonSerializationContext(String propertyName, JsonViewModelSerializer context, Object instance) : base(propertyName, context, instance)
        {
        }

        /// <summary>
        /// Creates a new JSON serialization context
        /// </summary>
        public JsonSerializationContext(String propertyName, JsonViewModelSerializer context, Object instance, JsonSerializationContext parent) : base(propertyName, context, instance, parent)
        {

        }

        /// <summary>
        /// Gets the Context as a JSON serializer
        /// </summary>
        public JsonViewModelSerializer JsonContext { get { return base.Context as JsonViewModelSerializer; } }

        /// <summary>
        /// Gets the JSON parent context
        /// </summary>
        public JsonSerializationContext JsonParent {  get { return base.Parent as JsonSerializationContext; } }

       
    }
}
