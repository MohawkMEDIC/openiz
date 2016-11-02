using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// Represents a refined message model
    /// </summary>
    [XmlType(nameof(ViewModelDescription), Namespace = "http://openiz.org/model/view")]
    [XmlRoot("ViewModel", Namespace = "http://openiz.org/model/view")]
    public class ViewModelDescription
    {

        // Description lookup
        private Dictionary<Object, PropertyContainerDescription> m_description = new Dictionary<object, PropertyContainerDescription>();

        // Lock object
        protected object m_lockObject = new object();

        /// <summary>
        /// Defaut ctor
        /// </summary>
        public ViewModelDescription()
        {
            this.Model = new List<TypeModelDescription>();
        }

        /// <summary>
        /// Gets or sets the name of the view model description
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        /// <summary>
        /// Represents the models which are to be defined in the model
        /// </summary>
        [XmlElement("type")]
        public List<TypeModelDescription> Model { get; set; }

        /// <summary>
        /// Load the specified view model description
        /// </summary>
        public static ViewModelDescription Load(Stream stream)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(ViewModelDescription));
            return xsz.Deserialize(stream) as ViewModelDescription;
        }

        /// <summary>
        /// Find description
        /// </summary>
        public PropertyContainerDescription FindDescription(Type rootType)
        {
            PropertyContainerDescription retVal = null;
            if (!this.m_description.TryGetValue(rootType, out retVal))
            {
                string typeName = rootType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                                   rootType.Name;
                retVal = this.Model.FirstOrDefault(o => o.TypeName == typeName);
                // Children from the heirarchy
                while (rootType != typeof(IdentifiedData) && retVal == null)
                {
                    rootType = rootType.GetTypeInfo().BaseType;
                    if (rootType == null) break;
                    typeName = rootType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                        rootType.Name;

                    if(!this.m_description.TryGetValue(rootType, out retVal))
                        retVal = this.Model.FirstOrDefault(o => o.TypeName == typeName);
                }

                if (retVal != null)
                    lock (this.m_lockObject)
                        if (!this.m_description.ContainsKey(rootType))
                            this.m_description.Add(rootType, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Find description
        /// </summary>
        public PropertyContainerDescription FindDescription(PropertyInfo propertyInfo, String propertyName, PropertyContainerDescription context)
        {
            PropertyContainerDescription retVal = null;
            if (!this.m_description.TryGetValue(propertyInfo, out retVal))
            {
                // Find the property information
                retVal = context?.Properties.FirstOrDefault(o => o.Name == propertyName);

                // Maybe this can be done via type?
                if (retVal == null)
                {
                    var elementType = propertyInfo.PropertyType;

                    if (elementType.GetTypeInfo().IsGenericType)
                        elementType = elementType.GetTypeInfo().GenericTypeArguments[0];

                    retVal = this.FindDescription(elementType);
                }

                if (retVal != null)
                    lock (this.m_lockObject)
                        if(!this.m_description.ContainsKey(propertyInfo))
                            this.m_description.Add(propertyInfo, retVal);
            }
            return retVal;
        }
    }
}
