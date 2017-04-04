/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-8-29
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Alert.Alerting
{
	/// <summary>
	/// Represents an alert message.
	/// </summary>
	[JsonObject(nameof(AlertMessage)), XmlType(nameof(AlertMessage), Namespace = "http://openiz.org/alerting")]
	public class AlertMessage : NonVersionedEntityData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AlertMessage"/> class.
		/// </summary>
		public AlertMessage()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AlertMessage"/> class
		/// with a specified from, to, subject, body, and alert message flags.
		/// </summary>
		/// <param name="from">The sender of the alert.</param>
		/// <param name="to">The recipient of the alert.</param>
		/// <param name="subject">The subject of the alert.</param>
		/// <param name="body">The body of the alert.</param>
		/// <param name="flags">The flags of the alert.</param>
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
		/// Gets or sets the alert body of the alert.
		/// </summary>
		[JsonProperty("body"), XmlElement("body")]
		public string Body { get; set; }

		/// <summary>
		/// Gets or sets the status of the alert.
		/// </summary>
		[JsonProperty("flags"), XmlElement("flags")]
		public AlertMessageFlags Flags { get; set; }

		/// <summary>
		/// Gets or sets the "from" subject if it is a human based message of the alert.
		/// </summary>
		[JsonProperty("from"), XmlElement("from")]
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the subject of the alert.
		/// </summary>
		[JsonProperty("subject"), XmlElement("subject")]
		public string Subject { get; set; }

        /// <summary>
        /// Date/time of the alert
        /// </summary>
        [XmlIgnore, JsonIgnore]
		public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the time of the alert.
        /// </summary>
        [JsonProperty("time"), XmlElement("time")]
        public String DateTimeXml {
            get { return this.TimeStamp.DateTime.ToString("o"); }
            set { this.TimeStamp = DateTime.Parse(value); }
        }

		/// <summary>
		/// Gets or sets the recipient of the alert in a human readable form
		/// </summary>
		[JsonProperty("to"), XmlElement("to")]
		public String To { get; set; }

        /// <summary>
        /// The recipient users used for query
        /// </summary>
        [JsonProperty("rcpt"), XmlElement("rcpt")]
        public List<SecurityUser> RcptTo { get; set; }

        /// <summary>
        /// Gets or sets the time this was modified on
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public override DateTimeOffset ModifiedOn
        {
            get
            {
                return this.CreationTime;
            }
        }
    }
}