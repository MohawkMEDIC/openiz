/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Applets.ViewModel.Description;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Protocol definition file
    /// </summary>
    [XmlType(nameof(ProtocolDefinition), Namespace = "http://openiz.org/cdss")]
    [XmlRoot(nameof(ProtocolDefinition), Namespace = "http://openiz.org/cdss")]
    public class ProtocolDefinition : DecisionSupportBaseElement
    {

        /// <summary>
        /// When clause for the entire protocol
        /// </summary>
        [XmlElement("when")]
        public ProtocolWhenClauseCollection When { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        [XmlElement("rule")]
        public List<ProtocolRuleDefinition> Rules { get; set; }

        /// <summary>
        /// View model description
        /// </summary>
        [XmlElement("loaderModel", Namespace = "http://openiz.org/model/view")]
        public ViewModelDescription Initialize { get; set; }

        /// <summary>
        /// Save the protocol definition to the specified stream
        /// </summary>
        public void Save(Stream ms)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(ProtocolDefinition));
            xsz.Serialize(ms, this);
        }

        /// <summary>
        /// Load the protocol from the stream
        /// </summary>
        public static ProtocolDefinition Load(Stream ms)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(ProtocolDefinition));
            return xsz.Deserialize(ms) as ProtocolDefinition;
        }
    }
}