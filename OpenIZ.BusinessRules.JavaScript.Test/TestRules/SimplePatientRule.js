/// <reference path="openiz-model.js"/>
/// <reference path="linq.js"/>

/**
 * Sample Business Rule for Patient
 */
OpenIZBre.AddBusinessRule("Patient", "AfterInsert", function (patient) {
    // Simplify
    var simplePatient = new OpenIZModel.Patient(OpenIZBre.SimplifyObject(patient));

    // Should get service
    var serviceManager = OpenIZBre.GetService("IServiceManager");
    console.assert(serviceManager != null, "Missing Service Manager");
    console.assert(serviceManager.AddServiceProvider !== undefined, "Service Manager isn't really really a service manager");
    serviceManager.AddServiceProvider(null);
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

    simplePatient.tag["foo"] = Enumerable.from(simplePatient.name.Legal).where("$.component != null");
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