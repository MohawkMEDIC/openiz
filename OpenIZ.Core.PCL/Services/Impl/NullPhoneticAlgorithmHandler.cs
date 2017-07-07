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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Interfaces;
using OpenIZ.Core.Model.Constants;
using System;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// null algorithm phonetic algorithm
	/// </summary>
	public class NullPhoneticAlgorithmHandler : IPhoneticAlgorithmHandler
	{
		/// <summary>
		/// Gets the algorithm id
		/// </summary>
		public Guid AlgorithmId
		{
			get
			{
				return PhoneticAlgorithmKeys.None;
			}
		}

		/// <summary>
		/// Generate the phonetic code
		/// </summary>
		public string GenerateCode(string input)
		{
			return null;
		}
	}
}