using System;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a phonetic algorithm record in the model
    /// </summary>
    public class PhoneticAlgorithm : IdentifiedData
    {

        /// <summary>
        /// Gets the name of the phonetic algorithm
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Gets the handler (or generator) for the phonetic algorithm
        /// </summary>
        public Type Handler { get; set; }

    }
}