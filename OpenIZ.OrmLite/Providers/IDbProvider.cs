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
 * User: justi
 * Date: 2017-1-21
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Data.Warehouse;

namespace OpenIZ.OrmLite.Providers
{
    /// <summary>
    /// Data provider
    /// </summary>
    public interface IDbProvider
    {

        
        /// <summary>
        /// Gets the features of the database back-end
        /// </summary>
        SqlEngineFeatures Features { get; }
        /// <summary>
        /// Trace SQL commands
        /// </summary>
        bool TraceSql { get; set; }
        
        /// <summary>
        /// Readonly (mirror) connection string
        /// </summary>
        String ReadonlyConnectionString { get; set; }

        /// <summary>
        /// Read/write connection string
        /// </summary>
        String ConnectionString { get; set; }

        /// <summary>
        /// Get name of the provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Create SQL keyword
        /// </summary>
        String CreateSqlKeyword(SqlKeyword keywordType);

        /// <summary>
        /// Retrieves a readonly connection
        /// </summary>
        DataContext GetReadonlyConnection();

        /// <summary>
        /// Retrieves a read/writer connection
        /// </summary>
        DataContext GetWriteConnection();

        /// <summary>
        /// Get connection to a specified connection string
        /// </summary>
        DataContext CloneConnection(DataContext source);

        /// <summary>
        /// Creates a command on the specified transaction
        /// </summary>
        IDbCommand CreateCommand(DataContext context, SqlStatement stmt);

        /// <summary>
        /// Creates a stored procedure call command
        /// </summary>
        IDbCommand CreateStoredProcedureCommand(DataContext context, String spName, params object[] parms);

        /// <summary>
        /// Create command with specified text and parameters
        /// </summary>
        IDbCommand CreateCommand(DataContext context, String sql, params object[] parms);

        /// <summary>
        /// Creates an Exists statement
        /// </summary>
        SqlStatement Count(SqlStatement sqlStatement);

        /// <summary>
        /// Creates an Exists statement
        /// </summary>
        SqlStatement Exists(SqlStatement sqlStatement);

        /// <summary>
        /// Appends a RETURNING statement
        /// </summary>
        SqlStatement Returning(SqlStatement sqlStatement, params ColumnMapping[] returnColumns);
        

        /// <summary>
        /// Get a lock for the database
        /// </summary>
        Object Lock(IDbConnection connection);

        /// <summary>
        /// Convert value
        /// </summary>
        Object ConvertValue(Object value, Type toType);

        /// <summary>
        /// Map datatype
        /// </summary>
        string MapDatatype(SchemaPropertyType type);
    }
}
