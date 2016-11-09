/// <reference href="openizmodel.js"/>
/**
 * Sample Business Rule for Manufactured Material
 */
OpenIZBre.AddBusinessRule("ManufacturedMaterial", "AfterInsert", function (manufacturedMaterial)
{
    return { value: manufacturedMaterial };
});