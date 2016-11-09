/// <reference src="OpenIZModel.js"/>

/**
 * Sample Business Rule for Patient
 */
OpenIZBre.AddBusinessRule("Patient", "AfterInsert", function (patient) {
    // Simplify
    var simplePatient = OpenIZBre.SimplifyObject(patient);

    console.assert(simplePatient != null, "Patient is null");
    console.assert(simplePatient.genderConceptModel != null, "Gender is null");
    console.assert(simplePatient.genderConceptModel.mnemonic == "Female", "Expected Female");
    console.assert(simplePatient.participation != null, "Participation missing");
    console.assert(simplePatient.participation.RecordTarget != null, "Record Target missing");
    console.assert(simplePatient.participation.RecordTarget.actModel.unitOfMeasureModel != null, "Unit of measure missing");
    console.assert(simplePatient.name != null, "Names null");
    console.assert(simplePatient.name.Legal != null, "Names missing Legal");
    console.assert(simplePatient.name.Legal.component != null, "Name missing components");
    console.assert(simplePatient.name.Legal.component.Given == "James", "Expected James as given name");
    console.assert(simplePatient.name.Legal.component.Family == "Smith", "Expected Smith as family");

    simplePatient.dateOfBirth = new Date();
    var expanded = OpenIZBre.ExpandObject(simplePatient);
    return { value: expanded };
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