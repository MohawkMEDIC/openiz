Immunization Management System Query Language Semantics

Many OpenIZ components use LINQ (Language Independent Natural Query) expressions for querying the data model for complex data structures. This poses a problem, how
does one effectively represent LINQ as HTTP query parameters or in a text file? Because of this, the OpenIZ.Model assembly
exposes a QueryExpressionBuilder and QueryExpressionParser class. These two classes are responsible for parsing HTTP formatted
#IMSQL (IMS Query Language) queries to/from LINQ expressions.

## IMSQL Query Semantics

The semantics of IMSQ make it quite easy to write complex queries. 

### Properties

IMSQL navigates OpenIZ's reference model using the serialization properties. To understand this, one must view the documentation
for the IMSI formatted objects. The query parameters are quite simple, take for example a Patient object (note values are removed
for clarity):

'''
<Patient xmlns="http://openiz.org/model">
  <id/>
  <genderConcept/>
  <dateOfBirth/>
  <relationship>
    <relationshipType/>
    <target/>
  </relationship>
  <name>
    <useConcept/>
    <component>
      <type/>
      <value/>
    </component>
  </name>
</Patient>
'''

If we wanted to filter in IMSQ based on the patients dateOfBirth, the query is simple:

'''
?dateOfBirth=2015-06-01
'''

### Chaining Parameters

If we wanted to query by the value of a name component, the concept of chaining could be used. Chaining instructs the query
process to follow a property path and match. For example, if we wanted to filter a patient by their name components:

'''
?name.component.value=SMITH
'''

### Guard Conditions

Guard conditions allow us to block a particular collection of data using a classifier. The chaining example would match any
name component for any name of SMITH. If we wanted to only match Legal names where the Family name is SMITH, a guard can be used:

'''
?name[Legal].component[Family].value=SMITH
'''

### Null Guarding

Usually when we're querying against a database, the query processor does not care whether the property on a traversal is null,
this is not the case when querying against in-memory models like on the cache or care planner. When we want to guard against
nulls we can use the "elvis" operator. For example, if we didn't want a null reference exception on an in-memory query, we
would add elvis operators (aka null guards)

'''
?name[Legal]?.component[Family]?.value=SMITH
'''

### Casting

Sometimes, a property will point at a base class like Entity or Act, and our guard condition needs to proceed only if the
resulting relationship is a particular type (or we want to query against a type). In order to do this, we use the cast operator
or @.

Take for example the patient's relationships. The patient relationship of DedicatedServiceDeliveryLocation will point at a Place
however a relationship of Mother will point at a Person. If we want to query against a patient's dedicated service delivery
locations latitude (an attribute of place not entity):

'''
?relationship[DedicatedServiceDeliveryLocation].target@Place.lat=34.30493
'''

Note that this is not needed if the query target is only using base properties. For example if we wanted to query by a
patient's Mother's name, the query could either be vanilla:

'''
?relationship[Mother].target.name.component.value=MARY
'''

or with casting:

'''
?relationship[Mother].target@Person.name.component.value=MARY
'''

This is because "name" is a property of Entity which is the type of the target property anyways.

### Operators

IMSQL queries also support a variety of operators which are described below. Their behavior is also described

| Operator        | Syntax  | Applies to | Behavior                                                                                 | Example                            |
+-----------------+---------+------------+------------------------------------------------------------------------------------------+------------------------------------|
| EQUAL           | =       | Any        | Performs an exact match on the proeprty                                                  | ?name.component.value=SMITH        |
| NOT EQUAL       | =!      | Any        | Matches when the proeprty does not exactly equal the operand                             | ?name.component.value=!SMITH       |
| LIKE            | =~      | String/Date| Matches when the proeprty value is "like" the operand on date this means approx. match   | ?dateOfBirth=~2015                 |
| GREATER         | =>      | Number/Date| Matches when the property value is greater than the operand                              | ?dateOfBirth=>2015-01-01           |
| GREATER OR EQUAL| =>=     | Number/Date| Matches when the property value is greater or equal to the operand                       | ?dateOfBirth=>=2015-01-01          |
| LESS            | =<      | Number/Date| Matches when the property value is less than the operand                                 | ?dateOfBirth=<2015-01-01           |
| LESS OR EQUAL   | =<=     | Number/Date| Matches when the property value is less or equal than the operand                        | ?dateOfBirth=<=2015-01-01          |
| GREATER         | =>      | Number/Date| Matches when the property value is greater than the operand                              | ?dateOfBirth=>2015-01-01           |

### Fuzzy Matching Strings

When using the LIKE operator (~) on a string field, the semantics of * and ? are used where ? matches any one character 
and * matches 0 or more characters. For example:

  Name starting with SM:
'''
  ?name.component.value=~SM*
'''
  
  Name ending with SM:
'''
  ?name.component.value=~*SM
'''
  
  Name beginning with SM and ending with TH
'''
  ?name.component.value=~SM*TH
'''
  
### And/Or Semantics

IMSQL supports AND and OR semantics according to the following rules:

    - If two properties apppear in the same IMSQL query with the same operator they are treated as OR
        Name matching JOHN -OR- SMITH
'''
          ?name.component.value=JOHN&name.component.value=SMITH
'''
    - If two properties appear in the same IMSQL query with different operators they are treated as AND
        Date of birth greater than 2015-01-01 and 2015-12-31
'''
          ?dateOfBirth=>2015-01-01&dateOfBirth=<2015-01-01
'''
    - If two properties have the same guard condition they are treated as AND up until the last common guard 
        Given Legal name of JONH -and- Family Legal name of SMITH
'''
          ?name[Legal].component[Given].value=JOHN&name[Legal].component[Family].value=SMITH
'''
    - If two properties have the same guard condition are part of a larger guard they are treated as OR
		Given Legal name of JOHN -or- Given Legal Name Johnny
'''
          ?name[Legal].component[Given].value=JOHN&name[Legal].component[Given].value=JOHNNY
'''

This gets quite powerful when traversing relationships. For example, to return patients which have NOT received OPV
(showed up and refused, no stock, etc.)

'''
	?participation[RecordTarget].act.isNegated=true&participation[RecordTarget].act.participation[Consumable].player.typeConcept.mnemonic=VaccineType-OPV
'''

## Debugging your queries

Debugging of queries can be done using the OpenIZ Debugging Tool (oizdt) in any debug mode (either Rules or CarePlan). To 
test your queries simply launch the debugger and use the DBQ or SDQ (database query or set data query) commands:

'''
oizdt --tool=Debug --operation=CarePlan

# dbq Patient dateOfBirth=>2015-01-01 
'''

## Creating IMSQL from C#

If you're using C# to write a plugin, and wish to serialize or parse an IMSQL query you can use the QueryExpressionBuilder or QueryExpressionParser classes. These
classes are described in more depth in the OpenIZ API documentation guide.

## Where is IMSQL used?

IMSQL is used in the following OpenIZ IMS components:

	* OpenIZ IMS Interface (IMSI) for querying objects
	* OpenIZ Xml Clinical Protocol Engine (CDSS) for rule guards (one option, XML Linq and Linq are also options)
	* OpenIZ Disconnected Client for querying data in Applets
