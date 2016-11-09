/// <reference path="openiz-model.js"/>
/// <reference path="linq.js"/>
/**
 * Sample Business Rule for Act
 * If act is type Adjustment then update held entity
 */
OpenIZBre.AddBusinessRule("Act", "AfterInsert", function (act)
{
    var simplifiedAct = new OpenIZModel.Act(OpenIZBre.SimplifyObject(act));

    if (simplifiedAct.classConcept != OpenIZModel.ActClassKeys.AccountManagement ||
        simplifiedAct.typeConcept != "XXXX" ||
        !simplifiedAct.tag["hasRunAdjustment"])
        return { value: act }; // not interested

    var placeRepository = OpenIZBre.GetService("IPlaceRepositoryService");
    var simplePlace = new OpenIZModel.Place(OpenIZBre.SimplifyObject(placeRepository.Get(simplifiedAct.participation.Location.targetModel.id)));
    var materialId = simplifiedAct.participation.Product.playerModel.id;
    var materialRelationship = new OpenIZModel.EntityRelationship(simplePlace.relationship.OwnedEntity.where(function (x) { return x.targetModel.id == materialId }));
    materialRelationship.quantity -= simplifiedAct.participation.Product.quantity;

    var PlaceExpander = OpenIZ.BusinessRules.JavaScript.Util.Expander(OpenIZ.Core.Model.Entities.Place);
    var expander = new PlaceExpander();

    placeRepository.Save(expander.Expand(simplePlace));

    simplifiedAct.tag["hasRunAdjustment"] = true;

    return { value: act };
});
