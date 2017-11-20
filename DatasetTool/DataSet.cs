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
 * User: fyfej
 * Date: 2017-7-20
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Persistence;
using OpenIZ.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;

namespace OizDevTool
{
	/// <summary>
	/// Represents tooling for interacting with datasets
	/// </summary>
	[Description("Tooling for interacting with OpenIZ dataset files")]
	public static class Dataset
	{
		/// <summary>
		/// Extracts data from the dataset file
		/// </summary>
		/// <param name="args"></param>
		[Description("Extracts data from the database into a dataset file")]
		[Example("Install custom concept dictionary", "--dataset=concept-dictionary-backup.dataset --type=Concept --includeRelated")]
		[ParameterClass(typeof(InstallParameters))]
		public static void Extract(string[] args)
		{
			var parms = new ParameterParser<ExtractionParameters>().Parse(args);

			// Is there any type specified, or no
			List<Type> targetTypes = new List<Type>();
			if (parms.Type.Count == 0) // All types
				targetTypes = typeof(DataInstallAction).GetRuntimeProperty("Element").GetCustomAttributes<XmlElementAttribute>().Select(o => o.Type).ToList();
			else
				targetTypes = typeof(DataInstallAction).GetRuntimeProperty("Element").GetCustomAttributes<XmlElementAttribute>().Where(o => parms.Type.Contains(o.ElementName)).Select(o => o.Type).ToList();

			// Now time to get the DPE and extract
			IEnumerable<IdentifiedData> objects = new List<IdentifiedData>();

			foreach (var itm in targetTypes)
			{
				Console.Write("Extracting {0}.", itm.Name);
				try
				{
					// Load persister
					var idt = typeof(IDataPersistenceService<>).MakeGenericType(itm);
					var idp = ApplicationContext.Current.GetService(idt) as IDataPersistenceService;
					if (idp == null)
						throw new InvalidOperationException("Cannot find data persistence service");

					// Convert query
					var mi = typeof(QueryExpressionParser).GetGenericMethod("BuildLinqExpression", new Type[] { itm }, new Type[] { typeof(OpenIZ.Core.Model.Query.NameValueCollection) });
					var qd = mi.Invoke(null, new object[] { OpenIZ.Core.Model.Query.NameValueCollection.ParseQueryString(parms.Filter) }) as Expression;

					// Exec query
					int tr = 1, ofs = 0;
					while (ofs < tr)
					{
						ofs += 100;
						(objects as List<IdentifiedData>).AddRange(idp.Query(qd, ofs, 100, out tr).OfType<IdentifiedData>());
						Console.Write(".");
					}

					Console.WriteLine("ok");
				}
				catch (Exception ex)
				{
					Trace.TraceError(ex.ToString());
					Console.WriteLine("fail");
				}
			}

			// Now we want to organize the file properly
			Console.WriteLine("Preliminary remove duplicates...");
			objects = objects.Distinct(new IdentifiedEntityComparer());

			// Now we flatten the objects and re-organize them
			DatasetInstall dsOutput = new DatasetInstall();
			Console.Write("Flattening objects: ( 0 / {0} )", objects.Count());
			int i = 0;
			foreach (var obj in objects)
			{
				Console.CursorLeft = 0;
				Console.Write("Flattening objects: ( {0} / {1} )", i++, objects.Count());

				if (!parms.IncludeRelated)
					dsOutput.Action.Add(new DataUpdate()
					{
						Element = obj,
						InsertIfNotExists = true
					});
				else
					LayoutObject(obj, dsOutput);
			}

			// Now we want to organize the file properly
			Console.WriteLine("Secondary remove duplicates...");
			objects = objects.Distinct(new IdentifiedEntityComparer());
		}

		/// <summary>
		/// Installs the specified dataset into the IMS database
		/// </summary>
		/// <param name="args"></param>
		[Description("Installs the specified dataset")]
		[Example("Install custom concept dictionary", "--dataset=my-dictionary.dataset")]
		[ParameterClass(typeof(InstallParameters))]
		public static void Install(string[] args)
		{
			// Load the file
			var parms = new ParameterParser<InstallParameters>().Parse(args);
            ApplicationContext.Current.Start();
			var dsi = new DataInitializationService();
			var ds = DatasetInstall.Load(parms.DatasetFile);
			Console.WriteLine("Will install dataset {0} ({1} objects)...", ds.Action.Count, ds.Id);

            dsi.ProgressChanged += (o, e) =>
            {
                Console.CursorLeft = 4;
                Console.Write("{0} ({1:0%})", e.State, e.Progress);
            };

			dsi.InstallDataset(ds);
		}

