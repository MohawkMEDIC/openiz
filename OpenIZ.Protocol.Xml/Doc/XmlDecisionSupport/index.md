# OpenIZ XML Clinical Decision Support (CDSS) Engine

When designing OpenIZ, there was a significant need to have a high performance, cross platform decision support system
which could operate on any .NET platform (to allow for offline use on mobiles). This resulted in the design and implementation
of an XML based clinical protocol support engine. 

This engine is on IClincialProtocol implementation that can be used in OpenIZ, others can be implemented to override this 
behavior.

## Methodology & Terminology

In this document the following terminology is going to be used to describe the clinical decision support system in OpenIZ:

|  Term      | Definition                                                                                                                              |
+------------+-----------------------------------------------------------------------------------------------------------------------------------------+
| Protocol   | A clinical protocol represents a series of steps which carry out a particular part of a patient's care. For example, OPV is a protocol  |
| Care Plan  | A care plan is one or more clinical protocols put together to represent how the OpenIZ engine proposes the patient should receive care  |
| Rule       | Represents a guard condition or piece of logic which should be applied to the avaialble facts to reach the conclussion				   |
| Fact       | A piece of data which is fed into the care planner / clinical protocol as inputs which are used to form conclusions					   |
| Conclusion | A proposal that the system has created based on the application of rules against the facts provided.                                    |

## Loading of Clinical Protocols

By default, the OpenIZ clinical protocols are located in the applets which are uploaded into the IMS at startup. This allows an 
OpenIZ deployment to be more flexible based on the jurisdiction and applet plugins loaded.

## Layout of a Protocol File

The format of a protocol file is documented in more detail in the schema for these files (OpenIZProtocol.xsd). A protocol has four
main parts:

	1. The header which uniquely identifies the protocol 
	2. The entry rules which dictate whether a patient is eligible for consideration in the protocol
	3. One or more rules (or steps) in the protocol.

You can think of a protocol as a series of when/then conditions. Or, when this series of rules is true for this series of facts
then propose something.

The general format of this file is:

<ProtocolDefinition xmlns="http://openiz.org/cdss" name="" protocolVersion="" uuid="">
  <when evaluation="">
    <linqExpression/> 
	Or
	<imsiExpression negationIndicator=""/>
	Or
	<expressionGrouping evaluation="and|or">
		...
	</expressionGrouping>
  </when>
  <rule id="">
    <when evaluation="and|or">
		<linqExpression/> 
		Or
		<imsiExpression negationIndicator=""/>
		Or
		<expressionGrouping evaluation="and|or">
			...
		</expressionGrouping>
    </when>
    <then>
      <action>
        <jsonModel>
          <![CDATA[
			// JSON FORMATTED DATA
            ]]>
        </jsonModel>
        <assign propertyName="">LINQ Expression</assign>
      </action>
    </then>
  </rule>
</ProtocolDefinition>