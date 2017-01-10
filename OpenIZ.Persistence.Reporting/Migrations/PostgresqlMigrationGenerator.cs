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
 * Date: 2017-1-9
 */
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Reporting.Migrations
{
	/// <summary>
	/// Represents a SQL migration generator for Postgresql.
	/// </summary>
	internal sealed class PostgresqlMigrationGenerator : NpgsqlMigrationSqlGenerator
	{
		/// <summary>
		/// Converts an add column operation.
		/// </summary>
		/// <param name="addColumnOperation">The add column operation to convert.</param>
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
