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
 * Date: 2016-8-15
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Detected issue priority
	/// </summary>
	public enum DetectedIssuePriorityType : int
	{
		Error = 1,
		Informational = 2,
		Warning = 4
	}

	/// <summary>
	/// Represents a service that executes business rules based on triggers which happen in the persistence layer
	/// </summary>
	/// <remarks>
	/// Note: This can be done, instead with events on the persistence layer on the OpenIZ datalayer, however there
	/// may come a time when a rule is fired without persistence occurring
	/// </remarks>
	public interface IBusinessRulesService<TModel> where TModel : IdentifiedData
	{
		/// <summary>
		/// Called after an insert occurs
		/// </summary>
		TModel AfterInsert(TModel data);

		/// <summary>
		/// Called after obsolete committed
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		TModel AfterObsolete(TModel data);

		/// <summary>
		/// Called after query
		/// </summary>
		IEnumerable<TModel> AfterQuery(IEnumerable<TModel> results);

		/// <summary>
		/// Called after retrieve
		/// </summary>
		TModel AfterRetrieve(TModel result);

		/// <summary>
		/// Called after update committed
		/// </summary>
		TModel AfterUpdate(TModel data);

		/// <summary>
		/// Called before an insert occurs
		/// </summary>
		TModel BeforeInsert(TModel data);

		/// <summary>
		/// Called before obsolete
		/// </summary>
		TModel BeforeObsolete(TModel data);

		/// <summary>
		/// Called before an update occurs
		/// </summary>
		TModel BeforeUpdate(TModel data);

		/// <summary>
		/// Called to validate a specific object
		/// </summary>
		List<DetectedIssue> Validate(TModel data);
	}

	/// <summary>
	/// Represents a detected issue
	/// </summary>
	[JsonObject(nameof(DetectedIssue))]
	[XmlType(nameof(DetectedIssue), Namespace = "http://openiz.org/issue")]
	public class DetectedIssue
	{
		/// <summary>
		/// Represents a detected issue priority
		/// </summary>
		[XmlAttribute("priority"), JsonProperty("priority")]
		public DetectedIssuePriorityType Priority { get; set; }

		/// <summary>
		/// Text related to the issue
		/// </summary>
		[XmlText, JsonProperty("text")]
		public String Text { get; set; }

		/// <summary>
		/// The type of issue (a concept)
		/// </summary>
		[XmlAttribute("type"), JsonProperty("type")]
		public Guid TypeKey { get; set; }
	}
}