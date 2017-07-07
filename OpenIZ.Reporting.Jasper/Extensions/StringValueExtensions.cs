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
 * Date: 2017-4-22
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Reporting.Jasper.Attributes;

namespace OpenIZ.Reporting.Jasper.Extensions
{
	/// <summary>
	/// Represents extensions for the <see cref="StringValueAttribute"/> class.
	/// </summary>
	public static class StringValueExtensions
	{
		/// <summary>
		/// Gets a string value for an enum.
		/// </summary>
		/// <param name="obj">The enum for which to return the string value for.</param>
		/// <returns>Returns a string value for an enum.</returns>
		/// <exception cref="System.InvalidOperationException">If the enum value is not marked with the <see cref="StringValueAttribute"/> class.</exception>
		public static string GetStringValue(this Enum obj)
		{
			var fieldInfo = obj.GetType().GetField(obj.ToString());

			var attributes = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

			if (attributes == null)
			{
				throw new InvalidOperationException($"{fieldInfo.Name} is not marked with {nameof(StringValueAttribute)}");
			}

			var output = (StringValueAttribute)attributes.GetValue(0);

			return output.Text;
		}
	}
}
