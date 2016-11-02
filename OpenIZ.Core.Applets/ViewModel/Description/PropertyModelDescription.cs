using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// Property model description
    /// </summary>
    [XmlType(nameof(PropertyModelDescription) ,Namespace = "http://openiz.org/model/view")]
    public class PropertyModelDescription : PropertyContainerDescription
    {


        /// <summary>
        /// Initialize the parent structure
        /// </summary>
        public void Initialize(PropertyContainerDescription parent)
        {
            this.Parent = parent;
            foreach (var itm in this.Properties)
                itm.Initialize(this);
        }
        
        /// <summary>
        /// The property of the model
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or ssets the where classifiers
        /// </summary>
        [XmlAttribute("classifier")]
        public String Classifier { get; set; }

        /// <summary>
        /// Seriallization behavior
        /// </summary>
        [XmlAttribute("behavior")]
        public SerializationBehaviorType Action { get; set; }

        /// <summary>
        /// Get name 
        /// </summary>
        internal override string GetName()
        {
            return this.Name;
        }
    }
}