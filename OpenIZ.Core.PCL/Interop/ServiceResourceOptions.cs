using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Interop
{
    /// <summary>
    /// Service resource options
    /// </summary>
    [XmlType(nameof(ServiceResourceOptions), Namespace = "http://openiz.org/model"), JsonObject(nameof(ServiceResourceOptions))]
    public class ServiceResourceOptions
    {

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceOptions"/> class.
		/// </summary>
	    public ServiceResourceOptions()
	    {
		    
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResourceOptions"/> class
		/// with a specific resource name, and verbs.
		/// </summary>
		/// <param name="resourceName">The name of the resource of the service resource options.</param>
		/// <param name="verbs">The list of HTTP verbs of the resource option.</param>
	    public ServiceResourceOptions(string resourceName, List<string> verbs)
	    {
		    this.ResourceName = resourceName;
		    this.Verbs = verbs;
	    }

        /// <summary>
        /// Gets the name of the resource
        /// </summary>
        [XmlAttribute("resource"), JsonProperty("resource")]
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets the verbs supported on the specified resource
        /// </summary>
        [XmlElement("verb"), JsonProperty("verb")]
        public List<string> Verbs { get; set; }
    }
}