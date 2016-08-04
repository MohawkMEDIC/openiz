using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Cast map
    /// </summary>
    [XmlType(nameof(CastMap), Namespace = "http://openiz.org/model/map")]
    public class CastMap : PropertyMap
    {

        // Model type
        private Type m_modelType;

        /// <summary>
        /// Type name
        /// </summary>
        [XmlAttribute("type")]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets the model CLR type
        /// </summary>
        [XmlIgnore]
        public Type ModelType
        {
            get
            {
                if (this.m_modelType == null)
                    this.m_modelType = Type.GetType(this.TypeName);
                return this.m_modelType;
            }
        }
    }
}