using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.ComponentModel;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Concept relationship type
    /// </summary>
    public class ConceptRelationshipType : IdentifiedData
    {

        /// <summary>
        /// Gets or sets the name of the relationship
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The invariant of the relationship type
        /// </summary>
        public String Mnemonic { get; set; }

    }
}