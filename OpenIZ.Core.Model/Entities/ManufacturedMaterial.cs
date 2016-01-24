using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Manufactured material
    /// </summary>
    
    [XmlType("ManufacturedMaterial",  Namespace = "http://openiz.org/model"), JsonObject("ManufacturedMaterial")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "ManufacturedMaterial")]
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
        [XmlElement("lotNumber"), JsonProperty("lotNumber")]
        public String LotNumber { get; set; }

    }
}
