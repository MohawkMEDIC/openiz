// Empty guid
EmptyGuid : "00000000-0000-0000-0000-000000000000",

/**
     * @class
     * @summary Represents a simple exception class
     * @constructor
     * @memberof OpenIZModel
     * @property {String} message Informational message about the exception
     * @property {Object} details Any detail / diagnostic information
     * @property {OpenIZModel#Exception} caused_by The cause of the exception
     * @param {String} message Informational message about the exception
     * @param {Object} detail Any detail / diagnostic information
     * @param {OpenIZModel#Exception} cause The cause of the exception
     */
Exception : function (message, detail, cause) {
    _self = this;

    this.message = message;
    this.details = detail;
    this.caused_by = cause;

}