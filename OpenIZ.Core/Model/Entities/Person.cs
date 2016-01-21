using MARC.Everest.DataTypes;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
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
    /// Represents an entity which is a person
    /// </summary>
    [Serializable]
    [DataContract(Name = "Person", Namespace = "http://openiz.org/model")]
    public class Person : Entity
    {

        // Language communication
        [NonSerialized]
        private List<PersonLanguageCommunication> m_languageCommunication;

        /// <summary>
        /// Person constructor
        /// </summary>
        public Person()
        {
            base.DeterminerConceptKey = DeterminerKeys.Specific;
            base.ClassConceptKey = EntityClassKeys.Person;
        }

        /// <summary>
        /// Gets or sets the person's date of birth
        /// </summary>
        [DataMember(Name = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the precision ofthe date of birth
        /// </summary>
        [DataMember(Name = "datePrecision")]
        public DatePrecision DatePrecision { get; set; }

        /// <summary>
        /// Gets the person's languages of communication
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public List<PersonLanguageCommunication> LanguageCommunication
        {
            get
            {
                if(this.m_languageCommunication == null &&
                    this.IsDelayLoadEnabled)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<PersonLanguageCommunication>>();
                    this.m_languageCommunication = persistenceService.Query(c=>c.SourceEntityKey == this.Key && this.VersionSequence >= c.EffectiveVersionSequenceId && (this.VersionSequence < c.ObsoleteVersionSequenceId || c.ObsoleteVersionSequenceId == null), null).ToList();
                }
                return this.m_languageCommunication;
            }
        }

    }
}
