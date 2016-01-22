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
    /// Represents a provider role of a person
    /// </summary>
    [Serializable]
    [DataContract(Name = "Provider", Namespace = "http://openiz.org/model")]
    [Resource(ModelScope.Clinical)]
    public class Provider : Person
    {

        // Specialty key
        private Guid? m_providerSpecialtyKey;
        // Specialty value
        
        private Concept m_providerSpeciality;

        /// <summary>
        /// Creates a new provider
        /// </summary>
        public Provider()
        {
            this.DeterminerConceptKey = DeterminerKeys.Specific;
            this.ClassConceptKey = EntityClassKeys.Provider;
        }

        /// <summary>
        /// Gets or sets the provider specialty key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "providerSpecialty")]
        public Guid? ProviderSpecialtyKey
        {
            get
            {
                return this.m_providerSpecialtyKey;
            }
            set
            {
                this.m_providerSpecialtyKey = value;
                this.m_providerSpeciality = null;
            }
        }

        /// <summary>
        /// Gets or sets the provider specialty
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad(nameof(ProviderSpecialtyKey))]
        public Concept ProviderSpecialty
        {
            get
            {
                this.m_providerSpeciality = base.DelayLoad(this.m_providerSpecialtyKey, this.m_providerSpeciality);
                return this.m_providerSpeciality;
            }
            set
            {
                this.m_providerSpeciality = value;
                this.m_providerSpecialtyKey = value?.Key;
            }
        }

        /// <summary>
        /// Force a refresh of the delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_providerSpeciality = null;
        }
    }
}
