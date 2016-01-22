using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a base entity extension
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://openiz.org/model")]
    public abstract class Extension<TBoundModel> : VersionBoundRelationData<TBoundModel> where TBoundModel : VersionedEntityData<TBoundModel>
    {

        // Extension type key
        private Guid m_extensionTypeKey;
        // Extension type
        
        private ExtensionType m_extensionType;

        /// <summary>
        /// Gets or sets the value of the extension
        /// </summary>
        [XmlElement("value")]
        public byte[] ExtensionValue { get; set; }

        /// <summary>
        /// Gets or sets the extension type key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [XmlIgnore]
        public Guid ExtensionTypeKey
        {
            get { return this.m_extensionTypeKey; }
            set
            {
                this.m_extensionTypeKey = value;
                this.m_extensionType = null;
            }
        }

        /// <summary>
        /// Gets or sets the extension type
        /// </summary>
        [DelayLoad(nameof(ExtensionTypeKey))]
        [XmlElement("extensionType")]
        public ExtensionType ExtensionType
        {
            get {
                this.m_extensionType = base.DelayLoad(this.m_extensionTypeKey, this.m_extensionType);
                return this.m_extensionType;
            }
            set
            {
                this.m_extensionType = value;
                if (value == null)
                    this.m_extensionTypeKey = Guid.Empty;
                else
                    this.m_extensionTypeKey = value.Key;
            }
        }

        /// <summary>
        /// Forces refresh 
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_extensionType = null;
        }
    }

    /// <summary>
    /// Extension bound to entity
    /// </summary>
    [Serializable]
    [XmlType("EntityExtension", Namespace = "http://openiz.org/model")]
    public class EntityExtension : Extension<Entity>
    {

    }

    /// <summary>
    /// Act extension
    /// </summary>
    [Serializable]
    [XmlType("ActExtension", Namespace = "http://openiz.org/model")]
    public class ActExtension : Extension<Act>
    {

    }
}
