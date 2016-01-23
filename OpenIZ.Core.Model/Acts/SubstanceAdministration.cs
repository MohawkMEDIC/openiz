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

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents an act whereby a substance is administered to the patient
    /// </summary>
    
    [XmlType("SubstanceAdministration", Namespace = "http://openiz.org/model")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "SubstanceAdministration")]
    public class SubstanceAdministration : Act
    {
        // Route key
        private Guid m_routeKey;
        // Dose unit key
        private Guid m_doseUnitKey;
        // Route
        private Concept m_route;
        // Dose unit
        private Concept m_doseUnit;

        /// <summary>
        /// Substance administration ctor
        /// </summary>
        public SubstanceAdministration()
        {
            base.ClassConceptKey = ActClassKeys.SubstanceAdministration;
        }

        /// <summary>
        /// Gets or sets the key for route
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("route")]
        public Guid RouteKey
        {
            get { return this.m_routeKey; }
            set
            {
                this.m_routeKey = value;
                this.m_route = null;
            }
        }

        /// <summary>
        /// Gets or sets the key for dosing unit
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("doseUnit")]
        public Guid DoseUnitKey
        {
            get { return this.m_doseUnitKey; }
            set
            {
                this.m_doseUnitKey = value;
                this.m_doseUnit = null;
            }
        }

        /// <summary>
        /// Gets or sets a concept which indicates the route of administration (eg: Oral, Injection, etc.)
        /// </summary>
        [XmlIgnore]
        [DelayLoad(nameof(RouteKey))]
        public Concept Route
        {
            get
            {
                this.m_route = base.DelayLoad(this.m_routeKey, this.m_route);
                return this.m_route;
            }
            set
            {
                this.m_route = value;
                if (value == null)
                    this.m_routeKey = Guid.Empty;
                else
                    this.m_routeKey = value.Key;
            }
        }

        /// <summary>
        /// Gets or sets a concept which indicates the unit of measure for the dose (eg: 5 mL, 10 mL, 1 drop, etc.)
        /// </summary>
        [XmlIgnore]
        [DelayLoad(nameof(DoseUnitKey))]
        public Concept DoseUnit
        {
            get
            {
                this.m_doseUnit = base.DelayLoad(this.m_doseUnitKey, this.m_doseUnit);
                return this.m_doseUnit;
            }
            set
            {
                this.m_doseUnit = value;
                if (value == null)
                    this.m_doseUnitKey = Guid.Empty;
                else
                    this.m_doseUnitKey = value.Key;
            }
        }

        /// <summary>
        /// Gets or sets the amount of substance administered
        /// </summary>
        [XmlElement("doseQuantity")]
        public Decimal DoseQuantity { get; set; }

        /// <summary>
        /// The sequence of the dose (i.e. OPV 0 = 0 , OPV 1 = 1, etc.)
        /// </summary>
        [XmlElement("doseSequence")]
        public uint SequenceId { get; set; }

        /// <summary>
        /// Force delay loading of properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_doseUnit = this.m_route = null;
        }
    }
}
