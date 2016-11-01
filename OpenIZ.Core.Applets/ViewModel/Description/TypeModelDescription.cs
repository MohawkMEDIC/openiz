using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{

    /// <summary>
    /// Represents model descriptions
    /// </summary>
    [XmlType(nameof(TypeModelDescription), Namespace = "http://openiz.org/model/view")]
    public class TypeModelDescription : PropertyContainerDescription
    {

        /// <summary>
        /// Initialize the type mode description
        /// </summary>
        public void Initialize()
        {
            foreach (var itm in this.Properties)
                itm.Initialize(this);
        }

        /// <summary>
        /// Name of the type to be loaded
        /// </summary>
        [XmlAttribute("type")]
        public string TypeName { get; set; }

        /// <summary>
        /// Get the name of the container
        /// </summary>
        internal override string GetName()
        {
            return this.TypeName;
        }

    }
}