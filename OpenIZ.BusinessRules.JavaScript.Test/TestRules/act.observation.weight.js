/// <reference path="~/.ref/js/openiz-bre.js"/>
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

/// <reference path="~/.ref/js/openiz-model.js"/>

OpenIZBre.AddBusinessRule("QuantityObservation", "BeforeInsert",
    /** 
     * @param {OpenIZModel.QuantityObservation} observation
     */
    function (observation) {

        var simplifiedObservation = observation;
        if (simplifiedObservation.$type != "QuantityObservation" ||
            simplifiedObservation.typeConcept != 'a261f8cd-69b0-49aa-91f4-e6d3e5c612ed')
            return observation;

        // We need to suggest ... yay! This is the fun part
        var rct = simplifiedObservation.participation.RecordTarget.playerModel;
        var ageAtObservation = Math.round((simplifiedObservation.actTime - rct.dateOfBirth) / 8.64e7);

        // Gender concept
        var refData = null;
        switch (rct.genderConcept) {
            case "094941e9-a3db-48b5-862c-bc289bd7f86c":
                refData = JSON.parse(OpenIZ.App.loadDataAsset("weight-ranges-female.json"));
                break;
            case "f4e3a6bb-612e-46b2-9f77-ff844d971198":
                refData = JSON.parse(OpenIZ.App.loadDataAsset("weight-ranges-male.json"));
                break;
        }

        // get the age
        if (simplifiedObservation.value < refData[ageAtObservation].zMinus2)
            simplifiedObservation.interpretationConcept = '6188f821-261f-420c-9520-0de240a05661';
        else if (simplifiedObservation.value > refData[ageAtObservation].zPlus2)
            simplifiedObservation.interpretationConcept = '3c4d6579-7496-4b44-aac1-18a714ff7a05';
        else
            simplifiedObservation.interpretationConcept = '41d42abf-17ad-4144-bf97-ec3fd907f57d';

        return simplifiedObservation;
    });


OpenIZBre.AddValidator("QuantityObservation",
    /** 
     * @param {OpenIZModel.QuantityObservation} observation
     */
    function (observation) {

        var simplifiedObservation = observation;
        if (simplifiedObservation.$type != "QuantityObservation" ||
            simplifiedObservation.typeConcept != 'a261f8cd-69b0-49aa-91f4-e6d3e5c612ed')
            return [];

        var retVal = [];
        // Verify that the weight is not 0
        if (observation.value == 0)
            retVal.push(new OpenIZBre.DetectedIssue("locale.encounter.cdss.weight.offscale0", OpenIZBre.IssuePriority.Error));
        if (observation.value >= 50)
            retVal.push(new OpenIZBre.DetectedIssue("locale.encounter.cdss.weight.superscale", OpenIZBre.IssuePriority.Error));
        if (observation.unitOfMeasure != 'a0a8d4db-db72-4bc7-9b8c-c07cef7bc796')
            retVal.push(new OpenIZBre.DetectedIssue("locale.encounter.cdss.weight.kgonly", OpenIZBre.IssuePriority.Error));

        return retVal;
    });