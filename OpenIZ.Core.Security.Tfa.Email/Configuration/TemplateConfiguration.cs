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
 * Date: 2016-11-30
 */
namespace OpenIZ.Core.Security.Tfa.Email.Configuration
{
	/// <summary>
	/// Represents template configuration
	/// </summary>
	public class TemplateConfiguration
	{
		/// <summary>
		/// Template configuration file
		/// </summary>
		public TemplateConfiguration(string lang, string file)
		{
			this.Language = lang;
			this.TemplateDefinitionFile = file;
		}

		/// <summary>
		/// Gets the language of the tempalte
		/// </summary>
		public string Language { get; private set; }

		/// <summary>
		/// Gets the file
		/// </summary>
		public string TemplateDefinitionFile { get; private set; }
	}
}