using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Alerting
{

    /// <summary>
    /// A message
    /// </summary>
    [JsonObject(nameof(AlertMessage)), XmlType(nameof(AlertMessage), Namespace = "http://openiz.org/alerting")]
    public class AlertMessage
    {
        /// <summary>
        /// Creates alert message
        /// </summary>
        public AlertMessage()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new alert message
        /// </summary>
        public AlertMessage(String from, String to, String subject, String body, AlertMessageFlags flags = AlertMessageFlags.None)
        {
            this.TimeStamp = DateTime.Now;
            this.From = from;
            this.Subject = subject;
            this.Body = body;
            this.To = to;
            this.Flags = flags;
        }

        /// <summary>
        /// Gets or sets the id of the alert
        /// </summary>
        [JsonProperty("id"), XmlElement("id")]
        public Guid Id { get; set; }


        /// <summary>
        /// Gets or sets the time
        /// </summary>
        [JsonProperty("time"), XmlElement("time")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the status of the alert
        /// </summary>
        [JsonProperty("flags"), XmlElement("flags")]
        public AlertMessageFlags Flags { get; set; }

        /// <summary>
        /// The principal that created the message
        /// </summary>
        [JsonProperty("createdBy"), XmlElement("createdBy")]
        public String CreatedBy { get; set; }

        /// <summary>
        /// Identifies the to
        /// </summary>
        [JsonProperty("to"), XmlElement("to")]
        public String To { get; set; }

        /// <summary>
        /// Gets or sets the "from" subject if it is a human based message
        /// </summary>
        [JsonProperty("from"), XmlElement("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [JsonProperty("subject"), XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Body of the message
        /// </summary>
        [JsonProperty("body"), XmlElement("body")]
        public string Body { get; set; }

    }

    /// <summary>
    /// Message status type
    /// </summary>
    public enum AlertMessageFlags
    {
        /// <summary>
        /// Just a normal alert
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Indicates the message requires some immediate action!
        /// </summary>
        Alert = 0x1,
        /// <summary>
        /// Indicates whether someone has acknowledged the alert
        /// </summary>
        Acknowledged = 0x2,
        /// <summary>
        /// Indicates the alert is high priority but doesn't require immediate action
        /// </summary>
        HighPriority = 0x4,
        /// <summary>
        /// Indicates the alert is a system alert
        /// </summary>
        System = 0x8,
        /// <summary>
        /// Indicates the alert is transient and shouldn't be persisted
        /// </summary>
        Transient = 0x10,
        HighPriorityAlert = HighPriority | Alert
    }
}