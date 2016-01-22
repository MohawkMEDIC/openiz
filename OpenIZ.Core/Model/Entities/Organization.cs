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
    /// Organization entity
    /// </summary>
    [Serializable]
    [DataContract(Name = "Organization", Namespace = "http://openiz.org/model")]
    [Resource(ModelScope.Clinical)]
    public class Organization : Entity
    {

        // Industry concept
        private Guid m_industryConceptKey;
        // Industry Concept
        
        private Concept m_industryConcept;

        /// <summary>
        /// Organization ctor
        /// </summary>
        public Organization()
        {
            this.DeterminerConceptKey = DeterminerKeys.Specific;
            this.ClassConceptKey = EntityClassKeys.Organization;
        }

        /// <summary>
        /// Gets or sets the industry concept key
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name = "industryConcept")]
        public Guid IndustryConceptKey
        {
            get { return this.m_industryConceptKey; }
            set
            {
                this.m_industryConceptKey = value;
                this.m_industryConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the industry in which the organization operates
        /// </summary>
        [DelayLoad(nameof(IndustryConceptKey))]
        [IgnoreDataMember]
        public Concept IndustryConcept
        {
            get {
                this.m_industryConcept = base.DelayLoad(this.m_industryConceptKey, this.m_industryConcept);
                return this.m_industryConcept;
            }
            set {
                this.m_industryConcept = value;
                if (value == null)
                    this.m_industryConceptKey = Guid.Empty;
                else
                    this.m_industryConceptKey = value.Key;
            }
        }


        /// <summary>
        /// Forces reload of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_industryConcept = null;
        }
    }
}
