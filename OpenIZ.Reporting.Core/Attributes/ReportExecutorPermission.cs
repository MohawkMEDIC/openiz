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
 * Date: 2017-4-4
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Reporting.Core.Configuration;
using OpenIZ.Reporting.Core.Exceptions;

namespace OpenIZ.Reporting.Core.Attributes
{
	/// <summary>
	/// Represents a report executor permission.
	/// </summary>
	/// <seealso cref="System.Security.IPermission" />
	public class ReportExecutorPermission : IPermission
	{
		/// <summary>
		/// Creates an XML encoding of the security object and its current state.
		/// </summary>
		/// <returns>An XML encoding of the security object, including any state information.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public SecurityElement ToXml()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reconstructs a security object with a specified state from an XML encoding.
		/// </summary>
		/// <param name="e">The XML encoding to use to reconstruct the security object.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void FromXml(SecurityElement e)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates and returns an identical copy of the current permission.
		/// </summary>
		/// <returns>A copy of the current permission.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IPermission Copy()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates and returns a permission that is the intersection of the current permission and the specified permission.
		/// </summary>
		/// <param name="target">A permission to intersect with the current permission. It must be of the same type as the current permission.</param>
		/// <returns>A new permission that represents the intersection of the current permission and the specified permission. This new permission is null if the intersection is empty.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IPermission Intersect(IPermission target)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a permission that is the union of the current permission and the specified permission.
		/// </summary>
		/// <param name="target">A permission to combine with the current permission. It must be of the same type as the current permission.</param>
		/// <returns>A new permission that represents the union of the current permission and the specified permission.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IPermission Union(IPermission target)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines whether the current permission is a subset of the specified permission.
		/// </summary>
		/// <param name="target">A permission that is to be tested for the subset relationship. This permission must be of the same type as the current permission.</param>
		/// <returns>true if the current permission is a subset of the specified permission; otherwise, false.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool IsSubsetOf(IPermission target)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Throws a <see cref="T:System.Security.SecurityException" /> at run time if the security requirement is not met.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Unable to locate report executor service</exception>
		/// <exception cref="System.NotImplementedException">Bearer Authentication</exception>
		/// <exception cref="OpenIZ.Reporting.Core.Exceptions.CertificateNotFoundException">If the certificate is not found in the certificate store.</exception>
		public void Demand()
		{
			var configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.reporting.core") as ReportingConfiguration;

			var reportExecutor = ApplicationContext.Current.GetService<IReportExecutor>();

			if (reportExecutor == null)
			{
				throw new InvalidOperationException("Unable to locate report executor service");
			}

			if (reportExecutor is IAuthenticationHandler)
			{
				(reportExecutor as IAuthenticationHandler).OnAuthenticationError += ReportExecutorPermission_OnAuthenticationError;
			}

			if (reportExecutor is ISupportBasicAuthentication)
			{
				var credential = configuration.Credentials.Credential as UsernamePasswordCredential;
				(reportExecutor as ISupportBasicAuthentication).Authenticate(credential.Username, credential.Password);
				(reportExecutor as ISupportBasicAuthentication).OnAuthenticationError -= ReportExecutorPermission_OnAuthenticationError;
			}
			else if (reportExecutor is ISupportBearerAuthentication)
			{
				throw new NotImplementedException();
			}
			else if (reportExecutor is ISupportCertificateAuthentication)
			{
				var credential = configuration.Credentials.Credential as CertificateCredential;

				var store = new X509Store(StoreName.My, credential.StoreLocation);

				store.Open(OpenFlags.ReadOnly);
				var certificateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, credential.Thumbprint, true);

				if (certificateCollection.Count == 0)
				{
					throw new CertificateNotFoundException($"Certificate not found using thumbprint: {credential.Thumbprint}");
				}

				(reportExecutor as ISupportCertificateAuthentication).Authenticate(certificateCollection[0]);
				(reportExecutor as ISupportCertificateAuthentication).OnAuthenticationError -= ReportExecutorPermission_OnAuthenticationError;
			}
			else
			{
				throw new InvalidOperationException($"{reportExecutor.GetType().AssemblyQualifiedName} has no supported authentication mechanisms");
			}
		}

		/// <summary>
		/// Handles the OnAuthenticationError event of the ReportExecutorPermission control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="Event.AuthenticationErrorEventArgs"/> instance containing the event data.</param>
		/// <exception cref="ReportExecutionAuthorizationViolationException"></exception>
		private void ReportExecutorPermission_OnAuthenticationError(object sender, Event.AuthenticationErrorEventArgs e)
		{
			throw new ReportExecutionAuthorizationViolationException(e);
		}
	}
}
