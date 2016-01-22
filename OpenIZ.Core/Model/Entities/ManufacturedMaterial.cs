using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Manufactured material
    /// </summary>
    [Serializable]
    [DataContract(Name = "ManufacturedMaterial", Namespace = "http://openiz.org/model")]
    [Resource(ModelScope.Clinical)]
    public class ManufacturedMaterial : Material
    {

        /// <summary>
        /// Creates a new manufactured material
        /// </summary>
        public ManufacturedMaterial()
        {
            base.DeterminerConceptKey = DeterminerKeys.Specific;
            base.ClassConceptKey = EntityClassKeys.ManufacturedMaterial;
        }

        /// <summary>
        /// Gets or sets the lot number of the manufactured material
        /// </summary>
        [DataMember(Name = "lotNumber")]
        public String LotNumber { get; set; }

    }
}
