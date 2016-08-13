using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Stock
{
    /// <summary>
    /// Stock balance
    /// </summary>
    [XmlType(nameof(StockBalance), Namespace="http://openiz.org/model"), JsonObject(nameof(StockBalance))]
    public class StockBalance : Association<Place>
    {
        // Material key
        private Guid? m_materialKey;
        private ManufacturedMaterial m_material;

        /// <summary>
        /// Gets or sets the quantity of object
        /// </summary>
        [XmlElement("quantity"), JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the material key
        /// </summary>
        [XmlElement("material"), JsonProperty("material")]
        public Guid? MaterialKey {
            get { return this.m_materialKey; }
            set
            {
                if (this.m_materialKey != value)
                    this.m_material = null;
                this.m_materialKey = value;
            }
        }

        /// <summary>
        /// Material which is being counted
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(MaterialKey)), AutoLoad]
        public ManufacturedMaterial Material {
            get {
                this.m_material = this.DelayLoad(this.m_materialKey, this.m_material);
                return this.m_material;
            }
            set
            {
                this.m_material = value;
                this.m_materialKey = value?.Key;
            }
        }


    }
}
