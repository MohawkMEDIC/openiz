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
 * Date: 2016-11-15
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.BusinessRules.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenIZ.BusinessRules.JavaScript;
using OpenIZ.Core;
using OpenIZ.Core.Applets;
using OpenIZ.Core.Applets.Model;

namespace OpenIZ.BusinessRules.Core
{
	/// <summary>
	/// Represents a business rules daemon service.
	/// </summary>
	public class BusinessRulesHandler : IDaemonService
	{
		/// <summary>
		/// The internal reference to the trace source.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.BusinessRules.Core");

		/// <summary>
		/// The internal reference to the configuration.
		/// </summary>
		private readonly BusinessRulesCoreConfiguration configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.businessrules.core") as BusinessRulesCoreConfiguration;

		/// <summary>
		/// Gets the running state of the message handler.
		/// </summary>
		public bool IsRunning => true;

		/// <summary>
		/// Fired when the object is starting up.
		/// </summary>
		public event EventHandler Started;

		/// <summary>
		/// Fired when the object is starting.
		/// </summary>
		public event EventHandler Starting;

		/// <summary>
		/// Fired when the service has stopped.
		/// </summary>
		public event EventHandler Stopped;

		/// <summary>
		/// Fired when the service is stopping.
		/// </summary>
		public event EventHandler Stopping;

		/// <summary>
		/// Starts the service. Returns true if the service started successfully.
		/// </summary>
		/// <returns>Returns true if the service started successfully.</returns>
		public bool Start()
		{
			try
			{
				this.Starting?.Invoke(this, EventArgs.Empty);

				var applets = new AppletCollection();

				foreach (var file in Directory.GetFiles(this.configuration.DirectoryConfiguration.Path, "*.*", SearchOption.TopDirectoryOnly).Where(f => this.configuration.DirectoryConfiguration.SupportedExtensions.Contains(Path.GetExtension(f))))
				{
					this.tracer.TraceEvent(TraceEventType.Information, 0, "Adding file {0}", file);

					using (var stream = new FileStream(file, FileMode.Open))
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
							applets.Add(AppletManifest.Load(memoryStream));
						}
					}
				}

				ApplicationServiceContext.Current = ApplicationContext.Current;

				foreach (var applet in applets.SelectMany(a => a.Assets).Where(a => a.Name.StartsWith("rules/")))
				{
					using (var reader = new StreamReader(new MemoryStream(applets.RenderAssetContent(applet))))
					{
						JavascriptBusinessRulesEngine.Current.AddRules(reader);
						this.tracer.TraceEvent(TraceEventType.Information, 0, "Added rules from {0}", applet.Name);
					}
				}

				this.Started?.Invoke(this, EventArgs.Empty);
				return true;
			}
			catch (Exception e)
			{
				this.tracer.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
				return false;
			}
		}

		/// <summary>
		/// Stops the service. Returns true if the service stopped successfully.
		/// </summary>
		/// <returns>Returns true if the service stopped successfully.</returns>
		public bool Stop()
		{
			this.Stopping?.Invoke(this, EventArgs.Empty);

			this.Stopped?.Invoke(this, EventArgs.Empty);

			return true;
		}
	}
}
