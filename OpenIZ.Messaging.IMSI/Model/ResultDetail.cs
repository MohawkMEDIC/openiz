/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-24
 */
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