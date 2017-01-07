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
 * User: khannan
 * Date: 2017-1-5
 */

using System;

namespace OpenIZ.Reporting.Core.Attributes
{
	/// <summary>
	/// Represents an attribute which defines a file extension for a given field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FileExtensionAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileExtensionAttribute"/> class.
		/// </summary>
		/// <param name="extension">The value of the file extension.</param>
		public FileExtensionAttribute(string extension)
		{
			if (extension.StartsWith("."))
			{
				throw new ArgumentException("The extension value cannot start with a '.'");
			}

			this.Extension = extension;
		}

		/// <summary>
		/// Gets or sets the file extension.
		/// </summary>
		public string Extension { get; }
	}
}