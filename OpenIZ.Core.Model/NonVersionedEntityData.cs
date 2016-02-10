using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Updateable entity data which is not versioned
    /// </summary>
    [XmlType(nameof(NonVersionedEntityData), Namespace = "http://openiz.org/model")]
    [JsonObject(Id = nameof(NonVersionedEntityData))]
    public class NonVersionedEntityData : BaseEntityData
    {

        // The updated by id
        private Guid? m_updatedById;
        // The updated by user
        private SecurityUser m_updatedBy;

        /// <summary>
        /// Updated time
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTimeOffset? UpdatedTime { get; set; }


        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [XmlElement("updatedTime"), JsonProperty("updatedTime")]
        public String UpdatedTimeXml
        {
            get { return this.UpdatedTime?.ToString("o", CultureInfo.InvariantCulture); }
            set
            {
                if (value != null)
                    this.UpdatedTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else
                    this.UpdatedTime = null;
            }
        }

        /// <summary>
        /// Gets or sets the user that updated this base data
        /// </summary>
        [DelayLoad(null)]
        [XmlIgnore, JsonIgnore]
        public SecurityUser UpdatedBy
        {
            get
            {
                this.m_updatedBy = base.DelayLoad(this.m_updatedById, this.m_updatedBy);
                return m_updatedBy;
            }
        }

        /// <summary>
        /// Gets or sets the created by identifier
        /// </summary>

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("updatedBy"), JsonProperty("updatedBy")]
        public Guid? UpdatedByKey
        {
            get { return this.m_updatedById; }
            set
            {
                if (this.m_updatedById != value)
                    this.m_updatedBy = null;
                this.m_updatedById = value;
            }
        }

        /// <summary>
        /// Forces refresh
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_updatedBy = null;
        }
    }
}
