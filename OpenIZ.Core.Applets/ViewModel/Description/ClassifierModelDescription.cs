using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// View model description for classifiers
    /// </summary>
    [XmlType(nameof(ClassifierModelDescription), Namespace = "http://openiz.org/model/view")]
    public class ClassifierModelDescription
    {

        /// <summary>
        /// Gets or sets the classifier which should be included
        /// </summary>
        [XmlAttribute("classifier")]
        public string Classifier { get; set; }
    }
}