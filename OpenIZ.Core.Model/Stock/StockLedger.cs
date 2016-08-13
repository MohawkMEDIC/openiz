using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Stock
{
    /// <summary>
    /// Represents a single entry in a place's stock ledger
    /// </summary>
    [XmlType(nameof(StockLedger), Namespace = "http://openiz.org/model"), JsonObject(nameof(StockLedger))]
    public class StockLedger : Association<Place>
    {

        private Guid? m_stockActionKey;
        private Guid? m_materialKey;
        private Guid? m_createdByKey;
        private Concept m_stockAction;
        private ManufacturedMaterial m_material;
        private SecurityUser m_createdBy;

        /// <summary>
        /// Gets or sets the quantity of stock in the ledger object
        /// </summary>
        [XmlElement("quantity"), JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the stock action performed
        /// </summary>
        [XmlElement("entryType"), JsonProperty("entryType")]
        public Guid? StockActionKey
        {
            get { return this.m_stockActionKey; }
            set
            {
                if (this.m_stockActionKey != value)
                    this.m_stockAction = null;
                this.m_stockActionKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the stock action
        /// </summary>
        [XmlIgnore, JsonIgnore, AutoLoad]
        public Concept StockAction
        {
            get
            {
                this.m_stockAction = this.DelayLoad(this.m_stockActionKey, this.m_stockAction);
                return this.m_stockAction;
            }
            set
            {
                this.m_stockAction = value;
                this.m_stockActionKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the material key
        /// </summary>
        [XmlElement("material"), JsonProperty("material")]
        public Guid? MaterialKey
        {
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
        public ManufacturedMaterial Material
        {
            get
            {
                this.m_material = this.DelayLoad(this.m_materialKey, this.m_material);
                return this.m_material;
            }
            set
            {
                this.m_material = value;
                this.m_materialKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the creation time
        /// </summary>
        [XmlElement("creationTime"), JsonProperty("creationTime")]
        public DateTimeOffset? CreationTime { get; set; }

        /// <summary>
        /// Gets or set the key of the user that created this 
        /// </summary>
        [XmlElement("createdBy"), JsonProperty("createdBy")]
        public Guid? CreatedByKey {
            get { return this.m_createdByKey; }
            set
            {
                if (this.m_createdByKey != value)
                    this.m_createdBy = null;
                this.m_createdByKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the user that created this entry
        /// </summary>
        [XmlIgnore, JsonIgnore, AutoLoad, SerializationReference(nameof(CreatedByKey))]
        public SecurityUser CreatedBy
        {
            get
            {
                this.m_createdBy = this.DelayLoad(this.m_createdByKey, this.m_createdBy);
                return this.m_createdBy;
            }
            set
            {
                this.m_createdBy = value;
                this.m_createdByKey = value?.Key;
            }
        }

    }
}
