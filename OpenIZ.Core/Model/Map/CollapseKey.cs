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
    /// Association map
    /// </summary>
    [XmlType(nameof(CollapseKey), Namespace = "http://openiz.org/model/map")]
    public class CollapseKey
    {
        /// <summary>
        /// Gets or sets the name of the property can be collapsed if a key is used
        /// </summary>
        [XmlAttribute("propertyName")]
        public String PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the key in the domain model which "PropertyName" can be collapsed
        /// </summary>
        [XmlAttribute("keyName")]
        public String KeyName { get; set; }


        /// <summary>
        /// Validate the collapse key
        /// </summary>
        public IEnumerable<IResultDetail> Validate(Type domainClass)
        {
            List<IResultDetail> retVal = new List<IResultDetail>();
            if (domainClass?.GetProperty(this.PropertyName) == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} not found", domainClass?.Name, this.PropertyName), null, null));
            if (domainClass?.GetProperty(this.KeyName) == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} not found", domainClass?.Name, this.KeyName), null, null));
            if(domainClass?.GetProperty(this.KeyName)?.PropertyType != typeof(Guid) &&
                domainClass?.GetProperty(this.KeyName)?.PropertyType != typeof(Guid?) &&
                domainClass?.GetProperty(this.KeyName)?.PropertyType != typeof(Decimal) &&
                domainClass?.GetProperty(this.KeyName)?.PropertyType != typeof(Decimal?))
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} must be one of [Guid, Nullable<Guid>, Decimal, Nullable<Decimal>]", domainClass?.Name, this.KeyName), null, null));
            return retVal;
        }
    }
}