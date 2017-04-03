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
 * Date: 2016-7-16
 */

using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// Represents an entity which is a person
	/// </summary>

	[XmlType("Person", Namespace = "http://openiz.org/model"), JsonObject("Person")]
	[XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Person")]
	public class Person : Entity
	{
		/// <summary>
		/// Person constructor
		/// </summary>
		public Person()
		{
			base.DeterminerConceptKey = DeterminerKeys.Specific;
			base.ClassConceptKey = EntityClassKeys.Person;
			this.LanguageCommunication = new List<PersonLanguageCommunication>();
		}

		/// <summary>
		/// Gets the security user account associated with this person if applicable
		/// </summary>
		[XmlIgnore, JsonIgnore, DataIgnore]
		public SecurityUser AsSecurityUser
		{
			get
			{
				return EntitySource.Current.Get<UserEntity>(this.Key, this.VersionKey)?.SecurityUser;
			}
		}

		/// <summary>
		/// Gets or sets the person's date of birth
		/// </summary>
		[XmlIgnore, JsonIgnore]
		public DateTime? DateOfBirth { get; set; }

		/// <summary>
		/// Gets or sets the precision ofthe date of birth
		/// </summary>
		[XmlElement("dateOfBirthPrecision"), JsonProperty("dateOfBirthPrecision")]
		public DatePrecision? DateOfBirthPrecision { get; set; }

		/// <summary>
		/// Gets the date of birth as XML
		/// </summary>
		[XmlElement("dateOfBirth"), JsonProperty("dateOfBirth"), DataIgnore]
		public String DateOfBirthXml
		{
			get
			{
				return this.DateOfBirth?.ToUniversalTime().ToString("o");
			}
			set
			{
				if (!String.IsNullOrEmpty(value))
					this.DateOfBirth = DateTime.ParseExact(value, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
				else
					this.DateOfBirth = null;
			}
		}

		/// <summary>
		/// Gets the person's languages of communication
		/// </summary>
		[AutoLoad, XmlElement("language"), JsonProperty("language")]
		public List<PersonLanguageCommunication> LanguageCommunication { get; set; }

		/// <summary>
		/// Semantic equality function
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as Person;
			if (other == null) return false;
			return base.SemanticEquals(obj) &&
				this.DateOfBirth == other.DateOfBirth &&
				this.DateOfBirthPrecision == other.DateOfBirthPrecision &&
				this.LanguageCommunication?.SemanticEquals(other.LanguageCommunication) == true;
		}
	}
}