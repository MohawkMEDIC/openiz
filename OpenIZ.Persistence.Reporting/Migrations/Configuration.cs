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
 * Date: 2017-1-9
 */

using System.Collections.Generic;
using OpenIZ.Persistence.Reporting.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using OpenIZ.Persistence.Reporting.Context;

namespace OpenIZ.Persistence.Reporting.Migrations
{
	/// <summary>
	/// Represents internal configuration for the database migrations.
	/// </summary>
	internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Configuration"/> class.
		/// </summary>
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			SetSqlGenerator("System.Data.SqlClient", new MigrationGenerator());
		}

		/// <summary>
		/// Seeds the database with pre-set data.
		/// </summary>
		/// <param name="context">The database context.</param>
		protected override void Seed(ApplicationDbContext context)
		{
			//context.ReportDefinitions.Add(new ReportDefinition
			//{
			//	Author = "nityan",
			//	Description = "test description",
			//	CorrelationId = Guid.NewGuid(),
			//	Name = "test report",
			//	Parameters = new List<ReportParameter>
			//	{
			//		new ReportParameter
			//		{
			//			ParameterTypeId = Guid.Parse("6CDE9F0D-1DA4-462F-8C41-163969D4E575"),
			//			IsNullable = false,
			//			Order = 0,
			//			Name = "test parameter",
			//			DefaultValues = new List<ParameterValue>
			//			{
			//				new ParameterValue(Guid.NewGuid())
			//			}
			//		}
			//	}
			//});

			//context.SaveChanges();
		}
	}
}
