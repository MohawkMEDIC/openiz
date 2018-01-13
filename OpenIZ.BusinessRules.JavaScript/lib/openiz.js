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

/**
 * @callback OpenIZ~continueWith
 * @summary The function call which is called whenever an asynchronous operation completes successfully
 * @param {Object} data The result data from the asynchronous callback
 * @param {Object} state State data which is to be passed to the async callback
 */
/**
 * @callback OpenIZ~onException
 * @summary The exception handling callback whenever an asynchronous operation does not complete successfully
 * @param {OpenIZModel#Exception} exception The exception which was thrown as a result of the operation.
 * @param {Object} state State data which is to be passed to the async callback
 */
/**
 * @callback OpenIZ~finally
 * @summary The callback which is always executed from an asynchronous call regardless of whehter the operation was successful or not
 * @param {Object} state State data which is to be passed to the async callback
 */
/**
 * @summary OpenIZ Javascript binding class.
 *
 * @description The purpose of this object is to facilitate and organize OpenIZ applet integration with the backing  * OpenIZ container. For example, to allow an applet to get the current on/offline status, or authenticate a user.
 * @namespace OpenIZ
 * @property {Object} _session the current session
 * @property {Object} urlParams the current session
 */
var OpenIZ = OpenIZ || {

    /** 
     * @summary URL Parameters
     */
    urlParams: {},
    /** 
     * @summary Provides a mechanism for user interface interaction with OpenIZ
     * @class
     * @static
     * @memberof OpenIZ
     */
    UserInterface: {

    },
    /**
     * @summary Provides operations for managing {@link OpenIZModel.Act} instances.
     * @memberof OpenIZ
     * @static
     * @class
     */
    Act: {
        /**
         * @summary Asynchronously deletes an encounter object in the IMS
         * @memberof OpenIZ.Act
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the act that is to be updated
         * @see {OpenIZ.IMS.delete}
         * @see OpenIZModel.Act
         */
        obsoleteAsync: function (controlData) {
            OpenIZ.Ims.delete({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                id: controlData.id,
                state: controlData.state
            });
        },
        /**
         * @summary Creates a fulfillment relationship act
         * @description This method creates a new act which fulfills the specified act
         * @param {OpenIZModel.Act} act The act which the new act should fulfill
         */
        createFulfillment: function (act) {

            var fulfills = new OpenIZModel.Act();

            // Clone act
            switch (act.$type) {
                case "SubstanceAdministration":
                    fulfills = new OpenIZModel.SubstanceAdministration(act);
                    break;
                case "Observation":
                    fulfills = new OpenIZModel.Observation(act);
                    break;
                case "QuantityObservation":
                    fulfills = new OpenIZModel.QuantityObservation(act);
                    break;
                case "CodedObservation":
                    fulfills = new OpenIZModel.CodedObservation(act);
                    break;
                case "TextObservation":
                    fulfills = new OpenIZModel.TextObservation(act);
                    break;
                case "PatientEncounter":
                    fulfills = new OpenIZModel.PatientEncounter(act);
                    break;
                case "ControlAct":
                    fulfills = new OpenIZModel.ControlAct(act);
                    break;
                default:
                    break;
            }

            // Re-assign the identifier
            fulfills._overdue = act.stopTime < new Date();
            fulfills.id = OpenIZ.App.newGuid();
            fulfills.moodConcept = OpenIZModel.ActMoodKeys.Eventoccurrence;
            fulfills.moodConceptModel = null;
            fulfills.creationTime = new Date();
            fulfills.createdBy = fulfills.createdByModel = null;
            fulfills.statusConcept = OpenIZModel.StatusKeys.Active;
            fulfills.statusConceptModel = null;
            fulfills.etag = null;
            // Add fulfillment relationship
            fulfills.relationship = fulfills.relationship || {};
            fulfills.relationship.Fulfills = new OpenIZModel.ActRelationship();
            fulfills.relationship.Fulfills.target = act.id;
            fulfills.relationship.Fulfills.targetModel = act;

            if (fulfills._overdue) {
            }
            else {
                fulfills.actTime = new Date();
                fulfills.startTime = fulfills.stopTime = null;
            }

            return fulfills;
        },
        /**
          * @summary Perform a search of acts asynchronously
          * @memberof OpenIZ.Act
          * @method
          * @param {Object} controlData An object containing search, offset, count and callback data
          * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
          * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
          * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
          * @param {object} controlData.query The query filters to apply to the search
          * @param {int} controlData.query._count The limit of results to return from the ims
          * @param {int} controlData.query._offset The offset of the search result window
          * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
          * @see {OpenIZ.IMS.get}
          */
        findAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: controlData.query,
                state: controlData.state
            });
        },
        /**
         * @summary Performs a patient insert asynchronously
         * @memberof OpenIZ.Act
         * @method
         * @see {OpenIZ.Ims.post}
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZModel.Act} controlData.data The data which is to be inserted on the IMS
         * @example
         * OpenIZ.Act.insertAsync({
         *      data: new OpenIZModel.Act(...),
         *      continueWith: function(result) { // Do something with result },
         *      onException: function(ex) { // Handle exception }
         * });
         */
        insertAsync: function (controlData) {
            OpenIZ.Ims.post({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data,
                state: controlData.state
            });
        },
    },
    /**
     * @summary Interoperation with the IMS
     * @see OpenIZModel
     * @static
     * @class
     * @memberof OpenIZ
     */
    Ims: {
        /**
         * @summary Post data to the IMS
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {object} controlData.data The IMSI resource data to be posted to the IMS
         * @method
         */
        post: function (controlData) {
            try {
                console.info("OpenIZ BRE IMS > POST" + JSON.stringify(controlData.data));

                var retVal = OpenIZBre.Insert(controlData.data);
                if (controlData.continueWith)
                    controlData.continueWith(retVal);
            }
            catch (e) {
                console.error(e + "");

                if (controlData.onException)
                    controlData.onException(e);
                else
                    throw e;
            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        },
        /**
         * @summary Put (Update) data in the IMS
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {uuid} controlData.id The identifier of the resource to be updated
         * @param {uuid} controlData.versionId The version identifier of the resource to be updated
         * @param {object} controlData.data The IMSI resource data to be posted to the IMS
         * @method
        */
        put: function (controlData) {
            try {
                console.info("OpenIZ BRE IMS > PUT " + JSON.stringify(controlData.data));

                var retVal = OpenIZBre.Save(controlData.data);
                if (controlData.continueWith)
                    controlData.continueWith(retVal);
            }
            catch (e) {
                console.error(e + "");

                if (controlData.onException)
                    controlData.onException(e);
                else
                    throw e;

            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        },
        /**
         * @summary Get data from the IMS
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {object} controlData.query The IMSI query (see documentation for structure) to execute
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @method
         */
        get: function (controlData) {
            try {
                console.info("OpenIZ BRE IMS > GET " + JSON.stringify(controlData.query));
                var retVal = null;
                if (controlData.query._id != null)
                    retVal = OpenIZBre.Get(controlData.resource, controlData.query._id);
                else {
                    retVal = new OpenIZModel.Bundle(OpenIZBre.Find(controlData.resource, controlData.query));
                }

                if(controlData.continueWith)
                    controlData.continueWith(retVal);
            }
            catch (e) {
                console.error(e + "");
                if (controlData.onException)
                    controlData.onException(e);
                else
                    throw e;

            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        },
        /**
         * @summary Deletes data from the IMS
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {object} controlData.id The identifier of the object to delete from the IMS 
         * @method
         */
        delete: function (controlData) {
            try {
                console.info("OpenIZ BRE IMS > DELETE" + controlData.id);

                var retVal = OpenIZBre.Obsolete(controlData.resource, controlData.id);
                if (controlData.continueWith)
                    controlData.continueWith(retVal);
            }
            catch (e) {
                console.error(e + "");

                if (controlData.onException)
                    controlData.onException(e);
                else
                    throw e;

            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        }
    },
    /**
     * @summary Stock Functions
     * @description In particular stock actions comprise a series of calculations
     * @memberof OpenIZ
     * @static
     * @class
     */
    Stock: {
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @param {int} qYear TODO
         * @param {int} pSupply TODO
         * @return TODO
         */
        calculateQPeriod: function (qYear, pSupply) {
            return qYear / 12 * pSupply;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateSReserve: function (qPeriod, reservePercent) {
            return qPeriod * reservePercent;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateSMax: function (qPeriod, sReserve) {
            return qPeriod + sReserve;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateSReorder: function (sReserve, qPeriod, lTime, pSupply) {
            return sReserve + qPeriod * lTime / pSupply;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateQOrder: function (sMax, sAvailable, qPeriod, lTime, pSupply) {
            return sMax - sAvailable + qPeriod * lTime / pSupply;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateQNeeded: function (sStart, qRecieved, sEnd, sLost) {
            return (sStart + qRecieved) - (sEnd + sLost);
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateAll: function (qYear, pSupply, reservePercent, lTime, sAvailable) {
            var object = {};
            object.qPeriod = OpenIZ.Stock.calculateQPeriod(qYear, pSupply);
            object.sReserve = OpenIZ.Stock.calculateSReserve(object.qPeriod, reservePercent);
            object.sMax = OpenIZ.Stock.calculateSMax(object.qPeriod, object.sReserve);
            object.sReorder = OpenIZ.Stock.calculateSReorder(object.sReserve, object.qPeriod, lTime, pSupply);
            object.qOrder = OpenIZ.Stock.calculateQOrder(object.sMax, sAvailable, object.qPeriod, lTime, pSupply);
            return object;
        }
    },
    /**
     * @summary Utility functions which assist in the writing of other OpenIZ functions
     * @static
     * @class
     * @memberof OpenIZ
     */
    Util: {
        /** 
         * @summary Renders the specified concept name
         * @memberof OpenIZ.Util
         * @method
         * @param {OpenIZModel.ConceptName} name The concept name to be rendered
         */
        renderConceptName: function (name) {
            if (typeof (name) == "String") return name;
            else if (name[OpenIZ.Localization.getLocale()] != null)
                return name[OpenIZ.Localization.getLocale()];
            else
                return name[Object.keys(name)[0]];
        },
        /**
         * @summary Render address for display
         * @method
         * @memberof OpenIZ.Util
         * @param {OpenIZModel.EntityAddress} entity The addres of the entity or the entity itself to render the address of
         * @return {string} The address formatted as an appropriate string for simple formatting
         */
        renderAddress: function (entity) {
            if (entity === undefined) return;

            var address = entity.component !== undefined ? entity :
                entity.address !== undefined ? (entity.address.Direct || entity.address.HomeAddress) :
                (entity.Direct || entity.HomeAddress);
            var retVal = "";
            if (address.component.AdditionalLocator)
                retVal += address.component.AdditionalLocator + ", ";
            if (address.component.StreetAddressLine)
                retVal += address.component.StreetAddressLine + ", ";
            if (address.component.City)
                retVal += address.component.City + ", ";
            if (address.component.County != null)
                retVal += address.component.County + ", ";
            if (address.component.State != null)
                retVal += address.component.State + ", ";
            if (address.component.Country != null)
                retVal += address.component.Country + ", ";
            return retVal.substring(0, retVal.length - 2);
        },
        /**
         * @summary Render act as a simple string
         * @memberof OpenIZ.Util
         * @method
         * @param {OpenIZModel.Act} act The act to render as a simple string
         * @return {string} The rendered string 
         */
        renderAct: function (act) {
            switch (act.$type) {
                case "SubstanceAdministration":
                    return OpenIZ.Localization.getString("locale.encounters.administer") +
                        OpenIZ.Util.renderName(act.participation.Product.name.OfficialRecord);
                case "QuantityObservation":
                case "CodedObservation":
                case "TextObservation":
                    return OpenIZ.Localization.getString('locale.encounters.observe') +
                        act.typeConceptModel.name[OpenIZ.Localization.getLocale()];
                default:
                    return "";
            }
        },
        /**
         * @summary Log an exception to the console
         * @method
         * @memberof OpenIZ.Util
         * @param {OpenIZModel.Exception} e The exception to be logged to the console
         */
        logException: function (e) {
            console.warn(e);
        },
        /** 
         * @summary Render a manufactured material as a simple string
         * @method
         * @memberof OpenIZ.Util
         * @param {OpenIZModel.ManufacturedMaterial} material The material which is to be rendered as a string
         * @return {string} The material rendered as a string in format "<<name>> [LN# <<ln>>]"
         */
        renderManufacturedMaterial: function (material) {
            var name = OpenIZ.Util.renderName(material.name.OfficialRecord || material.name.Assigned);
            return name + "[LN#: " + material.lotNumber + "]";
        },
        /** 
         * @summary Renders a name as a simple string
         * @method
         * @meberof OpenIZ.Util
         * @param {OpenIZModel.EntityName} entityName The entity name to be rendered in the appropriate format
         * @return {string} The rendered entity name
         */
        renderName: function (entityName) {
            if (entityName === null || entityName === undefined)
                return "";
            else if (entityName.join !== undefined)
                return entityName.join(' ');
            else if (entityName.component !== undefined) {
                var nameStr = "";
                if (entityName.component.Given !== undefined) {
                    if (typeof (entityName.component.Given) === "string")
                        nameStr += entityName.component.Given;
                    else if (entityName.component.Given.join !== undefined)
                        nameStr += entityName.component.Given.join(' ');
                    nameStr += " ";
                }
                if (entityName.component.Family !== undefined) {
                    if (typeof (entityName.component.Family) === "string")
                        nameStr += entityName.component.Family;
                    else if (entityName.component.Family.join !== undefined)
                        nameStr += entityName.component.Family.join(' ');
                }
                if (entityName.component.$other !== undefined) {
                    if (typeof (entityName.component.$other) === "string")
                        nameStr += entityName.component.$other;
                    else if (entityName.component.$other.join !== undefined)
                        nameStr += entityName.component.$other.join(' ');
                    else if (entityName.component.$other.value !== undefined)
                        nameStr += entityName.component.$other.value;

                }
                return nameStr;
            }
            else
                return entityName;
        },
        /**
         * @summary Changes the specified date string into an appropriate ISO string
         * @memberof OpenIZ.Util
         * @method
         * @param {String} date The date to be formatted
         * @return {string} A DATE as an ISO String only
         */
        toDateInputString: function (date) {
            return date.toISOString().substring(0, 10);
        }
    },
    /** 
    * @summary The authentication section is used to interface with OpenIZ's authentication sub-systems including session management information, etc.
    * @see OpenIZModel.SecurityUser
     * @static
     * @class
    * @memberof OpenIZ
    */
    Authentication: {
        /**
        * @summary Performs a query against the UserEntity
        * @memberof OpenIZ.Authentication
        * @param {Object} controlData Task control data
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @param {object} controlData.query The query object which represents the filters for the object
        * @param {int} controlData.query._count The limit of results to return from the ims
        * @param {int} controlData.query._offset The offset of the search result window
        * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
        * @method
        * @see {OpenIZ.Ims.get}
        */
        getUserAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "UserEntity",
                query: controlData.query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
    },
    /** 
     * @summary Represents functions for interacting with the protocol service
     * @see OpenIZModel.Patient
     * @see OpenIZModel.Act
     * @static
     * @class
     * @memberof OpenIZ
     */
    CarePlan: {
        /**
         * @summary Interprets the observation, setting the interpretationConcept property of the observation
         * @param {OpenIZModel.QuantityObservation} obs The observation which is to be interpretation
         * @param {string} ruleSet The rule set to be applied for the clinical decision
         * @memberof OpenIZ.CarePlan
         * @method
         */
        interpretObservation: function (obs, patient, ruleSet) {
            obs.participation = obs.participation || {};
            obs.participation.RecordTarget = obs.participation.RecordTarget || {};
            obs.participation.RecordTarget.playerModel = patient;

            var postVal = OpenIZBre.ExecuteRule("BeforeInsert", obs);
            obs.interpretationConcept = postVal.interpretationConcept;

            obs.participation.RecordTarget.playerModel = null;

        },
        /**
         * @summary Generate a care plan for the specified patient
         * @memberof OpenIZ.CarePlan
         * @method
         * @param {object} controlData The data which controls the asynchronous operation
         * @param {date} controlData.minDate If the care plan result is to be filtered, then the lower bound of the care plan to retrieve
         * @param {date} controlData.maxDate If the care plan result is to be filtered on an upper bound then the care plan to retrieve
         * @param {date} controlData.onDate Specifies the care plan service only return those objects where the proposed action should occur on the specified date
         * @param {uuid} controlData.classConcept Specifies the classification of acts which should be returned
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZModel.Patient} controlData.data The seed data which should be passed to the forecasting engine in order to calculate the plan
         * @param {string} controlData.query The additional query parameters which should be passed to the forecaster
         * @example
         * OpenIZ.CarePlan.getCarePlanAsync({
         *     maxDate: new Date(), // Only retrieve objects that should have already occurred
         *     classConcept: "932A3C7E-AD77-450A-8A1F-030FC2855450", // Only retrieve substance administrations
         *     data: { // Manually construct a patient to pass
         *          dateOfBirth: new Date(), // The patient was born today
         *          genderConceptModel: { mnemonic: "Male" }, // the patient is a male
         *      },
         *      continueWith: function(careplan) {
         *          alert("There are " + careplan.participation.RecordTarget.length + " proposed objects");
         *      },
         *      onException: function(ex) {
         *          alert(ex.message);
         *      }
         * });
         */
        getCarePlanAsync: function (controlData) {
            console.log(controlData);
            var url = "moodConcept=ACF7BAF2-221F-4BC2-8116-CEB5165BE079";
            if (controlData.minDate !== undefined)
                url += "&actTime=>" + controlData.minDate.toISOString();
            if (controlData.maxDate !== undefined)
                url += "&actTime=<" + controlData.maxDate.toISOString();
            if (controlData.onDate !== undefined)
                url += "&startTime=<" + controlData.onDate.toISOString() + "&stopTime=>" + controlData.onDate.toISOString();
            if (controlData.classConcept !== undefined)
                url += "&classConcept=" + controlData.classConcept;
            if (controlData.query !== undefined)
                url += "&" + controlData.query;
            console.info("Generating care plan...");
            try {
                var retVal = OpenIZBre.GenerateCarePlan(controlData.data, url);
                controlData.continueWith(retVal);
            }
            catch (e) {
                controlData.onException(e);
            }
            finally {
                if (controlData.finally)
                    controlData.finally();
            }
        },
    },
    /**
    * @summary Represents application specific functions for interoperating with the mobile application itself
     * @static
     * @class
    * @memberof OpenIZ
    */
    App: {
        /**
         * @summary Loads an asset synchronously from the data/ directory
         * @method
         * @memberof OpenIZ.App
         */
        loadDataAsset: function (dataId) {
            return OpenIZBre.GetDataAsset(dataId);
        },
        /**
         * @description Because JavaScript lacks native UUID generation, this function calls a JNI method to generate a new UUID which can be appended to IMS objects
         * @return {uuid} A newly generated uuid
         * @method
         * @memberof OpenIZ.App
         */
        newGuid: function () {
            return OpenIZBre.NewGuid();
        },
        /** 
         * @summary Gets the current version of the OpenIZ host
         * @method
         * @memberof OpenIZ.App
         * @return {String} The version code of the hosting environment
         */
        getVersion: function () {
            return OpenIZBre.GetVersion();
        },
        /**
         * @summary Indicates that the server should clear an object from the cache
        * @method
        * @memberof OpenIZ.App
        * @param {object} controlData The data which controls the operation of the asynchronous operation
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {object} controlData.data The object which is to be removed from the cache. Minimally a $type and id
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        */
        deleteCacheAsync: function (controlData) {
            try
            {
                OpenIZBre.DeleteCache(controlData.data.$type, controlData.data.id);
                if(controlData.continueWith)
                    controlData.continueWith(null, controlData.state);
            }
            catch(e)
            {
                if(controlData.onException)
                    controlData.onException(e, controlData.state);
            }
            finally {
                if(controlData.finally)
                    controlData.finally();
            }

        }
    },
    /**
     * @summary Represents functions related to the localization of applets
     * @static
     * @class
     * @memberof OpenIZ
     */
    Localization: {
        /**
         * @summary Gets the specified localized string the current display language from the resources file
         * @memberof OpenIZ.Localization
         * @method
         * @param {String} stringId The identifier of the string
         * @returns The specified string
         */
        getString: function (stringId) {
            try {
                return OpenIZBre.GetString(stringId);
            }
            catch (e) {
                console.error(e);
                return stringId;
            }
        },
        /**
         * @summary Gets the current user interface locale name
         * @memberof OpenIZ.Localization
         * @method
         * @returns The ISO language code of the current UI 
         */
        getLocale: function () {
            return (navigator.language || navigator.userLanguage).substring(0, 2);
        },
    },
    /**
     * @summary Represents functions related to the concept dictionary in particular those dealing with {@link OpenIZModel.Concept}, and {@link OpenIZModel.ConceptSet}s
     * @static
     * @class
     * @memberof OpenIZ
     */
    Concept: {
        /**
         * @summary Perform a search asynchronously
         * @memberof OpenIZ.Concept
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @see {OpenIZ.IMS.get}
         * @see OpenIZModel.Concept
         * @example
         * OpenIZ.Concept.findConceptAsync({
         *      query: { "mnemonic":"Female" },
         *      continueWith: function(result) { // do something with result },
         *      onException: function(ex) { // handle exception }
         *  });
         */
        findConceptAsync: function (controlData) {

            OpenIZ.Ims.get({
                resource: "Concept",
                query: controlData.query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
         * @summary Perform a search of concept sets asynchronously
         * @memberof OpenIZ.Concept
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @see {OpenIZ.IMS.get}
         * @see OpenIZModel.ConceptSet
         * @example
         * OpenIZ.Concept.findConceptSetAsync({
         *      query: { "member.mnemonic":"Female" },
         *      continueWith: function(result) { // do something with result },
         *      onException: function(ex) { // handle exception }
         *  });
         */
        findConceptSetAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "ConceptSet",
                query: controlData.query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        }
    },

    /**
     * @summary Represents a series of functions for submitting {@link OpenIZModel.Bundle} instances
     * @static
     * @class
     * @memberof OpenIZ
     */
    Bundle: {
        /**
         * @summary Insert the bundle asynchronously
         * @memberof OpenIZ.Bundle
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        insertAsync: function (controlData) {
            OpenIZ.Ims.post({
                resource: "Bundle",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data,
                state: controlData.state
            });
        }
    },
    /**
     * @summary Represents a series of functions related to {@link OpenIZModel.Patient} instances
     * @static
     * @class
     * @memberof OpenIZ
     */
    Patient: {
        /**
         * @summary Perform a search of patients asynchronously
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @see {OpenIZ.IMS.get}
         * @see OpenIZModel.Patient
         */
        findAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: controlData.query,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously insert a patient object into the IMS
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.data The patient data to be inserted into the IMS
         * @see {OpenIZ.IMS.post}
         * @see OpenIZModel.Patient
         */
        insertAsync: function (controlData) {
            OpenIZ.Ims.post({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously updates a patient object in the IMS
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.data The patient data to be inserted into the IMS
         * @param {uuid} controlData.id The identifier of the patient that is to be updated
         * @see {OpenIZ.IMS.put}
         * @see OpenIZModel.Patient
         */
        updateAsync: function (controlData) {
            OpenIZ.Ims.put({
                resource: "Patient",
                data: controlData.data,
                id: controlData.id,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously deletes a patient object in the IMS
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the patient that is to be updated
         * @see {OpenIZ.IMS.delete}
         * @see OpenIZModel.Patient
         */
        obsoleteAsync: function (controlData) {
            OpenIZ.Ims.delete({
                resource: "Patient",
                id: controlData.id,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        }
    },

    /**
     * @summary Provides a series of utility functions for interacting with {@link OpenIZModel.Place} instances
     * @static
     * @class
     * @memberof OpenIZ
     */
    Place: {
        /**
         * @summary Perform an asynchronous search on the place resource
         * @method
         * @memberof OpenIZ.Place
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @see OpenIZModel.Place
         */
        findAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Place",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: controlData.query,
                state: controlData.state
            });
        }
    },
    /**
     * @static
     * @class
     * @summary Provides functions for managing {@link OpenIZModel.Provider} objects on the IMS
     * @memberof OpenIZ
     */
    Provider: {
        /**
         * @summary Perform an asynchronous search on the provider resource
         * @method
         * @memberof OpenIZ.Provider
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         */
        findProviderAsync: function (controlData) {
            OpenIZ.Ims.get({
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                resource: 'Provider',
                query: controlData.query,
                state: controlData.state
            });
        }
    },
    /**
     * @summary Entity class for interacting with {@link OpenIZModel.Entity} instances and derivatives
     * @see OpenIZModel.Entity
     * @see OpenIZModel.Person
     * @see OpenIZModel.Place
     * @see OpenIZModel.Material
     * @see OpenIZModel.ManufacturedMaterial
     * @see OpenIZModel.Patient
     * @see OpenIZModel.UserEntity
     * @see OpenIZModel.Provider
     * @static
     * @class
     * @memberof OpenIZ
     */
    Entity: {
        /**
         * @summary Get an empty entity template object asynchronously. See the OpenIZ documentation for more information about templates
         * @method
         * @memberof OpenIZ.Entity
         * @param {object} controlData The data which controls the asynchronous operation.
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.templateId The identifier of the template which to retrieve. Templates should be registered with the application manifest
         * @example
         * OpenIZ.CarePlan.getEntityTemplateAsync({
         *      templateId: "Entity.Patient.Baby", 
         *      continueWith: function(template) {
         *          $scope.act = template;
         *      }
         * });
         */
        getEntityTemplateAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Entity/Template",
                query: { "templateId": controlData.templateId },
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        }
    },
    /**
     * @static
     * @class
     * @summary Provides utility functions for interacting with {@link OpenIZModel.ManufacturedMaterial} instances
     * @memberOf OpenIZ.ManufacturedMaterial
     */
    ManufacturedMaterial:
    {
        /**
         * @deprecated
         * @see OpenIZ.ManufacturedMaterial.getManufacturedMaterialsAsync
         */
        getManufacturedMaterials: function (controlData) {
            OpenIZ.ManufacturedMaterial.getManufacturedMaterialAsync(controlData);
        },
        /**
         * @summary Get manufactured materials from the IMS 
         * @param {object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @memberof OpenIZ.ManufacturedMaterial
         * @method
         */
        getManufacturedMaterialAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "ManufacturedMaterial",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                query: controlData.query,
                state: controlData.state
            })
        }
    }
};

/**
 * @method
 * @summary Get the week of the year
 */
Date.prototype.getWeek = function () {
    var oneJan = this.getFirstDayOfYear();
    return Math.ceil((((this - oneJan) / 86400000) + oneJan.getDay() + 1) / 7);
}

/** 
 * @method
 * @summary Get the first day of the year
 */
Date.prototype.getFirstDayOfYear = function () {
    return new Date(this.getFullYear(), 0, 1);
}

/**
 * @method 
 * @summary Get the first day of the following week
 */
Date.prototype.nextMonday = function () {
    var retVal = this.getFirstDayOfYear();
    retVal.setDate(retVal.getDate() + (new Date().getWeek() * 7));
    return retVal;
}

/**
 * @summary Gets the date on the next day
 * @method
 * @param {Number} days The number of days to add
 */
Date.prototype.addDays = function (days) {
    var retVal = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
    retVal.setDate(retVal.getDate() + days);
    return retVal;
}

/**
 * @summary Gets the date on the next day
 * @method
 */
Date.prototype.tomorrow = function () {
    return this.addDays(1);
}

/**
 * @summary Gets the date on the previous day
 * @method
 */
Date.prototype.yesterday = function () {
    return this.addDays(-1);
}