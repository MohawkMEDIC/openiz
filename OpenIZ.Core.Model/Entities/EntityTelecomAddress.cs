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
 * Date: 2016-7-16
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// Represents an entity telecom address
	/// </summary>
	[Classifier(nameof(AddressUse))]
	[XmlType("EntityTelecomAddress", Namespace = "http://openiz.org/model"), JsonObject("EntityTelecomAddress")]
	public class EntityTelecomAddress : VersionedAssociation<Entity>
	{
		private Concept m_nameUseConcept;

		// Name use key
		private Guid? m_nameUseKey;

		// Name use concept
		/// <summary>
		/// Default constructor
		/// </summary>
		public EntityTelecomAddress()
		{
		}

		/// <summary>
		/// Creates a new entity telecom address with specified use and value
		/// </summary>
		public EntityTelecomAddress(Guid addressUseKey, String value)
		{
			this.AddressUseKey = addressUseKey;
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the name use
		/// </summary>
		[SerializationReference(nameof(AddressUseKey)), AutoLoad]
		[XmlIgnore, JsonIgnore]
		public Concept AddressUse
		{
			get
			{
				this.m_nameUseConcept = base.DelayLoad(this.m_nameUseKey, this.m_nameUseConcept);
				return this.m_nameUseConcept;
			}
			set
			{
				this.m_nameUseConcept = value;
				this.m_nameUseKey = value?.Key;
			}
		}

		/// <summary>
		/// Gets or sets the name use key
		/// </summary>
		[XmlElement("use"), JsonProperty("use")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Binding(typeof(TelecomAddressUseKeys))]
		public Guid? AddressUseKey
		{
			get { return this.m_nameUseKey; }
			set
			{
				if (this.m_nameUseKey != value)
				{
					this.m_nameUseKey = value;
					this.m_nameUseConcept = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value as an IETF value
		/// </summary>
		[XmlIgnore, JsonIgnore, DataIgnore]
		public String IETFValue
		{
			get
			{
				if (this.Value == null) return null;

				// E-mail?
				Regex email = new Regex(".+?@.+?\\..+?"),
					tel = new Regex(@"([+0-9A-Za-z]{1,4})?\((\d{3})\)?(\d{3})\-(\d{4})X?(\d{1,6})?");

				if (email.IsMatch(this.Value))
					return String.Format("mailto:{0}", this.Value);
				else if (tel.IsMatch(this.Value))
				{
					var match = tel.Match(this.Value);
					StringBuilder sb = new StringBuilder("tel:");

					for (int i = 1; i < 5; i++)
						if (!String.IsNullOrEmpty(match.Groups[i].Value))
							sb.AppendFormat("{0}{1}", match.Groups[i].Value, i == 4 ? "" : "-");
					if (!string.IsNullOrEmpty(match.Groups[5].Value))
						sb.AppendFormat(";ext={0}", match.Groups[5].Value);

					return sb.ToString();
				}
				else
					return this.Value;
			}
			set
			{
				Regex re = new Regex(@"^(?<s1>(?<s0>[^:/\?#]+):)?(?<a1>//(?<a0>[^/\;#]*))?(?<p0>[^\;#]*)(?<q1>\;(?<q0>[^#]*))?(?<f1>#(?<f0>.*))?");

				// Match
				var match = re.Match(value);
				if (match.Groups[1].Value != "tel:")
				{
					this.Value = value.Substring(value.IndexOf(":"));
					return;
				}

				// Telephone
				string[] comps = match.Groups[5].Value.Split('-');
				StringBuilder sb = new StringBuilder(),
					phone = new StringBuilder();
				for (int i = 0; i < comps.Length; i++)
					if (i == 0 && comps[i].Contains("+"))
						sb.Append(comps[i]);
					else if (sb.Length == 0 && comps.Length == 3 ||
						comps.Length == 4 && i == 1) // area code?
						sb.AppendFormat("({0})", comps[i]);
					else if (i != comps.Length - 1)
					{
						sb.AppendFormat("{0}-", comps[i]);
						phone.AppendFormat("{0}", comps[i]);
					}
					else
					{
						sb.Append(comps[i]);
						phone.Append(comps[i]);
					}

				// Extension?
				string[] parms = match.Groups[7].Value.Split(';');
				foreach (var parm in parms)
				{
					string[] pData = parm.Split('=');
					if (pData[0] == "extension" || pData[0] == "ext" || pData[0] == "postd")
					{
						sb.AppendFormat("X{0}", pData[1]);
					}
				}

				this.Value = sb.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the value of the telecom address
		/// </summary>
		[XmlElement("value"), JsonProperty("value")]
		public String Value { get; set; }

		/// <summary>
		/// Empty
		/// </summary>
		/// <returns></returns>
		public override bool IsEmpty()
		{
			return String.IsNullOrEmpty(this.Value);
		}

		/// <summary>
		/// Forces refresh of the delay load properties
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();
			this.m_nameUseConcept = null;
		}

		/// <summary>
		/// Semantic equality function
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as EntityTelecomAddress;
			if (other == null) return false;
			return base.SemanticEquals(obj) && this.Value == other.Value && this.AddressUseKey == other.AddressUseKey;
		}
	}
}