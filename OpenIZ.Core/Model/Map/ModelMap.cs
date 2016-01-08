using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Model map format class
    /// </summary>
    [XmlRoot("modelMap", Namespace = "http://openiz.org/model/map")]
    [XmlType(nameof(ModelMap), Namespace = "http://openiz.org/model/map")]
    public class ModelMap
    {

        // Class cache
        private Dictionary<Type, ClassMap> m_classCache = new Dictionary<Type, ClassMap>();
        // Lock object
        private Object m_lockObject = new Object();

        /// <summary>
        /// Gets or sets the class mapping
        /// </summary>
        [XmlElement("class")]
        public List<ClassMap> Class { get; set; }

        /// <summary>
        /// Get a class map for the specified type
        /// </summary>
        public ClassMap GetModelClassMap(Type type)
        {
            ClassMap retVal = null;
            if(!this.m_classCache.TryGetValue(type, out retVal))
            {
                retVal = this.Class.Find(o => Type.GetType(o.ModelClass) == type);
                if(retVal != null)
                    lock(this.m_lockObject)
                        this.m_classCache.Add(type, retVal);
            }
            return retVal;
        }
    }
}
