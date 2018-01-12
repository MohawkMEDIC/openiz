/// <reference path="openiz.js"/>
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
 * @summary Execution environment for BRE in Javascript
 * @class
 * @static
 * @description The BRE normally runs in the server as a series of rules, however bcause some rules may
 * need to be run as local rules for the user interface, this class simulates the BRE back-end service
 */
var OpenIZBre = OpenIZBre || {
    /**
     * @summary Issue priority
     * @enum
     */
    IssuePriority: {
        Error: 1,
        Warning: 2,
        Information: 3
    },
    /** 
     * @summary Represents a detected issue
     * @constructor
     * @param {string} text The textual content of the issue
     * @param {OpenIZBre.IssuePriority} priority
     * @memberof OpenIZBre
     */
    DetectedIssue: function (text, priority)
    {
        this.text = text;
        this.priority = priority;
    },
    /**
     * @summary Allows rules to know they're running in the UI rather than the back-end
     * @memberof OpenIZBre
     */
    $inFrontEnd: true,
    /**
     * @summary Reference sets loaded
     */
    $refSets: {},
    /** 
     * @summary Reference set base URLs
     */
    $refSetBase: [],
    /**
     * @summary The current list of triggers registered for javascript
     * @memberof OpenIZBre
     */
    _triggers: [],
    /**
     * @summary The current list of validators registered for javascript
     * @memberof OpenIZBre
     */
    _validators: [],
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Simulates the simplify 
     * @deprecated
     */
    ToViewModel: function (object) { return object; },
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Simulates the expand object method
     * @deprecated
     */
    FromViewModel: function (object) { return object; },
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Adds a trigger event to the triggers collection
     * @param {string} type The type of object being registered
     * @param {string} trigger The name of the BRE trigger to subscribe to
     * @param {function} callback The callback function
     */
    AddBusinessRule: function (type, trigger, callback)
    {
        this._triggers.push({
            type: type,
            trigger: trigger,
            callback: callback
        });
    },
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Adds a validator to the validatos collection
     * @param {string} type The type of object being registered
     * @param {function} callback The callback function
     */
    AddValidator: function (type, callback)
    {
        this._validators.push({
            type: type,
            callback: callback
        });
    },
    /** 
     * @method
     * @memberof OpenIZBre
     * @summary Simulates the rule being executed
     */
    ExecuteRule: function (trigger, instance)
    {
        // Execute the rule
        var retVal = instance;
        for (var t in this._triggers)
            if (this._triggers[t].type == instance.$type)
            {
                var triggerResult = this._triggers[t].callback(retVal);
                retVal = triggerResult || retVal;
            }
        return retVal;
    },
    /** 
     * @method
     * @memberof OpenIZBre
     * @summary Simulates the validation method being run
     */
    Validate: function (instance)
    {

        // Execute the rule
        var retVal = [];
        for (var t in this._validators)
            if (this._validators[t].type == instance.$type)
            {
                var issues = this._validators[t].callback(instance);
                for (var i in issues)
                    retVal.push(issues[i]);
            }
        return retVal;

    },
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Allows adding of a breakpoint
     */
    Break: function () {

    }
};