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
 * Date: 2016-11-30
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Applets.Services;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZ.Core.Model.AMI.Security;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	public partial class AmiBehavior
	{
		/// <summary>
		/// Creates an applet.
		/// </summary>
		/// <param name="appletManifestInfo">The applet manifest info to be created.</param>
		/// <returns>Returns the created applet manifest info.</returns>
		public AppletManifestInfo CreateApplet(Stream appletData)
		{
			var pkg = AppletPackage.Load(appletData);
			ApplicationContext.Current.GetService<IAppletManagerService>().Install(pkg);
			X509Certificate2 cert = null;
			if (pkg.PublicKey != null)
				cert = new X509Certificate2(pkg.PublicKey);
			else if (pkg.Meta.PublicKeyToken != null)
			{
				X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
				try
				{
					store.Open(OpenFlags.ReadOnly);
					var results = store.Certificates.Find(X509FindType.FindByThumbprint, pkg.Meta.PublicKeyToken, false);
					if (results.Count > 0)
						cert = results[0];
				}
				finally
				{
					store.Close();
				}
			}
			return new AppletManifestInfo(pkg.Meta, new X509Certificate2Info(cert?.Issuer, cert?.NotBefore, cert?.NotAfter, cert?.Subject, cert?.Thumbprint));
		}

		/// <summary>
		/// Deletes an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be deleted.</param>
		/// <returns>Returns the deleted applet.</returns>
		/// <exception cref="System.ArgumentException">Applet not found.</exception>
		public void DeleteApplet(string appletId)
		{
			ApplicationContext.Current.GetService<IAppletManagerService>().UnInstall(appletId);
		}

		/// <summary>
		/// Downloads the applet.
		/// </summary>
		/// <param name="appletId">The applet identifier.</param>
		/// <returns>Stream.</returns>
		public Stream DownloadApplet(string appletId)
		{
			var appletService = ApplicationContext.Current.GetService<IAppletManagerService>();
			var appletData = appletService.GetPackage(appletId);

			if (appletData == null)
				throw new FileNotFoundException(appletId);
			else
			{
				var appletManifest = AppletPackage.Load(appletData);
				this.SetAppletHeaders(appletManifest.Meta);
				return new MemoryStream(appletData);
			}
		}

		/// <summary>
		/// Gets a specific applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to retrieve.</param>
		/// <returns>Returns the applet.</returns>
		/// <exception cref="System.InvalidOperationException">Applet not found.</exception>
		public AppletManifestInfo GetApplet(string appletId)
		{
			var appletService = ApplicationContext.Current.GetService<IAppletManagerService>();
			var appletData = appletService.GetPackage(appletId);
			if (appletData == null)
				throw new FileNotFoundException(appletId);
			else
			{
				var pkg = AppletPackage.Load(appletData);
				X509Certificate2 cert = null;
				if (pkg.PublicKey != null)
					cert = new X509Certificate2(pkg.PublicKey);
				else if (pkg.Meta.PublicKeyToken != null)
				{
					X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
					try
					{
						store.Open(OpenFlags.ReadOnly);
						var results = store.Certificates.Find(X509FindType.FindByThumbprint, pkg.Meta.PublicKeyToken, false);
						if (results.Count > 0)
							cert = results[0];
					}
					finally
					{
						store.Close();
					}
				}
				return new AppletManifestInfo(pkg.Meta, new X509Certificate2Info(cert?.Issuer, cert?.NotBefore, cert?.NotAfter, cert?.Subject, cert?.Thumbprint));
			}
		}

		/// <summary>
		/// Gets a list of applets for a specific query.
		/// </summary>
		/// <returns>Returns a list of applet which match the specific query.</returns>
		public AmiCollection<AppletManifestInfo> GetApplets()
		{
			return new AmiCollection<AppletManifestInfo>(ApplicationContext.Current.GetService<IAppletManagerService>().Applets.Select(o => new AppletManifestInfo(o.Info, null)));
		}

		/// <summary>
		/// Gets just the headers for the applet
		/// </summary>
		public void HeadApplet(String appletId)
		{
			var appletManifest = ApplicationContext.Current.GetService<IAppletManagerService>()?.Applets.FirstOrDefault(o => o.Info.Id == appletId);
			if (appletManifest == null)
				throw new FileNotFoundException(appletId);
			else
				this.SetAppletHeaders(appletManifest.Info);
		}

		/// <summary>
		/// Updates an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be updated.</param>
		/// <param name="appletData">The applet containing the updated information.</param>
		/// <returns>Returns the updated applet.</returns>
		/// <exception cref="System.ArgumentException">Applet not found.</exception>
		public AppletManifestInfo UpdateApplet(string appletId, Stream appletData)
		{
			var appletMgr = ApplicationContext.Current.GetService<IAppletManagerService>();
			if (!appletMgr.Applets.Any(o => o.Info.Id == appletId))
				throw new FileNotFoundException(appletId);

			var pkg = AppletPackage.Load(appletData);
			ApplicationContext.Current.GetService<IAppletManagerService>().Install(pkg, true);
			X509Certificate2 cert = null;
			if (pkg.PublicKey != null)
				cert = new X509Certificate2(pkg.PublicKey);
			else if (pkg.Meta.PublicKeyToken != null)
			{
				X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
				try
				{
					store.Open(OpenFlags.ReadOnly);
					var results = store.Certificates.Find(X509FindType.FindByThumbprint, pkg.Meta.PublicKeyToken, false);
					if (results.Count > 0)
						cert = results[0];
				}
				finally
				{
					store.Close();
				}
			}
			return new AppletManifestInfo(pkg.Meta, new X509Certificate2Info(cert?.Issuer, cert?.NotBefore, cert?.NotAfter, cert?.Subject, cert?.Thumbprint));
		}

		/// <summary>
		/// Set applet headers
		/// </summary>
		private void SetAppletHeaders(AppletInfo package)
		{
			WebOperationContext.Current.OutgoingResponse.ETag = package.Version;
			WebOperationContext.Current.OutgoingResponse.Headers.Add("X-OpenIZ-PakID", package.Id);
			if (package.Hash != null)
				WebOperationContext.Current.OutgoingResponse.Headers.Add("X-OpenIZ-Hash", Convert.ToBase64String(package.Hash));
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Type", "application/octet-stream");
			WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", $"attachment; filename=\"{package.Id}.pak.gz\"");
			WebOperationContext.Current.OutgoingResponse.Location = $"/ami/applet/{package.Id}/pak";
		}
	}
}