        /// <summary>
		/// Installs the specified dataset into the IMS database
		/// </summary>
		/// <param name="args"></param>
		[Description("Shows statistics from the specified dataset")]
        [Example("Count statistics from dataset", "--dataset=my-dictionary.dataset")]
        [ParameterClass(typeof(InstallParameters))]
        public static void Stats(string[] args)
        {
            // Load the file
            var parms = new ParameterParser<InstallParameters>().Parse(args);
            ApplicationContext.Current.Start();
            var ds = DatasetInstall.Load(parms.DatasetFile);
            Console.WriteLine("Statistics for {0} ({1} objects)...", ds.Action.Count, ds.Id);
            foreach(var gc in ds.Action?.GroupBy(o=>o.Element?.GetType()))
            {
                Console.WriteLine("{0} - {1} items", gc.Key, gc.Count());
            }
        }

        /// <summary>
        /// Will layout the object in a referentially proper way
        /// </summary>
        private static void LayoutObject(IdentifiedData obj, DatasetInstall dsOutput, bool insert = false)
		{
			// Add myself
			if (!insert)
				dsOutput.Action.Add(new DataUpdate()
				{
					Element = obj,
					InsertIfNotExists = true
				});
			else
				dsOutput.Action.Insert(0, new DataUpdate()
				{
					Element = obj,
					InsertIfNotExists = true
				});

			// Iterate properties
			foreach (var prop in obj.GetType().GetRuntimeProperties())
			{
				var sra = prop.GetCustomAttribute<SerializationReferenceAttribute>();
				var value = prop.GetValue(obj);

				// Value is list
				if (value is IList && prop.GetCustomAttribute<XmlElementAttribute>() != null)
				{
					foreach (var v in (value as IList))
						LayoutObject(v as IdentifiedData, dsOutput, true);
				}
				else if (value == null && sra != null) // No XmlElement .. hmm we might have to insert this stuff
					value = OpenIZ.Core.Model.ExtensionMethods.LoadProperty(obj, prop.Name);

				if (value is IdentifiedData)
					dsOutput.Action.Insert(0, new DataUpdate()
					{
						Element = value as IdentifiedData,
						InsertIfNotExists = true
					});
			}
		}

		/// <summary>
		/// Parameters which will extract data from the database into dataset form
		/// </summary>
		public class ExtractionParameters
		{
			/// <summary>
			/// Gets or sets the dataset output file
			/// </summary>
			[Description("The dataset file to be created")]
			[Parameter("dataset")]
			public String DatasetFile { get; set; }

			/// <summary>
			/// Filter of extraction
			/// </summary>
			[Parameter("filter")]
			[Description("The filter to be placed on data")]
			public String Filter { get; set; }

			/// <summary>
			/// When true include related items
			/// </summary>
			[Parameter("includeRelated")]
			[Description("When set includes the related objects")]
			public bool IncludeRelated { get; set; }

			/// <summary>
			/// Gets or sets the type of data
			/// </summary>
			[Description("The types of data to extract")]
			[Parameter("type")]
			public StringCollection Type { get; set; }
		}

		/// <summary>
		/// Installation parameters
		/// </summary>
		public class InstallParameters
		{
			/// <summary>
			/// Installes the specified dataset file
			/// </summary>
			[Description("The dataset file to install")]
			[Parameter("dataset")]
			public String DatasetFile { get; set; }
		}

		/// <summary>
		/// Identified entity comparer
		/// </summary>
		private class IdentifiedEntityComparer : IEqualityComparer<IdentifiedData>
		{
			public bool Equals(IdentifiedData x, IdentifiedData y)
			{
				return x.Key == y.Key;
			}

			public int GetHashCode(IdentifiedData obj)
			{
				return obj.Key.GetHashCode();
			}
		}
	}
}