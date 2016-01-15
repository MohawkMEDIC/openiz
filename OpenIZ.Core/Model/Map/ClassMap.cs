using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MARC.Everest.Connectors;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Class mapping
    /// </summary>
    [XmlType(nameof(ClassMap), Namespace = "http://openiz.org/model/map")]
    public class ClassMap
    {

        /// <summary>
        /// Gets or sets the model class for the mapper
        /// </summary>
        [XmlAttribute("modelClass")]
        public String ModelClass { get; set; }

        /// <summary>
        /// Gets or sets the domain class for the mapper
        /// </summary>
        [XmlAttribute("domainClass")]
        public String DomainClass { get; set; }

        /// <summary>
        /// Gets or sets the association maps which are used for collapsing keys
        /// </summary>
        [XmlElement("collapseKey")]
        public List<CollapseKey> CollapseKey { get; set; }

        /// <summary>
        /// Gets or sets the property maps
        /// </summary>
        [XmlElement("property")]
        public List<PropertyMap> Property { get; set; }

        /// <summary>
        /// Try to get a collapse key
        /// </summary>
        public bool TryGetCollapseKey(string propertyName, out CollapseKey retVal)
        {
            retVal = this.CollapseKey.Find(o => o.PropertyName == propertyName);
            return retVal != null;
        }

        /// <summary>
        /// Try to get a property map 
        /// </summary>
        public bool TryGetModelProperty(string modelName, out PropertyMap retVal)
        {
            retVal = this.Property.Find(o => o.ModelName == modelName);
            return retVal != null;
        }

        /// <summary>
        /// Validate the class map
        /// </summary>
        public IEnumerable<IResultDetail> Validate()
        {
            List<IResultDetail> retVal = new List<IResultDetail>();
            Type modelClass = Type.GetType(this.ModelClass),
                domainClass = Type.GetType(this.DomainClass);
            if (modelClass == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("Class {0} not found", this.ModelClass), null, null));
            if(domainClass == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("Class {0} not found", this.DomainClass), null, null));

            foreach(var p in this.Property)
                retVal.AddRange(p.Validate(modelClass, domainClass));
            foreach (var k in this.CollapseKey)
                retVal.AddRange(k.Validate(domainClass));

            return retVal;
        }

        /// <summary>
        /// Try to get a property map 
        /// </summary>
        public bool TryGetDomainProperty(string domainName, out PropertyMap retVal)
        {
            retVal = this.Property.Find(o => o.DomainName == domainName);
            return retVal != null;
        }
    }
}