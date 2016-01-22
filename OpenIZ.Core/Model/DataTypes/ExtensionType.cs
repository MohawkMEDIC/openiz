using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Instructions on how an extensionshould be handled
    /// </summary>
    [Serializable]
    [DataContract(Name = "ExtensionType", Namespace = "http://openiz.org/model")]
    public class ExtensionType : BaseEntityData
    {

        /// <summary>
        /// Gets or sets the extension handler
        /// </summary>
        [IgnoreDataMember]
        public Type ExtensionHandler { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [DataMember(Name = "name")]
        public String Name { get; set; }

    }
}
