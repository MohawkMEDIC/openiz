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
 * Date: 2017-2-17
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenIZ.Core.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Data;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Diagnostics;
using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Warehouse.ADO.Configuration;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.OrmLite;
using OpenIZ.Core.Model.Query;
using Newtonsoft.Json;
using OpenIZ.Warehouse.ADO.Data.Model;
using OpenIZ.Core.Security.Attribute;
using System.Security.Permissions;
using OpenIZ.Core.Security;

namespace OpenIZ.Warehouse.ADO
{
    /// <summary>
    /// Represents a simple ADO ad-hoc data warehouse
    /// </summary>
    public class ADODataWarehouse : IAdHocDatawarehouseService
    {

        private AdoConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(DataWarehouseConstants.ConfigurationSectionName) as AdoConfiguration;

        // Disposed
        private bool m_disposed = false;

        // Tracer
        private TraceSource m_tracer = new TraceSource(DataWarehouseConstants.TraceSourceName);

        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;

        /// <summary>
        /// Gets the name of the data provider so callers can determine how to create stored queries
        /// </summary>
        public string DataProvider
        {
            get
            {

                return "ado";
            }
        }

        /// <summary>
        /// True if this is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Constructs the SQLite data warehouse file
        /// </summary>
        public ADODataWarehouse()
        {
        }

        /// <summary>
        /// Throw an exception if the object is disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.m_disposed)
                throw new ObjectDisposedException(nameof(ADODataWarehouse));
        }

        /// <summary>
        /// Add an object to the data
        /// </summary>
        /// <param name="datamartId"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteWarehouseData)]
        public Guid Add(Guid datamartId, dynamic obj)
        {
            this.ThrowIfDisposed();
            using (var context = this.m_configuration.Provider.GetWriteConnection())
            {

                try
                {
                    context.Open();
                    context.BeginTransaction();
                    // Get the datamart
                    var mart = this.GetDatamart(context, datamartId);

                    Guid retVal = Guid.Empty;

                    if (obj is IEnumerable<dynamic>)
                        foreach (var itm in obj as IEnumerable<dynamic>)
                            retVal = this.InsertObject(context, mart.Schema.Name, mart.Schema, itm);
                    else
                        retVal = this.InsertObject(context, mart.Schema.Name, mart.Schema, obj);

                    context.Transaction.Commit();

                    return retVal;
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error while adding data to {0}: {1}", datamartId, e);
                    context.Transaction.Commit();
                    throw;
                }
            }
        }

        /// <summary>
        /// Insert specified object
        /// </summary>
        private Guid InsertObject(DataContext context, String path, IDatamartSchemaPropertyContainer pcontainer, dynamic obj, Guid? scope = null)
        {
            // Conver to expando
            IDictionary<String, Object> tuple = new ExpandoObject();

            if (obj is IDictionary<String, object>)
                foreach (var kv in obj as IDictionary<String, object>)
                    tuple.Add(kv.Key, kv.Value);
            else
                foreach (var pi in obj.GetType().GetProperties())
                    tuple.Add(pi.Name, pi.GetValue(obj, null));

            tuple.Add("uuid", Guid.NewGuid());
            //tuple.Add("cont_id", scope);
            //tuple.Add("ext_time", DateTime.Now);

            // Now time to store
            SqlStatement sbQuery = context.CreateSqlStatement("INSERT INTO "),
                sbValues = context.CreateSqlStatement();
            sbQuery.Append(path);
            sbQuery.Append("(");
            foreach (var p in tuple.Where(o => pcontainer.Properties.FirstOrDefault(p => p.Name == o.Key)?.Type != SchemaPropertyType.Object))
            {
                sbQuery.Append(p.Key);
                sbValues.Append("?", p.Value);
                if (p.Key != tuple.Last().Key)
                {
                    sbQuery.Append(",");
                    sbValues.Append(",");
                }
            }

            sbQuery = sbQuery.Append(") VALUES (").Append(sbValues).Append(")");
            context.ExecuteNonQuery(sbQuery);

            // Sub-properties
            foreach (var p in pcontainer.Properties.Where(o => o.Type == SchemaPropertyType.Object))
                this.InsertObject(context, String.Format("{0}_{1}", path, p.Name), p, obj[p.Name], (Guid)tuple["uuid"]);

            return (Guid)tuple["uuid"];
        }

