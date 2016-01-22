using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Roles
{
    /// <summary>
    /// Represents an entity which is a patient
    /// </summary>
    [Serializable]
    [DataContract(Name = "Patient", Namespace = "http://openiz.org/model")]
    [Resource(ModelScope.Clinical)]
    public class Patient : Person
    {

        // Gender concept key
        private Guid m_genderConceptKey;
        // Gender concept
        
        private Concept m_genderConcept;

        /// <summary>
        /// Represents a patient
        /// </summary>
        public Patient()
        {
            base.DeterminerConceptKey = DeterminerKeys.Specific;
            base.ClassConceptKey = EntityClassKeys.Patient;
        }

        /// <summary>
        /// Gets or sets the date the patient was deceased
        /// </summary>
        [DataMember(Name = "deceasedDate")]
        public DateTime? DeceasedDate { get; set; }
        /// <summary>
        /// Gets or sets the precision of the date of deceased
        /// </summary>
        [DataMember(Name = "deceasedDatePrecision")]
        public DatePrecision? DeceasedDatePrecision { get; set; }
        /// <summary>
        /// Gets or sets the multiple birth order of the patient 
        /// </summary>
        [DataMember(Name = "multipleBirthOrder")]
        public int? MultipleBirthOrder { get; set; }

        /// <summary>
        /// Gets or sets the gender concept key
        /// </summary>
        [DataMember(Name = "genderConcept")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid GenderCocneptKey
        {
            get { return this.m_genderConceptKey; }
            set
            {
                this.m_genderConceptKey = value;
                this.m_genderConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the gender concept
        /// </summary>
        [DelayLoad(nameof(GenderCocneptKey))]
        [IgnoreDataMember]
        public Concept GenderConcept
        {
            get
            {
                this.m_genderConcept = base.DelayLoad(this.m_genderConceptKey, this.m_genderConcept);
                return this.m_genderConcept;
            }
            set
            {
                this.m_genderConcept = value;
                if (value == null)
                    this.m_genderConceptKey = Guid.Empty;
                else
                    this.m_genderConceptKey = value.Key;
            }
        }
        
        /// <summary>
        /// Force a refresh of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_genderConcept = null;
            
        }
    }
}
