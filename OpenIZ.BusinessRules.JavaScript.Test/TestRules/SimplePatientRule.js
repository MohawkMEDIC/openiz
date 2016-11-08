/**
 * Sample Business Rule for Act
 * If act is type Adjustment then update held entity
 */
OpenIZBre.AddBusinessRule("Act", "AfterInsert", function (act) {

    return act;
});

/**
 * Sample Business Rule for Patient
 */
OpenIZBre.AddBusinessRule("Patient", "AfterInsert", function (patient)
{
    return patient;
});

/** 
 * Sample Validator for Patient - 
 * Must have gender, must be present
 */
OpenIZBre.AddValidator("Patient", function (patient) {

    var retVal = new Array();

    if (patient == null)
        retVal.push({ text: "NullValue", priority: 1 });
    else if (patient.GenderConceptKey == null)
        retVal.push({ text: "NoGender", priority: 1 });

    return retVal;

});