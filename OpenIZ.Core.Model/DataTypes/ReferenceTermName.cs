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
 * User: justi
 * Date: 2016-7-16
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
	/// <summary>
	/// Display name of a code system or reference term
	/// </summary>
	[XmlType("ReferenceTermName", Namespace = "http://openiz.org/model"), JsonObject("ReferenceTermName")]
	[Classifier(nameof(Language)), SimpleValue(nameof(Name))]
	public class ReferenceTermName : BaseEntityData, ISimpleAssociation
	{

		// Id of the algorithm used to generate phonetic code
		private Guid? m_phoneticAlgorithmId;
		// Algorithm used to generate the code

		private PhoneticAlgorithm m_phoneticAlgorithm;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceTermName"/> class.
		/// </summary>
		public ReferenceTermName()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceTermName"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public ReferenceTermName(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceTermName"/> class.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <param name="name">The name.</param>
		public ReferenceTermName(string language, string name)  : this(name)
		{
			this.Language = language;
		}

		/// <summary>
		/// Should serialize reference term key
		/// </summary>
		public bool ShouldSerializeSourceEntityKey()
		{
			return this.SourceEntityKey.HasValue;
		}

		/// <summary>
		/// Gets or sets the language code of the object
		/// </summary>
		[XmlElement("language"), JsonProperty("language")]
		public String Language { get; set; }

		/// <summary>
		/// Gets or sets the name of the reference term
		/// </summary>
		[XmlElement("value"), JsonProperty("value")]
		public String Name { get; set; }

		/// <summary>
		/// Gets or sets the phonetic code of the reference term
		/// </summary>
		[XmlElement("phoneticCode"), JsonProperty("phoneticCode")]
		public String PhoneticCode { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the phonetic code
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Binding(typeof(PhoneticAlgorithmKeys))]
		[XmlElement("phoneticAlgorithm"), JsonProperty("phoneticAlgorithm")]
		public Guid? PhoneticAlgorithmKey
		{
			get { return this.m_phoneticAlgorithmId; }
			set
			{
				this.m_phoneticAlgorithmId = value;
				this.m_phoneticAlgorithm = null;
			}
		}

		/// <summary>
		/// Gets or sets the phonetic algorithm
		/// </summary>
		[SerializationReference(nameof(PhoneticAlgorithmKey))]
		[XmlIgnore, JsonIgnore]
		public PhoneticAlgorithm PhoneticAlgorithm
		{
			get
			{
				this.m_phoneticAlgorithm = base.DelayLoad(this.m_phoneticAlgorithmId, this.m_phoneticAlgorithm);
				return this.m_phoneticAlgorithm;
			}
			set
			{
				this.m_phoneticAlgorithm = value;
				this.m_phoneticAlgorithmId = value?.Key;
			}
		}

		/// <summary>
		/// Gets the source entity key
		/// </summary>
		[XmlElement("source"), JsonProperty("source")]
		public Guid? SourceEntityKey
		{
			get; set;
		}

		/// <summary>
		/// Force reloading of delay load properties
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();
			this.m_phoneticAlgorithm = null;
		}
	}
}