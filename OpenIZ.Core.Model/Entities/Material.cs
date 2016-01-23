using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a material 
    /// </summary>
    
    [XmlType("Material", Namespace = "http://openiz.org/model")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Material")]
    public class Material : Entity
    {
        // Form concept key
        private Guid? m_formConceptKey;
        // Quantity concept key
        private Guid? m_quantityConceptKey;
        // Form concept
        private Concept m_formConcept;
        // Quantity concept
        private Concept m_quantityConcept;

        /// <summary>
        /// Material ctor
        /// </summary>
        public Material()
        {
            this.ClassConceptKey = EntityClassKeys.Material;
        }

        /// <summary>
        /// The base quantity of the object in the units. This differs from quantity on the relationship
        /// which is a /per ... 
        /// </summary>
        [XmlElement("quantity")]
        public Decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the form concept's key
        /// </summary>
        [XmlElement("formConcept")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? FormConceptKey
        {
            get { return this.m_formConceptKey; }
            set
            {
                this.m_formConceptKey = value;
                this.m_formConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the quantity concept ref
        /// </summary>
        [XmlElement("quantityConcept")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? QuantityConceptKey
        {
            get { return this.m_quantityConceptKey; }
            set
            {
                this.m_quantityConceptKey = value;
                this.m_quantityConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the concept which dictates the form of the material (solid, liquid, capsule, injection, etc.)
        /// </summary>
        [XmlIgnore]
        [DelayLoad(nameof(FormConceptKey))]
        public Concept FormConcept
        {
            get {
                this.m_formConcept = base.DelayLoad(this.m_formConceptKey, this.m_formConcept);
                return this.m_formConcept;
            }
            set
            {
                this.m_formConcept = value;
                this.m_formConceptKey = value?.Key;
            }

        }

        /// <summary>
        /// Gets or sets the concept which dictates the unit of measure for a single instance of this entity
        /// </summary>
        [XmlIgnore]
        [DelayLoad(nameof(QuantityConceptKey))]
        public Concept QuantityConcept
        {
            get
            {
                this.m_quantityConcept = base.DelayLoad(this.m_quantityConceptKey, this.m_quantityConcept);
                return this.m_quantityConcept;
            }
            set
            {
                this.m_quantityConcept = value;
                this.m_quantityConceptKey = value?.Key;
            }

        }

        /// <summary>
        /// Gets or sets the expiry date of the material
        /// </summary>
        [XmlElement("expiryDate")]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// True if the material is simply administrative
        /// </summary>
        [XmlElement("isAdministrative")]
        public Boolean IsAdministrative { get; set; }

        /// <summary>
        /// Forces refresh of the delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_formConcept = null;
            this.m_quantityConcept = null;
        }
    }
}
