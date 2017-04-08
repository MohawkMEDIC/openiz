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
 * User: Nityan
 * Date: 2017-4-8
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Linq;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	public partial class AmiBehavior
	{
		/// <summary>
		/// Creates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the created code system.</returns>
		public CodeSystem CreateCodeSystem(CodeSystem codeSystem)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the code system.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns a code system.</returns>
		public CodeSystem GetCodeSystem(string id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the code systems.
		/// </summary>
		/// <returns>Returns a list of code systems.</returns>
		public AmiCollection<CodeSystem> GetCodeSystems()
		{
			throw new NotImplementedException();
		}
	}
}