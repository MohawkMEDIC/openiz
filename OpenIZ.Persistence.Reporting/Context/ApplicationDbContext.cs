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
 * Date: 2017-1-7
 */

using System;
using System.Configuration;
using OpenIZ.Persistence.Reporting.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;

namespace OpenIZ.Persistence.Reporting.Context
{
	/// <summary>
	/// Represents the application database context.
	/// </summary>
	public class ApplicationDbContext : DbContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
		/// </summary>
		public ApplicationDbContext() : base(ReportingService.Configuration.ConnectionStringName)
		{
			this.Configuration.LazyLoadingEnabled = false;
		}

		/// <summary>
		/// Gets or sets the data types.
		/// </summary>
		public DbSet<DataType> DataTypes { get; set; }

		/// <summary>
		/// Gets or sets the parameter values.
		/// </summary>
		public DbSet<ParameterValue> ParameterValues { get; set; }

		/// <summary>
		/// Gets or sets the report definitions.
		/// </summary>
		public DbSet<ReportDefinition> ReportDefinitions { get; set; }

		/// <summary>
		/// Gets or sets the report parameters.
		/// </summary>
		public DbSet<ReportParameter> ReportParameters { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="ApplicationDbContext"/> instance.
		/// </summary>
		/// <returns>Returns a new instance of the application database context.</returns>
		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		/// <summary>
		/// Runs setup and configuration when the model is being created.
		/// </summary>
		/// <param name="modelBuilder">The model builder used to create and run the configuration.</param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			base.OnModelCreating(modelBuilder);
		}
	}
}