        /// <summary>
        /// Query against the specified object
        /// </summary>
        public IEnumerable<dynamic> AdhocQuery(Guid datamartId, dynamic queryParameters)
        {
            this.ThrowIfDisposed();
            int tr = 0;
            return this.AdhocQuery(datamartId, queryParameters, 0, 100, out tr);
        }

        /// <summary>
        /// Perform an ad-hoc query
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.QueryWarehouseData)]
        public IEnumerable<dynamic> AdhocQuery(Guid datamartId, dynamic queryParameters, int offset, int count, out int totalResults)
        {
            this.ThrowIfDisposed();
            using (var context = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    context.Open();

                    var mart = this.GetDatamart(context, datamartId);
                    if (mart == null)
                        throw new KeyNotFoundException(datamartId.ToString());

                    // Query paremeters
                    // Query paremeters
                    IDictionary<String, Object> parms = queryParameters as ExpandoObject;
                    if (queryParameters is String)
                        queryParameters = NameValueCollection.ParseQueryString(queryParameters as String);
                    if (queryParameters is NameValueCollection)
                    {
                        parms = new ExpandoObject();
                        foreach (var itm in (queryParameters as NameValueCollection))
                            parms.Add(itm.Key, itm.Value);
                    }
                    else
                    {
                        parms = new ExpandoObject();
                        foreach (var itm in queryParameters.GetType().GetProperties())
                            parms.Add(itm.Name, itm.GetValue(queryParameters, null));
                    }

                    return this.QueryInternal(context, mart.Schema.Name, mart.Schema.Properties, parms, offset, count, out totalResults);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error executing {0} : {1}", datamartId, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Create a datamart
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerWarehouse)]
        public DatamartDefinition CreateDatamart(string name, object schema)
        {
            this.ThrowIfDisposed();

            DatamartSchema dmSchema = schema as DatamartSchema;
            if (schema is ExpandoObject)
                dmSchema = JsonConvert.DeserializeObject<DatamartSchema>(JsonConvert.SerializeObject(schema));

            // Now create / register the data schema
            using (var context = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    context.Open();
                    context.BeginTransaction();
                    this.m_tracer.TraceInfo("Creating datamart {0}", name);

                    // Register the schema
                    dmSchema.Id = dmSchema.Id == default(Guid) ? Guid.NewGuid() : dmSchema.Id;
                    context.Insert(new AdhocSchema()
                    {
                        Name = name,
                        SchemaId = dmSchema.Id
                    });

                    this.CreateProperties(context, name, dmSchema, null);

                    // Create mart
                    var retVal = new DatamartDefinition()
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        CreationTime = DateTime.Now,
                        Schema = dmSchema
                    };

                    context.Insert(new AdhocDatamart()
                    {
                        DatamartId = retVal.Id,
                        Name = name,
                        CreationTime = DateTime.Now,
                        SchemaId = dmSchema.Id
                    });

                    foreach (var itm in dmSchema.Queries)
                        this.CreateStoredQueryInternal(context, retVal.Id, itm);

                    context.Transaction.Commit();
                    return retVal;
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error creating data mart {0} : {1}", name, e);
                    context.Transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Create properties for the specified container
        /// </summary>
        private void CreateProperties(DataContext context, String pathName, DatamartSchema schema, IDatamartSchemaPropertyContainer container)
        {
            List<SqlStatement> indexes = new List<SqlStatement>();

            // Create the property container table
            SqlStatement createSql = context.CreateSqlStatement("CREATE TABLE ")
                .Append(pathName)
                .Append("(")
                .Append("uuid UUID NOT NULL PRIMARY KEY DEFAULT uuid_generate_v4(),");

            if (container is DatamartSchemaProperty)
                createSql = createSql.Append("entity_uuid UUID NOT NULL, ");

            // Create the specified dm_<<name>>_table
            foreach (var itm in (container ?? schema).Properties)
            {
                itm.Id = itm.Id == default(Guid) ? Guid.NewGuid() : itm.Id;

                // Insert
                context.Insert(new AdhocProperty()
                {
                    Attributes = (int)itm.Attributes,
                    TypeId = (int)itm.Type,
                    ContainerId = (container as DatamartSchemaProperty)?.Id,
                    SchemaId = schema.Id,
                    Name = itm.Name,
                    PropertyId = itm.Id
                });

                if (itm.Properties?.Count > 0)
                    this.CreateProperties(context, String.Format("{0}_{1}", pathName, itm.Name), schema, itm);

                // get datatype
                var typeString = context.GetDataType(itm.Type); ;

                // Add property to the table
                if (!String.IsNullOrEmpty(typeString))
                {
                    String attributeString = "";
                    if (itm.Attributes.HasFlag(SchemaPropertyAttributes.NotNull))
                        attributeString += "NOT NULL ";
                    if (itm.Attributes.HasFlag(SchemaPropertyAttributes.Unique))
                        attributeString += "UNIQUE ";
                    createSql = createSql.Append($"{itm.Name} {typeString} {attributeString}");

                    // HACK:
                    createSql.Append(",");
                    if (itm.Attributes.HasFlag(SchemaPropertyAttributes.Indexed))
                        indexes.Add(context.CreateSqlStatement($"CREATE INDEX {pathName}_{itm.Name}_idx ON {pathName}({itm.Name})"));
                }
            }

            createSql.RemoveLast();
            createSql = createSql.Append(")");

            // Now execute SQL create statement
            if (!(container is DatamartStoredQuery))
            {
                context.ExecuteNonQuery(createSql);
                foreach (var idx in indexes)
                    context.ExecuteNonQuery(idx);
            }
        }

        /// <summary>
        /// Delete the specified tuple from the datamart object
        /// </summary>
        /// <param name="datamartId"></param>
        /// <param name="tupleId"></param>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.DeleteWarehouseData)]
        public void Delete(Guid datamartId, dynamic deleteQuery)
        {

            this.ThrowIfDisposed();

            using (var context = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    context.Open();
                    context.BeginTransaction();
                    var mart = this.GetDatamart(context, datamartId);
                    if (mart == null)
                        throw new KeyNotFoundException(datamartId.ToString());

                    // Query paremeters
                    IDictionary<String, Object> parms = null;
                    if (deleteQuery is String)
                        deleteQuery = NameValueCollection.ParseQueryString(deleteQuery as String);
                    if (deleteQuery is NameValueCollection)
                    {
                        parms = new ExpandoObject();
                        foreach (var itm in (deleteQuery as NameValueCollection))
                            parms.Add(itm.Key, itm.Value);
                    }
                    else
                    {
                        parms = new ExpandoObject();
                        foreach (var itm in deleteQuery.GetType().GetProperties())
                            parms.Add(itm.Name, itm.GetValue(deleteQuery, null));
                    }

                    // First, we delete the record
                    var sql = this.ParseFilterDictionary(context, mart.Schema.Name, parms, mart.Schema.Properties).Build();
                    var delSql = context.CreateSqlStatement($"DELETE FROM {mart.Schema.Name} WHERE {sql.Build().SQL} ", sql.Arguments.ToArray());
                    context.ExecuteNonQuery(delSql);
                    this.DeleteProperties(context, mart.Schema.Name, mart.Schema);

                    context.Transaction.Commit();
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error deleting {0} : {1}", datamartId, e);
                    context.Transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete property values
        /// </summary>
        private void DeleteProperties(DataContext context, String path, IDatamartSchemaPropertyContainer container)
        {
            foreach (var p in container.Properties.Where(o => o.Type == SchemaPropertyType.Object))
            {
                context.ExecuteNonQuery(context.CreateSqlStatement($"DELETE FROM {path}_{p.Name} WHERE entity_uuid NOT IN (SELECT uuid FROM {path})"));
                this.DeleteProperties(context, String.Format("{0}_{1}", path, p.Name), p);
            }
        }
        
        /// <summary>
        /// Delete the datamart
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerWarehouse)]
        public void DeleteDatamart(Guid datamartId)
        {
            this.ThrowIfDisposed();

            using(var context = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    context.Open();
                    using (var tx = context.BeginTransaction())
                    {
                        var schema = this.GetDatamart(datamartId);
                        context.Delete<AdhocQuery>(o => o.SchemaId == schema.Id);
                        context.Delete<AdhocProperty>(o => o.SchemaId == schema.Id);
                        context.Delete<AdhocSchema>(o => o.SchemaId == schema.Id);
                        context.Delete<AdhocDatamart>(o => o.DatamartId == datamartId);
                        context.ExecuteNonQuery(context.CreateSqlStatement($"DROP TABLE {schema.Name};"));
                        tx.Commit();
                    }
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error deleting datamart: {0}", e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get the specified tuple id from the specified datamart
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadWarehouseData)]
        public dynamic Get(Guid datamartId, Guid tupleId)
        {
            this.ThrowIfDisposed();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the specified data mart
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public DatamartDefinition GetDatamart(String name)
        {

            using (var context = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    context.Open();

                    var dbMart = context.FirstOrDefault<AdhocDatamart>(o => o.Name == name);
                    if (dbMart != null)
                        return new DatamartDefinition()
                        {
                            Id = dbMart.DatamartId,
                            Name = dbMart.Name,
                            CreationTime = dbMart.CreationTime.DateTime,
                            Schema = this.LoadSchema(context, dbMart.SchemaId)
                        };
                    else return null;
                }

                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error getting datamart {0} : {1}", name, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get the specified data mart
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public DatamartDefinition GetDatamart(Guid id)
        {

            using (var context = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    context.Open();

                    var dbMart = context.FirstOrDefault<AdhocDatamart>(o => o.DatamartId == id);      
                    if (dbMart != null)
                        return new DatamartDefinition()
                        {
                            Id = dbMart.DatamartId,
                            Name = dbMart.Name,
                            CreationTime = dbMart.CreationTime.DateTime,
                            Schema = this.LoadSchema(context, dbMart.SchemaId)
                        };
                    else return null;
                }

                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error getting datamart {0} : {1}", id, e);
                    throw;
                }
            }
        }


        /// <summary>
        /// Get data marts
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public List<DatamartDefinition> GetDatamarts()
        {
            this.ThrowIfDisposed();

            using (var context = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {

                    context.Open();
                    return context.Query<AdhocDatamart>(o => o.Name != null).Select(o => new DatamartDefinition()
                    {
                        Id = o.DatamartId,
                        Name = o.Name,
                        CreationTime = o.CreationTime.DateTime,
                        Schema = this.LoadSchema(context, o.SchemaId)
                    }).ToList();
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error getting datamarts: {0}", e);
                    throw;
                }
            }

        }

        /// <summary>
        /// Execute a stored query
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.QueryWarehouseData)]
        public IEnumerable<dynamic> StoredQuery(Guid datamartId, string queryId, dynamic queryParameters)
        {
            this.ThrowIfDisposed();


            using (var context = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    context.Open();
                    // Query paremeters
                    var mart = this.GetDatamart(context, datamartId);

                    IDictionary<String, Object> parms = queryParameters as ExpandoObject;
                    if (queryParameters is String)
                        queryParameters = NameValueCollection.ParseQueryString(queryParameters as String);
                    if (queryParameters is NameValueCollection)
                    {
                        parms = new ExpandoObject();
                        foreach (var itm in (queryParameters as NameValueCollection))
                            parms.Add(itm.Key, itm.Value);
                    }
                    else
                    {
                        parms = new ExpandoObject();
                        foreach (var itm in queryParameters.GetType().GetProperties())
                            parms.Add(itm.Name, itm.GetValue(queryParameters, null));
                    }

                    var queryDefn = mart.Schema.Queries.FirstOrDefault(m => m.Name == queryId);

                    int tr = 0;
                    return this.QueryInternal(context, String.Format("sqp_{0}_{1}", mart.Schema.Name, queryId), queryDefn.Properties, parms, 0, 0, out tr);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error executing stored query {0}.{1} : {2}", datamartId, queryId, e);
                    throw;
                }
            }


        }

        /// <summary>
        /// Query the specified object with the specified parameters
        /// </summary>
        private IEnumerable<dynamic> QueryInternal(DataContext context, string objectName, List<DatamartSchemaProperty> properties, IDictionary<string, object> parms, int offset, int count, out int totalResults)
        {
            // Build a query
            var qry = context.CreateSqlStatement($"SELECT DISTINCT * FROM {objectName} ")
                .Where(this.ParseFilterDictionary(context, objectName, parms, properties));

            // Construct the result set
            List<dynamic> retVal = new List<dynamic>();

            totalResults = context.Count(qry);

            if (count > 0)
                qry = qry.Limit(count);
            if (offset > 0)
                qry = qry.Offset(offset);

            return context.Query<ExpandoObject>(qry);
        }

        /// <summary>
        /// Parses a filter dictionary and creates the necessary SQL
        /// </summary>
        private SqlStatement ParseFilterDictionary(DataContext context, String objectName, IDictionary<string, object> parms, List<DatamartSchemaProperty> properties)
        {
            SqlStatement retVal = context.CreateSqlStatement();

            if (parms.Count() > 0)
            {
                foreach (var s in parms)
                {

                    object rValue = s.Value;
                    if (!(rValue is IList))
                        rValue = new List<Object>() { rValue };

                    retVal.Append("(");

                    string key = s.Key.Replace(".", "_").Replace("[]", ""),
                        scopedQuery = objectName + ".";

                    // Property info
                    var pi = properties.FirstOrDefault(o => o.Name == key);

                    foreach (var itm in rValue as IList)
                    {
                        var value = itm;
                        String filter = String.Empty;
                        var op = "AND";

                        if (value is String)
                        {
                            var sValue = itm as String;
                            switch (sValue[0])
                            {
                                case '<':
                                    if (sValue[1] == '=')
                                    {
                                        filter = $" {key} <= ?";
                                        value = sValue.Substring(2);
                                    }
                                    else
                                    {
                                        filter = $" {key} < ?";
                                        value = sValue.Substring(1);
                                    }
                                    break;
                                case '>':
                                    if (sValue[1] == '=')
                                    {
                                        filter = $"{key} >= ?";
                                        value = sValue.Substring(2);
                                    }
                                    else
                                    {
                                        filter = $"{key} > ?";
                                        value = sValue.Substring(1);
                                    }
                                    break;
                                case '!':
                                    if (sValue.Equals("!null"))
                                    {
                                        filter = $"{key} IS NOT NULL";
                                        value = sValue = null;
                                    }
                                    else
                                    {
                                        filter = $"{key} <> ?";
                                        value = sValue.Substring(1);
                                    }
                                    break;
                                case '~':
                                    filter = $"{key} {this.m_configuration.Provider.CreateSqlKeyword(OrmLite.Providers.SqlKeyword.ILike)} '%' || ? || '%'";
                                    value = sValue.Substring(1);
                                    op = "OR";

                                    break;
                                default:
                                    if (sValue.Equals("null"))
                                    {
                                        filter = $"{key} IS NULL";
                                        value = sValue = null;
                                    }
                                    else
                                    {
                                        filter = $"{key} = ?";
                                        op = "OR";
                                    }
                                    break;
                            }

                            sValue = value as String;
                            if (sValue != null)
                                switch (pi.Type)
                                {
                                    case SchemaPropertyType.Binary:
                                        value = Convert.FromBase64String(sValue);
                                        break;
                                    case SchemaPropertyType.Boolean:
                                        value = Boolean.Parse(sValue);
                                        break;
                                    case SchemaPropertyType.Date:
                                        value = DateTime.Parse(sValue).Date;
                                        break;
                                    case SchemaPropertyType.DateTime:
                                    case SchemaPropertyType.TimeStamp:
                                        value = DateTimeOffset.Parse(sValue);
                                        break;
                                    case SchemaPropertyType.Decimal:
                                        value = Decimal.Parse(sValue);
                                        break;
                                    case SchemaPropertyType.Float:
                                        value = float.Parse(sValue);
                                        break;
                                    case SchemaPropertyType.Integer:
                                        value = int.Parse(sValue);
                                        break;
                                    case SchemaPropertyType.Uuid:
                                        value = Guid.Parse(sValue);
                                        break;
                                }
                        }
                        else if (value != null)
                            filter = $"{key} = ?";
                        else
                        {
                            filter = $"{key} IS NULL";
                            value = null;
                        }

                        // Append 
                        if (value != null)
                            retVal.Append(filter, value);
                        else
                            retVal.Append(filter);
                        retVal.Append(op);

                    }

                    retVal.RemoveLast();
                    retVal.Append(")").Append("AND");
                } // exist or value
                retVal.RemoveLast();
            }
            else
                retVal.Append("1 = 1");
            return retVal;
        }



        /// <summary>
        /// Create stored query internally 
        /// </summary>
        private DatamartStoredQuery CreateStoredQueryInternal(DataContext context, Guid datamartId, object queryDefinition)
        {
            var dmQuery = queryDefinition as DatamartStoredQuery;
            if (queryDefinition is ExpandoObject)
                dmQuery = JsonConvert.DeserializeObject<DatamartStoredQuery>(JsonConvert.SerializeObject(queryDefinition));

            // Not interested
            if (dmQuery == null) throw new ArgumentOutOfRangeException(nameof(queryDefinition));

            var mySql = dmQuery.Definition.FirstOrDefault(o => o.ProviderId == this.m_configuration.Provider.Name);

            if (mySql == null) return null;
            else if (!mySql.Query.Trim().StartsWith("select", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only SELECT allowed for stored queries");

            try
            {
                this.m_tracer.TraceInfo("Creating stored query {0}", dmQuery.Name);

                var mart = this.GetDatamart(context, datamartId);
                if (mart == null) throw new KeyNotFoundException(datamartId.ToString());

                context.ExecuteNonQuery(context.CreateSqlStatement($"DROP VIEW IF EXISTS sqp_{mart.Schema.Name}_{dmQuery.Name}"));

                // Create the data in the DMART
                SqlStatement queryBuilder = context.CreateSqlStatement("CREATE VIEW ")
                    .Append($"sqp_{mart.Schema.Name}_{dmQuery.Name} AS SELECT * FROM ({mySql.Query}) AS source");

                context.ExecuteNonQuery(queryBuilder);

                // Register the stored query and properties
                dmQuery.Id = Guid.NewGuid();
                context.Insert(new AdhocQuery()
                {
                    Name = dmQuery.Name,
                    QueryId = dmQuery.Id,
                    SchemaId = mart.Schema.Id
                });
                this.CreateProperties(context, String.Empty, mart.Schema, dmQuery);

                return dmQuery;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error creating stored query {0} : {1}", dmQuery.Name, e);
                throw;
            }
        }

        /// <summary>
        /// Create a stored query
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerWarehouse)]
        public DatamartStoredQuery CreateStoredQuery(Guid datamartId, object queryDefinition)
        {
            this.ThrowIfDisposed();


            // Now create / register the data schema
            using (var context = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    context.Open();
                    context.BeginTransaction();
                    var retVal = this.CreateStoredQueryInternal(context, datamartId, queryDefinition);
                    if (retVal == null)
                        throw new KeyNotFoundException(datamartId.ToString());
                    context.Transaction.Commit();
                    return retVal;
                }
                catch (Exception e)
                {
                    context.Transaction.Rollback();
                    this.m_tracer.TraceError("Error creating stored query {0} : {1}", queryDefinition, e);
                    throw;
                }
            }

        }

        /// <summary>
        /// Get the specified data mart
        /// </summary>
        private DatamartDefinition GetDatamart(DataContext context, Guid datamartId)
        {


            var dm = context.FirstOrDefault<AdhocDatamart>(o=>o.DatamartId == datamartId);
            if (dm == null) return null;
            else
                return new DatamartDefinition()
                {
                    Id = dm.DatamartId,
                    CreationTime = dm.CreationTime.DateTime,
                    Name = dm.Name,
                    Schema = this.LoadSchema(context, dm.SchemaId)
                };

        }

        /// <summary>
        /// Load schema for the specified item
        /// </summary>
        private DatamartSchema LoadSchema(DataContext context, Guid id)
        {
            DatamartSchema retVal = null;

            var dbsc = context.FirstOrDefault<AdhocSchema>(o => o.SchemaId == id);
            if (dbsc == null) return null;

            retVal = new DatamartSchema()
            {
                Id = dbsc.SchemaId,
                Name = dbsc.Name
            };

            // Stored queries
            retVal.Queries = context.Query<AdhocQuery>(o => o.SchemaId == id).Select(o =>
              new DatamartStoredQuery(){
                  Id = o.QueryId,
                  Name = o.Name,
                  Properties = this.LoadProperties(context, o.QueryId)
              }).ToList();

            // properties 
            retVal.Properties = this.LoadProperties(context, id);

            // TODO: load schema
            return retVal;
        }

        /// <summary>
        /// Load properties for the specified container id
        /// </summary>
        private List<DatamartSchemaProperty> LoadProperties(DataContext context, Guid containerId)
        {
            return context.Query<AdhocProperty>(o => o.ContainerId == containerId || o.SchemaId == containerId).Select(o => new DatamartSchemaProperty()
            {
                Attributes = (SchemaPropertyAttributes)o.Attributes,
                Type = (SchemaPropertyType)o.TypeId,
                Name = o.Name,
                Id = o.PropertyId,
                Properties = this.LoadProperties(context, o.PropertyId)
            }).ToList();
        }

        /// <summary>
        /// Executes an adhoc query 
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadWarehouseData)]
        public IEnumerable<dynamic> AdhocQuery(string queryText)
        {
            this.ThrowIfDisposed();

            using(var context = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    context.Open();
                    return context.Query<ExpandoObject>(context.CreateSqlStatement(queryText));
                }
                catch(Exception e)
                {
                    this.m_tracer.TraceError("Error executing {0} : {1}", queryText, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Truncate the specified datamart
        /// </summary>
        public void Truncate(Guid datamartId)
        {
            this.ThrowIfDisposed();

            using (var context = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    context.Open();
                    context.BeginTransaction();
                    var mart = this.GetDatamart(context, datamartId);
                    if (mart == null)
                        throw new KeyNotFoundException(datamartId.ToString());

                    // First, we delete the record
                    var delSql = context.CreateSqlStatement($"TRUNCATE {mart.Schema.Name}");
                    context.ExecuteNonQuery(delSql);
                    this.DeleteProperties(context, mart.Schema.Name, mart.Schema);

                    context.Transaction.Commit();
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error deleting {0} : {1}", datamartId, e);
                    context.Transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
