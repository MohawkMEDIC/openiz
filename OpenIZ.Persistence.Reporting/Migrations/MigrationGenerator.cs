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

using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;

namespace OpenIZ.Persistence.Reporting.Migrations
{
	/// <summary>
	/// Represents an SQL migration generator for SQL Server.
	/// </summary>
	internal sealed class MigrationGenerator : SqlServerMigrationSqlGenerator
	{
		/// <summary>
		/// Generates SQL for the add column operation.
		/// </summary>
		/// <param name="addColumnOperation">The add column operation.</param>
		protected override void Generate(AddColumnOperation addColumnOperation)
		{
			SetCreationTimeColumn(addColumnOperation.Column);

			base.Generate(addColumnOperation);
		}

		/// <summary>
		/// Generates SQL for the create table operation.
		/// </summary>
		/// <param name="createTableOperation">The create table operation.</param>
		protected override void Generate(CreateTableOperation createTableOperation)
		{
			SetCreationTimeColumn(createTableOperation.Columns);

			base.Generate(createTableOperation);
		}

		/// <summary>
		/// Sets the "CreationTime" column.
		/// </summary>
		/// <param name="columns">The list of column models.</param>
		private static void SetCreationTimeColumn(IEnumerable<ColumnModel> columns)
		{
			foreach (var columnModel in columns)
			{
				SetCreationTimeColumn(columnModel);
			}
		}

		/// <summary>
		/// Sets the "CreationTime" column's default SQL value.
		/// </summary>
		/// <param name="column">The column.</param>
		private static void SetCreationTimeColumn(PropertyModel column)
		{
			if (column.Name == "CreationTime")
			{
				column.DefaultValueSql = "GETUTCDATE()";
			}
		}
	}
}