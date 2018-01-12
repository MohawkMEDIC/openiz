/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Applets.ViewModel.Description;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{

    /// <summary>
    /// When the rule should be fired
    /// </summary>
    [XmlType(nameof(RuleSetTriggerEvent), Namespace = "http://openiz.org/cdss")]
    public enum RuleSetTriggerEvent
    {
        Inserting,
        Inserted,
        Updating,
        Updated,
        Obsoleting,
        Obsoleted,
        Queried
    }

    /// <summary>
    /// Represents an independent when/then condition which may or may not be executed 
    /// when trigger events occur. Rules are different than protocols in that the ruleset is
    /// fired from a trigger or called manually
    /// </summary>
    [XmlType(nameof(RuleSetDefinition), Namespace = "http://openiz.org/cdss")]
    [XmlRoot(nameof(RuleSetDefinition), Namespace = "http://openiz.org/cdss")]
    public class RuleSetDefinition : DecisionSupportBaseElement
    {

        private static XmlSerializer s_xsz = new XmlSerializer(typeof(RuleSetDefinition));

        /// <summary>
        /// Triggers for the ruleset
        /// </summary>
        [XmlElement("trigger")]
        public List<RuleSetTrigger> Triggers { get; set; }

        /// <summary>
        /// When clause for the entire ruleset
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
        [XmlElement("initialize", Namespace = "http://openiz.org/model/view")]
        public ViewModelDescription Initialize { get; set; }

        /// <summary>
        /// Save the rules definition to the specified stream
        /// </summary>
        public void Save(Stream ms)
        {
            s_xsz.Serialize(ms, this);
        }

        /// <summary>
        /// Load the rules from the stream
        /// </summary>
        public static RuleSetDefinition Load(Stream ms)
        {
            return s_xsz.Deserialize(ms) as RuleSetDefinition;
        }

    }

    /// <summary>
    /// Represents a ruleset trigger
    /// </summary>
    [XmlType(nameof(RuleSetTrigger), Namespace = "http://openiz.org/cdss")]
    public class RuleSetTrigger
    {

        /// <summary>
        /// Type of the trigger
        /// </summary>
        [XmlAttribute("type")]
        public String Type { get; set; }

        /// <summary>
        /// The event of the trigger
        /// </summary>
        [XmlAttribute("event")]
        public RuleSetTriggerEvent Event { get; set; }

    }
}
