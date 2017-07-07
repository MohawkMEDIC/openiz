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
 * Date: 2017-4-7
 */
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using OpenIZ.Reporting.Core.Event;

namespace OpenIZ.Reporting.Core.Exceptions
{
	/// <summary>
	/// Thrown when a report execution authorization is violated.
	/// </summary>
	public class ReportExecutionAuthorizationViolationException : SecurityException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		public ReportExecutionAuthorizationViolationException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public ReportExecutionAuthorizationViolationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		/// <param name="args">The <see cref="AuthenticationErrorEventArgs"/> instance containing the event data.</param>
		public ReportExecutionAuthorizationViolationException(AuthenticationErrorEventArgs args) : this(args.Message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="inner">The exception that is the cause of the current exception. If the <paramref name="inner" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception.</param>
		public ReportExecutionAuthorizationViolationException(string message, System.Exception inner) : base(message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="type">The type of the permission that caused the exception to be thrown.</param>
		public ReportExecutionAuthorizationViolationException(string message, Type type) : base(message, type)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="type">The type of the permission that caused the exception to be thrown.</param>
		/// <param name="state">The state of the permission that caused the exception to be thrown.</param>
		public ReportExecutionAuthorizationViolationException(string message, Type type, string state) : base(message, type, state)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="deny">The denied permission or permission set.</param>
		/// <param name="permitOnly">The permit-only permission or permission set.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that identifies the method that encountered the exception.</param>
		/// <param name="demanded">The demanded permission, permission set, or permission set collection.</param>
		/// <param name="permThatFailed">An <see cref="T:System.Security.IPermission" /> that identifies the permission that failed.</param>
		public ReportExecutionAuthorizationViolationException(string message, object deny, object permitOnly, MethodInfo method, object demanded, IPermission permThatFailed) : base(message, deny, permitOnly, method, demanded, permThatFailed)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="assemblyName">An <see cref="T:System.Reflection.AssemblyName" /> that specifies the name of the assembly that caused the exception.</param>
		/// <param name="grant">A <see cref="T:System.Security.PermissionSet" /> that represents the permissions granted the assembly.</param>
		/// <param name="refused">A <see cref="T:System.Security.PermissionSet" /> that represents the refused permission or permission set.</param>
		/// <param name="method">A <see cref="T:System.Reflection.MethodInfo" /> that represents the method that encountered the exception.</param>
		/// <param name="action">One of the <see cref="T:System.Security.Permissions.SecurityAction" /> values.</param>
		/// <param name="demanded">The demanded permission, permission set, or permission set collection.</param>
		/// <param name="permThatFailed">An <see cref="T:System.Security.IPermission" /> that represents the permission that failed.</param>
		/// <param name="evidence">The <see cref="T:System.Security.Policy.Evidence" /> for the assembly that caused the exception.</param>
		public ReportExecutionAuthorizationViolationException(string message, AssemblyName assemblyName, PermissionSet grant, PermissionSet refused, MethodInfo method, SecurityAction action, object demanded, IPermission permThatFailed, Evidence evidence) : base(message, assemblyName, grant, refused, method, action, demanded, permThatFailed, evidence)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportExecutionAuthorizationViolationException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ReportExecutionAuthorizationViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
