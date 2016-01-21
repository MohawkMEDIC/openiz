using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a material 
    /// </summary>
    [Serializable]
    [DataContract(Name = "Material", Namespace = "http://openiz.org/model")]
    public class Material : Entity
    {
        // Form concept key
        private Guid? m_formConceptKey;
        // Quantity concept key
        private Guid? m_quantityConceptKey;
        // Form concept
        [NonSerialized]
        private Concept m_formConcept;
        // Quantity concept
        [NonSerialized]
        private Concept m_quantityConcept;

        /// <summary>
        /// Material ctor
        /// </summary>
        public Material()
        {
            this.ClassConceptKey = EntityClassKeys.Material;
        }

        /// <summary>
        /// Gets or sets the form concept's key
        /// </summary>
        [DataMember(Name = "formConceptRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
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
        [DataMember(Name = "quantityConceptRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
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
        [IgnoreDataMember]
        [DelayLoad]
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
        [IgnoreDataMember]
        [DelayLoad]
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
        [DataMember(Name = "expiryDate")]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// True if the material is simply administrative
        /// </summary>
        [DataMember(Name = "isAdministrative")]
        public Boolean IsAdministrative { get; set; }
    }
}
