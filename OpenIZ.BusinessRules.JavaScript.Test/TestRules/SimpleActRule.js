/**
 * Sample Business Rule for Act
 * If act is type Adjustment then update held entity
 */
OpenIZBre.AddBusinessRule("Act", "AfterInsert", function (act)
{
    return act;
});