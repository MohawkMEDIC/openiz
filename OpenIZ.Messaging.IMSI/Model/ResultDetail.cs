using System.Xml.Serialization;

namespace OpenIZ.Messaging.IMSI.Model
{

    /// <summary>
    /// Gets or sets the detail type
    /// </summary>
    [XmlType(nameof(DetailType), Namespace = "http://openiz.org/imsi")]
    public enum DetailType
    {
        [XmlEnum("I")]
        Information,
        [XmlEnum("W")]
        Warning,
        [XmlEnum("E")]
        Error
    }

    /// <summary>
    /// A single result detail
    /// </summary
    [XmlType(nameof(ResultDetail), Namespace = "http://openiz.org/imsi")]
    public class ResultDetail
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public ResultDetail()
        { }

        /// <summary>
        /// Creates a new result detail
        /// </summary>
        public ResultDetail(DetailType type, string text)
        {
            this.Type = type;
            this.Text = text;
        }
        /// <summary>
        /// Gets or sets the type of the error
        /// </summary>
        [XmlAttribute("type")]
        public DetailType Type { get; set; }

        /// <summary>
        /// Gets or sets the text of the error
        /// </summary>
        [XmlText]
        public string Text { get; set; }
    }
}