/// <Reference path="./openiz.js"/>
/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */

/// <Reference path="./openiz.js"/>

var OpenIZWarehouse = OpenIZWarehouse || {

    // OpenIZ.Core.Data.Warehouse.DatamartDefinition, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents a datamart definition which contains the definition of fields for a datamart            
     * @property {uuid} id            Gets or sets the identifier of the data mart            
     * @property {string} name            Gets or sets the name of the data mart            
     * @property {date} creationTime            Gets or sets the time that the data mart was created            
     * @property {OpenIZWarehouse.DatamartSchema} schema            Gets or sets the datamart schema            
     * @param {OpenIZWarehouse.DatamartDefinition} copyData Copy constructor (if present)
     */
    DatamartDefinition: function (copyData) {
        this.$type = 'DatamartDefinition';
        if (copyData) {
            this.schema = copyData.schema;
            this.creationTime = copyData.creationTime;
            this.name = copyData.name;
            this.id = copyData.id;
        }
    },  // DatamartDefinition 
    // OpenIZ.Core.Data.Warehouse.DatamartSchema, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents a datamart schema which gives hints to the properties to be stored from             a dynamic object            
     * @property {uuid} id            Gets or sets the unique identifier for the schema itself            
     * @property {string} name            Gets or sets the name of the element in the database            
     * @property {OpenIZWarehouse.DatamartSchemaProperty} property            Gets or sets the property names for the schema element            
     * @property {OpenIZWarehouse.DatamartStoredQuery} sqp            Gets or sets the query associated with the schema            
     * @param {OpenIZWarehouse.DatamartSchema} copyData Copy constructor (if present)
     */
    DatamartSchema: function (copyData) {
        this.$type = 'DatamartSchema';
        if (copyData) {
            this.sqp = copyData.sqp;
            this.property = copyData.property;
            this.name = copyData.name;
            this.id = copyData.id;
        }
    },  // DatamartSchema 
    // OpenIZ.Core.Data.Warehouse.DatamartSchemaProperty, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents a single property on the data mart schema            
     * @property {uuid} id            Gets or sets the identifier of the warehouse property            
     * @property {string} name            Gets or sets the name of the property            
     * @property {SchemaPropertyType} type            Gets or sets the type of property            
     * @property {SchemaPropertyAttributes} attributes            Gets or sets the attributes associated with the property            
     * @property {OpenIZWarehouse.DatamartSchemaProperty} property            Gets or sets the sub-properties of this property            
     * @param {OpenIZWarehouse.DatamartSchemaProperty} copyData Copy constructor (if present)
     */
    DatamartSchemaProperty: function (copyData) {
        this.$type = 'DatamartSchemaProperty';
        if (copyData) {
            this.property = copyData.property;
            this.attributes = copyData.attributes;
            this.type = copyData.type;
            this.name = copyData.name;
            this.id = copyData.id;
        }
    },  // DatamartSchemaProperty 
    // OpenIZ.Core.Data.Warehouse.DatamartStoredQuery, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents a stored query creation statement            
     * @property {uuid} id            Gets or sets the provider identifier            
     * @property {string} name            Definition of the query            
     * @property {OpenIZWarehouse.DatamartSchemaProperty} property            Gets or sets the property names for the schema element            
     * @property {OpenIZWarehouse.DatamartStoredQueryDefinition} select            Definition of the query            
     * @param {OpenIZWarehouse.DatamartStoredQuery} copyData Copy constructor (if present)
     */
    DatamartStoredQuery: function (copyData) {
        this.$type = 'DatamartStoredQuery';
        if (copyData) {
            this.select = copyData.select;
            this.property = copyData.property;
            this.name = copyData.name;
            this.id = copyData.id;
        }
    },  // DatamartStoredQuery 
    // OpenIZ.Core.Data.Warehouse.DatamartStoredQueryDefinition, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents the SQL for an actual query            
     * @property {string} provider            Provider identifier            
     * @property {string} sql            The SQL             
     * @param {OpenIZWarehouse.DatamartStoredQueryDefinition} copyData Copy constructor (if present)
     */
    DatamartStoredQueryDefinition: function (copyData) {
        this.$type = 'DatamartStoredQueryDefinition';
        if (copyData) {
            this.sql = copyData.sql;
            this.provider = copyData.provider;
        }
    },  // DatamartStoredQueryDefinition 

    /** 
     * @enum {int}
     * @memberof OpenIZWarehouse
     * @public
     * @readonly
     * @summary Schema property attributes
     */
    SchemaPropertyAttributes: {
        None: 0x0,
        Indexed: 0x1,
        NotNull: 0x2,
        Unique: 0x4
    },

    /** 
     * @enum {int}
     * @memberof OpenIZWarehouse
     * @public
     * @readonly
     * @summary Schema property types
     */
    SchemaPropertyType: {
        String: 0,
        Integer: 1,
        Float: 2,
        Date: 3,
        Boolean: 4,
        Uuid: 5,
        Binary: 6,
        Object: 7
    },

    /**
     * @class 
     * @memberof OpenIZWarehouse
     * @summary Represents the ad-hoc warehouse service wrappers
     * @static
     */
    Adhoc: {
        /** 
         * @memberof OpenIZWarehouse.Adhoc
         * @method
         * @summary Creates a new datamart with the specified schema
         * @param {object} controlData the control data
         * @param {String} controlData.name The name of the datamart
         * @param {OpenIZWarehouse.DatamartSchema} controlData.schema The schema (definition) of the object being stored in the datamart
         * @return {OpenIZWarehouse.DatamartDefinition} The created datamart definition
         */
        createDatamartAsync: function (controlData) {
            try {


                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null; // no warehouse service registered
                }

                // Execute a create datamart
                if(controlData.continueWith)
                    controlData.continueWith(serivce.CreateDatamart(controlData.name, controlData.schema));
            }
            catch (e) {
                console.error(e);
                if(controlData.onException)
                    controlData.onException(e);
            }
            finally {
                if(controlData.finally)
                    controlData.finally();
            }
        },
        /** 
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Gets a list of all available datamarts
         */
        getDatamartsAsync: function (controlData) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                if(controlData.continueWith)
                    controlData.continueWith(service.GetDatamarts());
            }
            catch (e) {
                console.error(e);
                if (controlData.onException)
                    controlData.onException(e);
            }
            finally
            {
                if (controlData.finally)
                    controlData.finally();
            }
        },
        /** 
         * @method
         * @param {object} controlData 
         * @param {string} controlData.name
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Gets a list of all available datamarts
         */
        getDatamartAsync: function (controlData) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                if (controlData.continueWith)
                    controlData.continueWith(service.GetDatamart(controlData.name));
            }
            catch (e) {
                console.error(e);
                if (controlData.onException)
                    controlData.onException(e);
            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Deletes a datamart
         */
        deleteDatamartAsync: function (controlData) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                if (controlData.continueWith)
                    controlData.continueWith(service.DeleteDatamart(OpenIZBre.ParseGuid(controlData.martId)));
            }
            catch (e) {
                console.error(e);
                if (controlData.onException)
                    controlData.onException(e);
            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }

        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @param {object} controlData the control data
         * @param {string} controlData.martId The datamart from which the object should be retrieved
         * @param {string} controlData.tupleId The identifier of the tuple in that data mart's schema
         * @return {Object} The data stored in that tuple
         */
        getObjectAsync: function (controlData) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                if(controlData.continueWith)
                    controlData.continueWith(service.Get(OpenIZBre.ParseGuid(controlData.martId), OpenIZBre.ParseGuid(controlData.tupleId)));
            }
            catch (e) {
                console.error(e);
                if (controlData.onException)
                    controlData.onException(e);
            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @param {object} controlData the control data
         * @param {string} controlData.martId The datamart from which the object should be retrieved
         * @param {string} controlData.query The query to match control data on
         * @return {Object} The data stored in that tuple
         */
        adhocQueryAsync : function (controlData) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                if (controlData.continueWith)
                    controlData.continueWith(service.AdhocQuery(OpenIZBre.ParseGuid(controlData.martId), controlData.query));
            }
            catch (e) {
                console.error(e);
                if (controlData.onException)
                    controlData.onException(e);
            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Executes a stored query on the datamart with the specified parameters
         * @param {Object} controlData The control data for the operation
         * @param {string} controlData.martId The datamart from which the object should be retrieved
         * @param {string} controlData.queryName The query to be executed
         * @param {Object} controlData.parameter The object representing query parameters to the mart
         * @return {Object} A list of matching tuple or aggregates
         */
        queryAsync: function (controlData) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                if (controlData.continueWith)
                    controlData.continueWith(service.StoredQuery(OpenIZBre.ParseGuid(controlData.martId), controlData.queryName, controlData.parameters));
            }
            catch (e) {
                console.error(e);
                if (controlData.onException)
                    controlData.onException(e);
            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Adds the specified tuple to the datamart
        * @param {Object} controlData The control data for the operation
         * @param {string} controlData.martId The datamart to which the object should be stored
         * @param {Object} controlData.object The object to be stored in the datamart
         * @return {string} The tuple identifier of the object stored
         */
        addAsync: function (controlData) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                if (controlData.continueWith)
                    controlData.continueWith(service.StoredQuery(OpenIZBre.ParseGuid(controlData.martId), controlData.object));
            }
            catch (e) {
                console.error(e);
                if (controlData.onException)
                    controlData.onException(e);
            }
            finally {
                if(controlData.finally)
                    controlData.finally();
            }
        },
        /** 
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Removes the specified tuple from the warehouse
        * @param {Object} controlData The control data for the operation
         * @param {string} controlData.martId The datamart from which the object should be removed
         * @param {string} controlData.tupleId The identifier of the tuple to be deleted
         * @return {string} The tuple identifier of the object stored
         */
        removeAsync: function (controlData) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                if (controlData.continueWith)
                    controlData.continueWith(service.Delete(OpenIZBre.ParseGuid(controlData.martId), OpenIZBre.ParseGuid(controlData.tupleId)));
            }
            catch (e) {
                console.error(e);
                if (controlData.onException)
                    controlData.onException(e);
            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        }
    }

}
