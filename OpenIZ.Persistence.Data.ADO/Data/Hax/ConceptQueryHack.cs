using OpenIZ.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Map;
using OpenIZ.Persistence.Data.ADO.Data.Model;

namespace OpenIZ.Persistence.Data.ADO.Data.Hax
{
    /// <summary>
    /// This particular hack will override queries where concepts are filtered by mnemonic to be more efficient
    /// </summary>
    public class ConceptQueryHack : IQueryBuilderHack
    {

        // The mapper to be used
        private ModelMapper m_mapper;

        /// <summary>
        /// Creates a new query hack
        /// </summary>
        public ConceptQueryHack(ModelMapper mapper)
        {
            this.m_mapper = mapper;
        }

        /// <summary>
        /// Hack the particular query
        /// </summary>
        public bool HackQuery(QueryBuilder builder, SqlStatement sqlStatement, SqlStatement whereClause, PropertyInfo property, String queryPrefix, QueryPredicate predicate, object values, IEnumerable<TableMapping> scopedTables)
        {

            // Hack mnemonic queries
            if (typeof(Concept).IsAssignableFrom(property.PropertyType) && predicate.SubPath == "mnemonic")
            {

                // Has this already been joined? 
                var declType = TableMapping.Get(this.m_mapper.MapModelType(property.DeclaringType));
                var keyProperty = property.PropertyType == typeof(Guid) ? property : property.DeclaringType.GetRuntimeProperty(property.Name + "Key");
                var declProp = declType.GetColumn(this.m_mapper.MapModelProperty(property.DeclaringType, declType.OrmType, keyProperty));
                if(declProp.ForeignKey == null) return false; // No FK link

                var tblMap = TableMapping.Get(this.m_mapper.MapModelType(property.PropertyType));
                var fkTbl = TableMapping.Get(declProp.ForeignKey.Table);
                string directFkName = $"{queryPrefix}{fkTbl.TableName}";

                // We have to join to the FK table
                if (!declProp.IsAlwaysJoin) 
                {
                    var fkColumn = fkTbl.GetColumn(declProp.ForeignKey.Column);
                    sqlStatement.Append($" INNER JOIN {fkTbl.TableName} AS {directFkName}_{declProp.Name} ON ({queryPrefix}{declType.TableName}.{declProp.Name} = {directFkName}_{declProp.Name}.{fkColumn.Name})");
                    directFkName += $"_{declProp.Name}";
                }

                // We aren't yet joined to our table, we need to join to our table though!!!!
                if (declProp.ForeignKey.Table != tblMap.OrmType) 
                {
                    var fkKeyColumn = fkTbl.Columns.FirstOrDefault(o => o.ForeignKey?.Table == tblMap.OrmType && o.Name == tblMap.PrimaryKey.First().Name) ??
                        tblMap.Columns.FirstOrDefault(o=>o.ForeignKey?.Table == fkTbl.OrmType && o.Name == fkTbl.PrimaryKey.First().Name);
                    if (fkKeyColumn == null) return false; // couldn't find the FK link

                    // Now we want to filter our FK
                    var tblName = $"{queryPrefix}{declProp.Name}_{tblMap.TableName}";
                    sqlStatement.Append($" INNER JOIN {tblMap.TableName} AS {tblName} ON ({directFkName}.{fkKeyColumn.Name} = {tblName}.{fkKeyColumn.Name})");

                    // Append the where clause
                    whereClause.And(builder.CreateWhereCondition(property.PropertyType, predicate.SubPath, values, $"{queryPrefix}{declProp.Name}_", new List<TableMapping>() { tblMap }));

                    // Add obslt_utc version?
                    if (typeof(IDbBaseData).IsAssignableFrom(tblMap.OrmType))
                        whereClause.And($"{tblName}.{tblMap.GetColumn(nameof(IDbBaseData.ObsoletionTime)).Name} IS NULL");
                }
               
                return true;
            }
            else
                return false;

        }
    }
}
