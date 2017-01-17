/// <Reference path="./openiz.js"/>
/// <Reference path="./openiz.js"/>

var OpenIZWarehouse = OpenIZWarehouse || {

    /** 
     * @class 
     * @public
     * @memberof OpenIZWarehouse
     * @summary Represents an ad-hoc data mart
     * @property {string} id The identifier for the datamart
     * @property {string} name The name of the datamart
     * @property {Date} creationTime The time that the data mart was created
     * @property {OpenIZWarehouse.DatamartSchema} schema The schema for the datamart
     * @param {OpenIZWarehouse.DatamartDefinition} copyData Data from which to copy the datamart definition
     */
    DatamartDefinition: function (copyData) {
        if (copyData) {
            this.id = copyData.id,
            this.name = copyData.name;
            this.creationTime = copyData.creationTime;
            this.schema = copyData.schema;
        }
    },

    /**
     * @class
     * @public
     * @memberof OpenIZWarehouse
     * @summary Represents a schema definition which describes the object that is to be persisted
     * @property {string} id The identifier for the schema
     * @property {string} name The name of the schema element. 
     * @property {OpenIZWarehouse.DatamartSchemaProperty} property The properties which are associated with the schema item
     * @param {OpenIZWarehouse.DatamartSchema} copyData Data from which the schema is to be copied
     */
    DatamartSchema: function (copyData) {
        if (copyData) {
            this.id = copyData.id;
            this.name = copyData.name;
            this.property = copyData.property;
        }
    },
    /** 
     * @class
     * @public
     * @summary Represents a single property which can be assigned to a schema object
     * @memberof OpenIZWarehouse
     * @property {string} id The identifier for the schema property object
     * @property {string} name The name of the schema property object
     * @property {OpenIZWarehouse.SchemaPropertyType} type The type of data which is to be stored in the warehouse property
     * @property {OpenIZWarehouse.SchemaPropertyAttributes} attributes The attributes applied to the property
     * @property {OpenIZWarehouse.DatamartSchemaProperty} property The properties to be stored as sub-properties of this property
     * @param {OpenIZWarehouse.DatamartSchemaProperty} copyData The data from which the schema property is to be copied
     */
    DatamartSchemaProperty: function (copyData) {
        if (copyData) {
            this.id = copyData.id;
            this.name = copyData.name;
            this.type = copyData.type;
            this.attributes = copyData.attributes;
            this.property = copyData.property;
        }
    },

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
         * @param {String} name The name of the datamart
         * @param {OpenIZWarehouse.DatamartSchema} schema The schema (definition) of the object being stored in the datamart
         * @return {OpenIZWarehouse.DatamartDefinition} The created datamart definition
         */
        createDatamart: function (name, schema) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null; // no warehouse service registered
                }

                // Execute a create datamart
                return serivce.CreateDatamart(name, schema);
            }
            catch (e) {
                console.error(e);
            }
        },
        /** 
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Gets a list of all available datamarts
         * @return {OpenIZWarehouse.DatamartDefinition} The datamarts in the warehouse file
         */
        getDatamarts: function () {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                return service.GetDatamarts();
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Deletes a datamart
         */
        deleteDatamart: function (martId) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                return service.DeleteDatamart(OpenIZBre.ParseGuid(martId));
            }
            catch (e) {
                console.error(e);
            }

        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @param {string} martId The datamart from which the object should be retrieved
         * @param {string} tupleId The identifier of the tuple in that data mart's schema
         * @return {Object} The data stored in that tuple
         */
        getObject: function (martId, tupleId) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                return service.Get(OpenIZBre.ParseGuid(martId), OpenIZBre.ParseGuid(tupleId));
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Executes a stored query on the datamart with the specified parameters
         * @param {string} martId The datamart from which the object should be retrieved
         * @param {string} queryName The query to be executed
         * @param {Object} parameter The object representing query parameters to the mart
         * @return {Object} A list of matching tuple or aggregates
         */
        query: function (martId, queryName, parameters) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                return service.StoredQuery(OpenIZBre.ParseGuid(martId), queryName, parameters);
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Adds the specified tuple to the datamart
         * @param {string} martId The datamart to which the object should be stored
         * @param {Object} object The object to be stored in the datamart
         * @return {string} The tuple identifier of the object stored
         */
        add: function (martId, object) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                return service.StoredQuery(OpenIZBre.ParseGuid(martId), object);
            }
            catch (e) {
                console.error(e);
            }
        },
        /** 
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Removes the specified tuple from the warehouse
         * @param {string} martId The datamart from which the object should be removed
         * @param {string} tupleId The identifier of the tuple to be deleted
         * @return {string} The tuple identifier of the object stored
         */
        remove: function (martId, tupleId) {
            try {

                // Get service
                var service = OpenIZBre.GetService("IAdHocDatawarehouseService");
                if (service == null) {
                    console.warn("No warehousing service registered");
                    return null;
                }

                // Execute get datamart
                return service.Delete(OpenIZBre.ParseGuid(martId), OpenIZBre.ParseGuid(tupleId));
            }
            catch (e) {
                console.error(e);
            }
        }
    }

}
