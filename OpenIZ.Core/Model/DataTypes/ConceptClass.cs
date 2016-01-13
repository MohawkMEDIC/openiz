using System;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Identifies a classification for a concept
    /// </summary>
    public class ConceptClass : IdentifiedData
    {

        /// <summary>
        /// Gets or sets the name of the concept class
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the mnemonic
        /// </summary>
        public string Mnemonic { get; set; }


    }
}