using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

    }
}
