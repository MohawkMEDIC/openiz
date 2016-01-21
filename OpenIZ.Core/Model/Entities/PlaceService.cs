using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a service for a place
    /// </summary>
    [Serializable]
    [DataContract(Name = "Place", Namespace = "http://openiz.org/model")]
    public class PlaceService : VersionBoundRelationData<Entity>
    {

        // Service key
        private Guid m_serviceConceptKey;
        // Service
        [NonSerialized]
        private Concept m_service;

        /// <summary>
        /// The schedule that the service is offered
        /// </summary>
        [DataMember(Name = "serviceSchedule")]
        public XmlElement ServiceSchedule { get; set; }

        /// <summary>
        /// Gets or sets the service concept key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "serviceConceptRef")]
        public Guid ServiceConceptKey
        {
            get { return this.m_serviceConceptKey; }
            set
            {
                this.m_serviceConceptKey = value;
                this.m_service = null;
            }
        }

        /// <summary>
        /// Gets or sets the service concept
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public Concept ServiceConcept
        {
            get {
                this.m_service = base.DelayLoad(this.m_serviceConceptKey, this.m_service);
                return this.m_service;
            }
            set
            {
                this.m_service = value;
                if (value == null)
                    this.m_serviceConceptKey = Guid.Empty;
                else
                    this.m_serviceConceptKey = value.Key;
            }
        }

    }
}