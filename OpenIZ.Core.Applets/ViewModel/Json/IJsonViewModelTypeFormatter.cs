using Newtonsoft.Json;
using OpenIZ.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents a view model type formatter that is for writing/reading to JSON streams
    /// </summary>
    public interface IJsonViewModelTypeFormatter : IViewModelTypeFormatter
    {


        /// <summary>
        /// Serialize specified object <paramref name="o"/> onto writer <paramref name="w"/> in context <paramref name="context"/>
        /// </summary>
        /// <param name="w">The writer to write data to</param>
        /// <param name="o">The object to be graphed</param>
        /// <param name="context">The current serialization context</param>
        void Serialize(JsonWriter w, IdentifiedData o, JsonSerializationContext context);

        /// <summary>
        /// Parses the specified object from the reader
        /// </summary>
        /// <param name="r">The reader type</param>
        /// <param name="context">The current parse context</param>
        /// <param name="asType">The type which is being constructed</param>
        /// <returns></returns>
        object Deserialize(JsonReader r, Type asType, JsonSerializationContext context);

        /// <summary>
        /// Convert from simple value
        /// </summary>
        object FromSimpleValue(object simpleValue);

        /// <summary>
        /// Gets the simple value
        /// </summary>
        object GetSimpleValue(object instance);
    }
}