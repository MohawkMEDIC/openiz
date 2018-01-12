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
OpenIZBre.AddBusinessRule("Act", "AfterInsert", function (act)
{
    var simplifiedAct = new OpenIZModel.Act(act);

    if (simplifiedAct.classConcept != OpenIZModel.ActClassKeys.AccountManagement ||
        simplifiedAct.typeConcept != "XXXX" ||
        !simplifiedAct.tag["hasRunAdjustment"])
        return { value: act }; // not interested

    var simplePlace = OpenIZBre.Data.Get("Place", simplifiedAct.participation.Location.targetModel.id);
    var materialId = simplifiedAct.participation.Product.playerModel.id;
    var materialRelationship = new OpenIZModel.EntityRelationship(simplePlace.relationship.OwnedEntity.where(function (x) { return x.targetModel.id == materialId }));
    materialRelationship.quantity -= simplifiedAct.participation.Product.quantity;
    OpenIZ.Save(simplePlace);

    simplifiedAct.tag["hasRunAdjustment"] = true;

    return { value: act };
});
