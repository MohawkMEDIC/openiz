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
        /// <typeparam name="TModel">The type of model to be parsed</typeparam>
        /// <param name="r">The reader type</param>
        /// <param name="context">The current parse context</param>
        /// <returns></returns>
        TModel Deserialize<TModel>(JsonReader r, JsonSerializationContext context);
        /// <summary>
        /// Gets the simple value
        /// </summary>
        object GetSimpleValue(object instance);
    }
}