/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-11-23
 */

using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZ.Core.Model.AMI.Security;
using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;

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
		public AppletManifestInfo CreateApplet(AppletManifestInfo appletManifestInfo)
		{
			// creates the applets directory where the applets will live
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			var appletDirectory = Path.Combine(baseDirectory, "applets");

			if (!appletManifestInfo.FileExtension.StartsWith("."))
			{
				appletManifestInfo.FileExtension = appletManifestInfo.FileExtension.Insert(0, ".");
			}

			switch (appletManifestInfo.FileExtension.ToLowerInvariant())
			{
				case ".pak":
				case ".pak.gz":
					using (var fileStream = File.Create(Path.Combine(appletDirectory, appletManifestInfo.AppletManifest.Info.Id + appletManifestInfo.FileExtension)))
					using (var gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
					{
						var package = appletManifestInfo.AppletManifest.CreatePackage();
						var serializer = new XmlSerializer(typeof(AppletPackage));

						serializer.Serialize(gzipStream, package);
					}
					break;
			}

			return appletManifestInfo;
		}

		/// <summary>
		/// Deletes an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be deleted.</param>
		/// <returns>Returns the deleted applet.</returns>
		public AppletManifestInfo DeleteApplet(string appletId)
		{
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			var appletDirectory = Path.Combine(baseDirectory, "applets");

			var file = Directory.GetFiles(appletDirectory).FirstOrDefault(f => f == appletId);

			if (file == null)
			{
				throw new ArgumentException("Applet not found");
			}

			File.Delete(file);

			return new AppletManifestInfo();
		}

		/// <summary>
		/// Gets a specific applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to retrieve.</param>
		/// <returns>Returns the applet.</returns>
		public AppletManifestInfo GetApplet(string appletId)
		{
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			var appletDirectory = Path.Combine(baseDirectory, "applets");

			var files = Directory.GetFiles(appletDirectory).Select(Path.GetFileName);

			var file = files.FirstOrDefault(f => f.StartsWith(appletId));

			var applet = new AppletManifestInfo();

			using (var stream = new FileStream(Path.Combine(appletDirectory, file), FileMode.Open))
			{
				AppletPackage package = null;

				if (file.EndsWith(".pak") || file.EndsWith(".pak.gz"))
				{
					using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
					{
						var serializer = new XmlSerializer(typeof(AppletPackage));
						package = (AppletPackage)serializer.Deserialize(gzipStream);
					}
				}

				using (var memoryStream = new MemoryStream(package.Manifest))
				{
					applet = new AppletManifestInfo(AppletManifest.Load(memoryStream));
					applet.FileExtension = file.EndsWith(".pak.gz") ? ".pak.gz" : ".pak";
				}
			}

			return applet;
		}

		/// <summary>
		/// Gets a list of applets for a specific query.
		/// </summary>
		/// <returns>Returns a list of applet which match the specific query.</returns>
		public AmiCollection<AppletManifestInfo> GetApplets()
		{
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			var appletDirectory = Path.Combine(baseDirectory, "applets");

			var files = Directory.GetFiles(appletDirectory);

			var applets = new AmiCollection<AppletManifestInfo>();

			foreach (var file in from file in files let bytes = File.ReadAllBytes(file) select file)
			{
				using (var stream = new FileStream(file, FileMode.Open))
				{
					AppletPackage package = null;

					if (file.EndsWith(".pak") || file.EndsWith(".pak.gz"))
					{
						using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress))
						{
							var serializer = new XmlSerializer(typeof(AppletPackage));
							package = (AppletPackage)serializer.Deserialize(gzipStream);
						}
					}

					using (var memoryStream = new MemoryStream(package.Manifest))
					{
						applets.CollectionItem.Add(new AppletManifestInfo(AppletManifest.Load(memoryStream)));
					}
				}
			}

			return applets;
		}

		/// <summary>
		/// Updates an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be updated.</param>
		/// <param name="appletManifestInfo">The applet containing the updated information.</param>
		public AppletManifestInfo UpdateApplet(string appletId, AppletManifestInfo appletManifestInfo)
		{
			if (appletId != appletManifestInfo.AppletManifest.Info.Id)
			{
				throw new ArgumentException($"Unable to update applet using id {appletId} and {appletManifestInfo.AppletManifest.Info.Id}");
			}
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			var appletDirectory = Path.Combine(baseDirectory, "applets");

			var file = Directory.GetFiles(appletDirectory).FirstOrDefault(f => f.StartsWith(appletId));

			if (file == null)
			{
				throw new ArgumentException("Applet not found");
			}

			File.Delete(file);

			using (var fileStream = File.Create(Path.Combine(appletDirectory, appletManifestInfo.AppletManifest.Info.Id)))
			{
				var buffer = new byte[fileStream.Length];

				fileStream.Write(buffer, 0, buffer.Length);

				fileStream.Close();
			}

			return appletManifestInfo;
		}
	}
}