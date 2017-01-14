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
 * Date: 2017-1-12
 */

using System;
using System.Runtime.Serialization;

namespace OpenIZ.Persistence.Reporting.Exceptions
{
	/// <summary>
	/// Represents an exception thrown when an entity is not found.
	/// </summary>
	internal class EntityNotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
		/// </summary>
		public EntityNotFoundException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
		/// with a specific message.
		/// </summary>
		/// <param name="message">The message of the exception.</param>
		public EntityNotFoundException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The message of the exception.</param>
		/// <param name="innerException">The inner exception.</param>
		public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
		/// </summary>
		/// <param name="info">The serialization information.</param>
		/// <param name="context">The context information.</param>
		protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}