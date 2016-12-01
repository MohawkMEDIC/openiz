using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
    /// <summary>
    /// Represents an auto complete source definition 
    /// </summary>
    [XmlType(nameof(AutoCompleteSourceDefinition), Namespace = "http://openiz.org/risi")]
    public abstract class AutoCompleteSourceDefinition : BaseEntityData
    {
        
    }


    /// <summary>
    /// Represents an auto-complete source definition which is that of a query
    /// </summary>
    [XmlType(nameof(QueryAutoCompleteSourceDefinition), Namespace = "http://openiz.org/risi")]
    public class QueryAutoCompleteSourceDefinition : AutoCompleteSourceDefinition
    {
        /// <summary>
        /// Gets the source of the auto-complete source
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the query itself
        /// </summary>
        [XmlElement("query")]
        public string Query { get; set; }

    }

    /// <summary>
    /// Represents an auto complete source which is fed from a static list of members
    /// </summary>
    [XmlType(nameof(ListAutoCompleteSourceDefinition), Namespace = "http://openiz.org/risi")]
    public class ListAutoCompleteSourceDefinition : AutoCompleteSourceDefinition
    {
        /// <summary>
        /// Gets or sets the static list of auto-complete items
        /// </summary>
        [XmlElement("item")]
        public List<string> Item { get; set; }

    }
}