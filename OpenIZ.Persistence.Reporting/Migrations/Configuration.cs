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
using Npgsql;
using OpenIZ.Persistence.Reporting.Model;

namespace OpenIZ.Persistence.Reporting.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Data.Entity.Migrations.Model;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<OpenIZ.Persistence.Reporting.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
	        SetSqlGenerator("Npgsql", new CustomSqlGenerator());
        }

        protected override void Seed(OpenIZ.Persistence.Reporting.ApplicationDbContext context)
        {
	    //    context.ReportDefinitions.Add(new Model.ReportDefinition
	    //    {
		   //     Author = "nityan",
		   //     Description = "test report definition",
		   //     Parameters = new List<ReportParameter>
		   //     {
					//new ReportParameter
					//{
												
					//}
		   //     }
	    //    });

	    //    context.SaveChanges();
        }
    }

	internal sealed class CustomSqlGenerator : NpgsqlMigrationSqlGenerator
	{
		protected override void Convert(AddColumnOperation addColumnOperation)
		{
			if (addColumnOperation.Column.Name == "creation_time")
			{
				addColumnOperation.Column.DefaultValue = DateTimeOffset.UtcNow;
			}

			if (addColumnOperation.Column.Name == "id")
			{
				addColumnOperation.Column.DefaultValue = Guid.NewGuid();
			}

			base.Convert(addColumnOperation);
		}
	}
}
