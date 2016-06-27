using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Class redirect on mapper
    /// </summary>
    [XmlType(nameof(ClassRedirect), Namespace = "http://openiz.org/model/map")]
    public class ClassRedirect
    {
        // Domain type
        private Type m_fromType = null;
        /// <summary>
        /// Gets the domain CLR type
        /// </summary>
        [XmlIgnore]
        public Type FromType
        {
            get
            {
                if (this.m_fromType == null)
                    this.m_fromType = Type.GetType(this.FromClass);
                return this.m_fromType;
            }
        }

        /// <summary>
        /// Gets or sets the model class for the mapper
        /// </summary>
        [XmlAttribute("fromClass")]
        public String FromClass { get; set; }

        /// <summary>
        /// Gets or sets the property maps
        /// </summary>
        [XmlElement("via")]
        public List<PropertyMap> Property { get; set; }

        // TODO: Validation
    }
}