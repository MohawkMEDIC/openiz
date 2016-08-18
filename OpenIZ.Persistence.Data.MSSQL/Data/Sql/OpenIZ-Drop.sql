ALTER TABLE [dbo].[EntityNote] DROP CONSTRAINT [FK_EntityNoteObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityNote] DROP CONSTRAINT [FK_EntityNoteEntityId]
GO

ALTER TABLE [dbo].[EntityNote] DROP CONSTRAINT [FK_EntityNoteEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityNote] DROP CONSTRAINT [FK_EntityNoteAuthorEntityId]
GO

ALTER TABLE [dbo].[ActExtension] DROP CONSTRAINT [FK_ActExtensionObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ActExtension] DROP CONSTRAINT [FK_ActExtensionExtensionTypeId]
GO

ALTER TABLE [dbo].[ActExtension] DROP CONSTRAINT [FK_ActExtensionEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ActExtension] DROP CONSTRAINT [FK_ActExtensionActId]
GO

ALTER TABLE [dbo].[Act] DROP CONSTRAINT [FK_ActVersionMoodConceptId]
GO

ALTER TABLE [dbo].[Act] DROP CONSTRAINT [FK_ActTemplateDefinitionId]
GO

ALTER TABLE [dbo].[Act] DROP CONSTRAINT [FK_ActClassConceptId]
GO

ALTER TABLE [dbo].[EntityAddressComponent] DROP CONSTRAINT [FK_EntityAddressComponentValueId]
GO

ALTER TABLE [dbo].[EntityAddressComponent] DROP CONSTRAINT [FK_EntityAddressComponentTypeConceptId]
GO

ALTER TABLE [dbo].[EntityAddressComponent] DROP CONSTRAINT [FK_EntityAddressComponentEntityAddressId]
GO

ALTER TABLE [dbo].[ActIdentifier] DROP CONSTRAINT [FK_ActIdentifierObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ActIdentifier] DROP CONSTRAINT [FK_ActIdentifierIdentifierTypeId]
GO

ALTER TABLE [dbo].[ActIdentifier] DROP CONSTRAINT [FK_ActIdentifierEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ActIdentifier] DROP CONSTRAINT [FK_ActIdentifierAssigningAuthorityId]
GO

ALTER TABLE [dbo].[ActIdentifier] DROP CONSTRAINT [FK_ActIdentifierActId]
GO

ALTER TABLE [dbo].[PatientEncounter] DROP CONSTRAINT [FK_PatientEncounterDischargeDispositionConceptId]
GO

ALTER TABLE [dbo].[PatientEncounter] DROP CONSTRAINT [FK_PatientEncounterActVersionId]
GO

ALTER TABLE [dbo].[PersonLanguageCommunication] DROP CONSTRAINT [FK_PersonLanguageCommunicationPersonEntityId]
GO

ALTER TABLE [dbo].[PersonLanguageCommunication] DROP CONSTRAINT [FK_PersonLanguageCommunicationEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[PersonLanguageCommunication] DROP CONSTRAINT [FK_PersonLanguageCommunciationObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[PlaceService] DROP CONSTRAINT [FK_PlaceServiceServiceConceptId]
GO

ALTER TABLE [dbo].[PlaceService] DROP CONSTRAINT [FK_PlaceServicePlaceEntityId]
GO

ALTER TABLE [dbo].[PlaceService] DROP CONSTRAINT [FK_PlaceServiceObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[PlaceService] DROP CONSTRAINT [FK_PlaceServiceEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[IdentifierType] DROP CONSTRAINT [FK_EntityIdentifierTypeIdentifierTypeConceptId]
GO

ALTER TABLE [dbo].[IdentifierType] DROP CONSTRAINT [FK_EntityIdentifierTypeEntityClassScopeConceptId]
GO

ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_ObservationInterpretationConceptId]
GO

ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_ObservationActVersionId]
GO

ALTER TABLE [dbo].[Patient] DROP CONSTRAINT [FK_PatientGenderConceptId]
GO

ALTER TABLE [dbo].[Patient] DROP CONSTRAINT [FK_PatientEntityVersionId]
GO

ALTER TABLE [dbo].[ActRelationship] DROP CONSTRAINT [FK_ActRelationshipTargetActId]
GO

ALTER TABLE [dbo].[ActRelationship] DROP CONSTRAINT [FK_ActRelationshipSourceActId]
GO

ALTER TABLE [dbo].[ActRelationship] DROP CONSTRAINT [FK_ActRelationshipRelationshipTypeConceptId]
GO

ALTER TABLE [dbo].[ActRelationship] DROP CONSTRAINT [FK_ActRelationshipObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ActRelationship] DROP CONSTRAINT [FK_ActRelationshipEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ActTag] DROP CONSTRAINT [FK_ActTagObsoletedBy]
GO

ALTER TABLE [dbo].[ActTag] DROP CONSTRAINT [FK_ActTagCreatedBy]
GO

ALTER TABLE [dbo].[ActTag] DROP CONSTRAINT [FK_ActTagActId]
GO

ALTER TABLE [dbo].[ActVersion] DROP CONSTRAINT [FK_ActVersionTypeConceptId]
GO

ALTER TABLE [dbo].[ActVersion] DROP CONSTRAINT [FK_ActVersionStatusConceptId]
GO

ALTER TABLE [dbo].[ActVersion] DROP CONSTRAINT [FK_ActVersionReplacesVersionId]
GO

ALTER TABLE [dbo].[ActVersion] DROP CONSTRAINT [FK_ActVersionObsoletedBy]
GO

ALTER TABLE [dbo].[ActVersion] DROP CONSTRAINT [FK_ActVersionCreatedBy]
GO

ALTER TABLE [dbo].[ActVersion] DROP CONSTRAINT [FK_ActVersionAct]
GO

ALTER TABLE [dbo].[ActParticipation] DROP CONSTRAINT [FK_ActParticipationRoleConceptId]
GO

ALTER TABLE [dbo].[ActParticipation] DROP CONSTRAINT [FK_ActParticipationObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ActParticipation] DROP CONSTRAINT [FK_ActParticipationEntityId]
GO

ALTER TABLE [dbo].[ActParticipation] DROP CONSTRAINT [FK_ActParticipationEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ActParticipation] DROP CONSTRAINT [FK_ActParticipationActId]
GO

ALTER TABLE [dbo].[ActPolicy] DROP CONSTRAINT [FK_ActPolicyPolicyId]
GO

ALTER TABLE [dbo].[ActPolicy] DROP CONSTRAINT [FK_ActPolicyObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ActPolicy] DROP CONSTRAINT [FK_ActPolicyEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ActPolicy] DROP CONSTRAINT [FK_ActPolicyActId]
GO

ALTER TABLE [dbo].[ActProtocol] DROP CONSTRAINT [FK_ActProtocolProtocolId]
GO

ALTER TABLE [dbo].[ActProtocol] DROP CONSTRAINT [FK_ActProtocolActId]
GO

ALTER TABLE [dbo].[ControlAct] DROP CONSTRAINT [FK_ControlActActVersionId]
GO

ALTER TABLE [dbo].[DeviceEntity] DROP CONSTRAINT [FK_DeviceEntityEntityVersionId]
GO

ALTER TABLE [dbo].[DeviceEntity] DROP CONSTRAINT [FK_DeviceEntityDeviceId]
GO

ALTER TABLE [dbo].[EntityAddress] DROP CONSTRAINT [FK_EntityAddressObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityAddress] DROP CONSTRAINT [FK_EntityAddressEntityId]
GO

ALTER TABLE [dbo].[EntityAddress] DROP CONSTRAINT [FK_EntityAddressEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityAddress] DROP CONSTRAINT [FK_EntityAddressAddressUseConceptId]
GO

ALTER TABLE [dbo].[ApplicationEntity] DROP CONSTRAINT [FK_ApplicationEntityVersionId]
GO

ALTER TABLE [dbo].[ApplicationEntity] DROP CONSTRAINT [FK_ApplicationEntitySecurityApplication]
GO

ALTER TABLE [dbo].[CodedObservation] DROP CONSTRAINT [FK_CodedObservationValueConceptId]
GO

ALTER TABLE [dbo].[CodedObservation] DROP CONSTRAINT [FK_CodedObservationActVersionId]
GO

ALTER TABLE [dbo].[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationshpObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationshipTargetConceptId]
GO

ALTER TABLE [dbo].[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationshipSourceConceptId]
GO

ALTER TABLE [dbo].[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationshipEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ConceptRelationship] DROP CONSTRAINT [FK_ConceptRelationshipConceptRelationshipTypeId]
GO

ALTER TABLE [dbo].[Protocol] DROP CONSTRAINT [FK_ProtocolReplacesProtocolId]
GO

ALTER TABLE [dbo].[Protocol] DROP CONSTRAINT [FK_ProtocolProtocolHandlerId]
GO

ALTER TABLE [dbo].[Protocol] DROP CONSTRAINT [FK_ProtocolObsoletedBy]
GO

ALTER TABLE [dbo].[Protocol] DROP CONSTRAINT [FK_ProtocolCreatedBy]
GO

ALTER TABLE [dbo].[StockBalance] DROP CONSTRAINT [FK_StockBalancePlaceEntity]
GO

ALTER TABLE [dbo].[StockBalance] DROP CONSTRAINT [FK_StockBalanceMaterialEntity]
GO

ALTER TABLE [dbo].[QuantifiedEntityAssociation] DROP CONSTRAINT [FK_QuantifiedEntityAssociationEntityAssociationId]
GO

ALTER TABLE [dbo].[QuantifiedActParticipation] DROP CONSTRAINT [FK_QuantifiedActParticipationId]
GO

ALTER TABLE [dbo].[SecurityUserClaim] DROP CONSTRAINT [FK_SecurityUserClaimUserId]
GO

ALTER TABLE [dbo].[SecurityUserLogins] DROP CONSTRAINT [FK_SecurityUserLoginsUserId]
GO

ALTER TABLE [dbo].[QuantityObservation] DROP CONSTRAINT [FK_QuantityObservationUnitOfMeasureConceptId]
GO

ALTER TABLE [dbo].[QuantityObservation] DROP CONSTRAINT [FK_QuantityObservationActVersionId]
GO

ALTER TABLE [dbo].[StockLedger] DROP CONSTRAINT [FK_StockLedgerPlaceEntity]
GO

ALTER TABLE [dbo].[StockLedger] DROP CONSTRAINT [FK_StockLedgerMaterialEntity]
GO

ALTER TABLE [dbo].[StockLedger] DROP CONSTRAINT [FK_StockLedgerActionConcept]
GO

ALTER TABLE [dbo].[ActNote] DROP CONSTRAINT [FK_ActNoteObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ActNote] DROP CONSTRAINT [FK_ActNoteEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ActNote] DROP CONSTRAINT [FK_ActNoteAuthorEntityId]
GO

ALTER TABLE [dbo].[ActNote] DROP CONSTRAINT [FK_ActNodeActId]
GO

ALTER TABLE [dbo].[SecurityDevicePolicy] DROP CONSTRAINT [FK_SecurityDevicePolicyPolicyId]
GO

ALTER TABLE [dbo].[SecurityDevicePolicy] DROP CONSTRAINT [FK_SecurityDevicePolicyDeviceId]
GO

ALTER TABLE [dbo].[ProtocolHandler] DROP CONSTRAINT [FK_ProtocolHandlerObsoletedBy]
GO

ALTER TABLE [dbo].[ProtocolHandler] DROP CONSTRAINT [FK_ProtocolHandlerCreatedBy]
GO

ALTER TABLE [dbo].[TemplateDefinition] DROP CONSTRAINT [FK_TemplateDefinitionUpdatedBy]
GO

ALTER TABLE [dbo].[TemplateDefinition] DROP CONSTRAINT [FK_TemplateDefinitionObsoletedBy]
GO

ALTER TABLE [dbo].[TemplateDefinition] DROP CONSTRAINT [FK_TemplateDefinitionCreatedBy]
GO

ALTER TABLE [dbo].[SubstanceAdministration] DROP CONSTRAINT [FK_SubstanceAdministrationSiteConceptId]
GO

ALTER TABLE [dbo].[SubstanceAdministration] DROP CONSTRAINT [FK_SubstanceAdministrationRouteConceptId]
GO

ALTER TABLE [dbo].[SubstanceAdministration] DROP CONSTRAINT [FK_SubstanceAdministrationDoseUnitConceptId]
GO

ALTER TABLE [dbo].[SubstanceAdministration] DROP CONSTRAINT [FK_SubstanceAdministrationActVersionId]
GO

ALTER TABLE [dbo].[TextObservation] DROP CONSTRAINT [FK_StringObservationActVersionId]
GO

ALTER TABLE [dbo].[ConceptRelationshipType] DROP CONSTRAINT [FK_ConceptRelationshipTypeUpdatedBy]
GO

ALTER TABLE [dbo].[ConceptRelationshipType] DROP CONSTRAINT [FK_ConceptRelationshipTypeObsoletedBy]
GO

ALTER TABLE [dbo].[ConceptRelationshipType] DROP CONSTRAINT [FK_ConceptRelationshipTypeCreatedBy]
GO

ALTER TABLE [dbo].[SecurityUserRole] DROP CONSTRAINT [FK_SecurityUserRoleUserId]
GO

ALTER TABLE [dbo].[SecurityUserRole] DROP CONSTRAINT [FK_SecurityUserRoleRoleId]
GO

ALTER TABLE [dbo].[SecurityRole] DROP CONSTRAINT [FK_SecurityRoleUpdatedBy]
GO

ALTER TABLE [dbo].[SecurityRole] DROP CONSTRAINT [FK_SecurityRoleObsoletedBy]
GO

ALTER TABLE [dbo].[SecurityRole] DROP CONSTRAINT [FK_SecurityRoleCreatedBy]
GO

ALTER TABLE [dbo].[SecurityRolePolicy] DROP CONSTRAINT [FK_SecurityRolePolicyRoleId]
GO

ALTER TABLE [dbo].[SecurityRolePolicy] DROP CONSTRAINT [FK_SecurityRolePolicyPolicyId]
GO

ALTER TABLE [dbo].[AssigningAuthority] DROP CONSTRAINT [FK_AssigningAuthorityObsoletedBy]
GO

ALTER TABLE [dbo].[AssigningAuthority] DROP CONSTRAINT [FK_AssigningAuthorityCreatedBy]
GO

ALTER TABLE [dbo].[AssigningAuthority] DROP CONSTRAINT [FK_AssigningAuthorityAssigningDeviceId]
GO

ALTER TABLE [dbo].[UserEntity] DROP CONSTRAINT [FK_UserEntityUserId]
GO

ALTER TABLE [dbo].[UserEntity] DROP CONSTRAINT [FK_UserEntityEntityId]
GO

ALTER TABLE [dbo].[AssigningAuthorityScope] DROP CONSTRAINT [FK_AssigningAuthorityScopeScope]
GO

ALTER TABLE [dbo].[AssigningAuthorityScope] DROP CONSTRAINT [FK_AssigningAuthorityScopeAuthority]
GO

ALTER TABLE [dbo].[ConceptClass] DROP CONSTRAINT [FK_ConceptClassUpdatedBy]
GO

ALTER TABLE [dbo].[ConceptClass] DROP CONSTRAINT [FK_ConceptClassObsoletedBy]
GO

ALTER TABLE [dbo].[ConceptClass] DROP CONSTRAINT [FK_ConceptClassCreatedBy]
GO

ALTER TABLE [dbo].[PhoneticAlgorithm] DROP CONSTRAINT [FK_PhoneticAlgorithmUpdatedBy]
GO

ALTER TABLE [dbo].[PhoneticAlgorithm] DROP CONSTRAINT [FK_PhoneticAlgorithmObsoletedBy]
GO

ALTER TABLE [dbo].[PhoneticAlgorithm] DROP CONSTRAINT [FK_PhoneticAlgorithmCreatedBy]
GO

ALTER TABLE [dbo].[Person] DROP CONSTRAINT [FK_PersonEntityVersionId]
GO

ALTER TABLE [dbo].[SecurityApplication] DROP CONSTRAINT [FK_SecurityApplicationObsoletedBy]
GO

ALTER TABLE [dbo].[SecurityApplication] DROP CONSTRAINT [FK_SecurityApplicationCreatedBy]
GO

ALTER TABLE [dbo].[ExtensionType] DROP CONSTRAINT [FK_ExtensionTypeUpdatedBy]
GO

ALTER TABLE [dbo].[ExtensionType] DROP CONSTRAINT [FK_ExtensionTypeObsoletedBy]
GO

ALTER TABLE [dbo].[ExtensionType] DROP CONSTRAINT [FK_ExtensionTypeEnabledBy]
GO

ALTER TABLE [dbo].[Organization] DROP CONSTRAINT [FK_OrganizationIndustryConceptId]
GO

ALTER TABLE [dbo].[Organization] DROP CONSTRAINT [FK_OrganizationEntityVersionId]
GO

ALTER TABLE [dbo].[ManufacturedMaterial] DROP CONSTRAINT [FK_ManufacturedMaterialEntityVersionId]
GO

ALTER TABLE [dbo].[Provider] DROP CONSTRAINT [FK_ProviderProviderSpecialtyConceptId]
GO

ALTER TABLE [dbo].[Provider] DROP CONSTRAINT [FK_ProviderEntityVersionId]
GO

ALTER TABLE [dbo].[SecurityApplicationPolicy] DROP CONSTRAINT [FK_SecurityApplicationPolicyPolicyId]
GO

ALTER TABLE [dbo].[SecurityApplicationPolicy] DROP CONSTRAINT [FK_SecurityApplicationPolicyApplicationId]
GO

ALTER TABLE [dbo].[SecurityDevice] DROP CONSTRAINT [FK_SecurityDeviceCreatedBy]
GO

ALTER TABLE [dbo].[SecurityDevice] DROP CONSTRAINT [FK_SecuirtyDeviceObsoletedBy]
GO

ALTER TABLE [dbo].[Policy] DROP CONSTRAINT [FK_PolicyObsoletedBy]
GO

ALTER TABLE [dbo].[Policy] DROP CONSTRAINT [FK_PolicyCreatedBy]
GO

ALTER TABLE [dbo].[CodeSystem] DROP CONSTRAINT [FK_CodeSystemUpdatedBy]
GO

ALTER TABLE [dbo].[CodeSystem] DROP CONSTRAINT [FK_CodeSystemObsoletedBy]
GO

ALTER TABLE [dbo].[CodeSystem] DROP CONSTRAINT [FK_CodeSystemCreatedBy]
GO

ALTER TABLE [dbo].[Material] DROP CONSTRAINT [FK_MaterialQuantityConceptId]
GO

ALTER TABLE [dbo].[Material] DROP CONSTRAINT [FK_MaterialFormConceptId]
GO

ALTER TABLE [dbo].[Material] DROP CONSTRAINT [FK_MaterialEntityVersionId]
GO

ALTER TABLE [dbo].[ConceptSet] DROP CONSTRAINT [FK_ConceptSetUpdatedBy]
GO

ALTER TABLE [dbo].[ConceptSet] DROP CONSTRAINT [FK_ConceptSetObsoletedBy]
GO

ALTER TABLE [dbo].[ConceptSet] DROP CONSTRAINT [FK_ConceptSetCreatedBy]
GO

ALTER TABLE [dbo].[EntityTelecomAddress] DROP CONSTRAINT [FK_EntityTelecomAddressUseConceptId]
GO

ALTER TABLE [dbo].[EntityTelecomAddress] DROP CONSTRAINT [FK_EntityTelecomAddressTelecomAddressTypeConceptId]
GO

ALTER TABLE [dbo].[EntityTelecomAddress] DROP CONSTRAINT [FK_EntityTelecomAddressObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityTelecomAddress] DROP CONSTRAINT [FK_EntityTelecomAddressEntityId]
GO

ALTER TABLE [dbo].[EntityTelecomAddress] DROP CONSTRAINT [FK_EntityTelecomAddressEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[Place] DROP CONSTRAINT [FK_PlaceEntityVersionId]
GO

ALTER TABLE [dbo].[SecurityUser] DROP CONSTRAINT [FK_SecurityUserUpdatedBy]
GO

ALTER TABLE [dbo].[SecurityUser] DROP CONSTRAINT [FK_SecurityUserObsoletedBy]
GO

ALTER TABLE [dbo].[SecurityUser] DROP CONSTRAINT [FK_SecurityUserCreatedBy]
GO

ALTER TABLE [dbo].[SecurityUser] DROP CONSTRAINT [FK_SecurityUserClass]
GO

ALTER TABLE [dbo].[ReferenceTermDisplayName] DROP CONSTRAINT [FK_ReferenceTermDisplayNameReferenceTermId]
GO

ALTER TABLE [dbo].[ReferenceTermDisplayName] DROP CONSTRAINT [FK_ReferenceTermDisplayNamePhoneticAlgorithmId]
GO

ALTER TABLE [dbo].[ReferenceTermDisplayName] DROP CONSTRAINT [FK_ReferenceTermDisplayNameObsoletedBy]
GO

ALTER TABLE [dbo].[ReferenceTermDisplayName] DROP CONSTRAINT [FK_ReferenceTermDisplayNameCreatedBy]
GO

ALTER TABLE [dbo].[ConceptSetMember] DROP CONSTRAINT [FK_ConceptSetMemberConceptSetId]
GO

ALTER TABLE [dbo].[ConceptSetMember] DROP CONSTRAINT [FK_ConceptSetMemberConceptId]
GO

ALTER TABLE [dbo].[PhoneticValues] DROP CONSTRAINT [FK_PhoneticValuesPhoneticAlgorithmId]
GO

ALTER TABLE [dbo].[Entity] DROP CONSTRAINT [FK_EntityTemplateDefinitionId]
GO

ALTER TABLE [dbo].[Entity] DROP CONSTRAINT [FK_EntityDeterminerConceptId]
GO

ALTER TABLE [dbo].[Entity] DROP CONSTRAINT [FK_EntityClassConceptId]
GO

ALTER TABLE [dbo].[ConceptReferenceTerm] DROP CONSTRAINT [FK_ConceptReferenceTermReferenceTermId]
GO

ALTER TABLE [dbo].[ConceptReferenceTerm] DROP CONSTRAINT [FK_ConceptReferenceTermObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ConceptReferenceTerm] DROP CONSTRAINT [FK_ConceptReferenceTermEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ConceptReferenceTerm] DROP CONSTRAINT [FK_ConceptReferenceTermConceptRelationshipTypeId]
GO

ALTER TABLE [dbo].[ConceptReferenceTerm] DROP CONSTRAINT [FK_ConceptReferenceTermConceptId]
GO

ALTER TABLE [dbo].[EntityNameComponent] DROP CONSTRAINT [FK_EntityNameComponentValueId]
GO

ALTER TABLE [dbo].[EntityNameComponent] DROP CONSTRAINT [FK_EntityNameComponentTypeConceptId]
GO

ALTER TABLE [dbo].[EntityNameComponent] DROP CONSTRAINT [FK_EntityNameComponentEntityNameId]
GO

ALTER TABLE [dbo].[EntityName] DROP CONSTRAINT [FK_EntityNameUseConceptId]
GO

ALTER TABLE [dbo].[EntityName] DROP CONSTRAINT [FK_EntityNameObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityName] DROP CONSTRAINT [FK_EntityNameEntityId]
GO

ALTER TABLE [dbo].[EntityName] DROP CONSTRAINT [FK_EntityNameEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ConceptName] DROP CONSTRAINT [FK_ConceptNamePhoneticAlgorithmId]
GO

ALTER TABLE [dbo].[ConceptName] DROP CONSTRAINT [FK_ConceptNameObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[ConceptName] DROP CONSTRAINT [FK_ConceptNameEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ConceptName] DROP CONSTRAINT [FK_ConceptNameConcept]
GO

ALTER TABLE [dbo].[ReferenceTerm] DROP CONSTRAINT [FK_ReferenceTermUpdatedBy]
GO

ALTER TABLE [dbo].[ReferenceTerm] DROP CONSTRAINT [FK_ReferenceTermObsoletedBy]
GO

ALTER TABLE [dbo].[ReferenceTerm] DROP CONSTRAINT [FK_ReferenceTermCreatedBy]
GO

ALTER TABLE [dbo].[ReferenceTerm] DROP CONSTRAINT [FK_ReferenceTermCodeSystemId]
GO

ALTER TABLE [dbo].[EntityAssociation] DROP CONSTRAINT [FK_EntityAssociationTargetEntityId]
GO

ALTER TABLE [dbo].[EntityAssociation] DROP CONSTRAINT [FK_EntityAssociationSourceEntityId]
GO

ALTER TABLE [dbo].[EntityAssociation] DROP CONSTRAINT [FK_EntityAssociationObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityAssociation] DROP CONSTRAINT [FK_EntityAssociationEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityAssociation] DROP CONSTRAINT [FK_EntityAssociationAssociationConceptId]
GO

ALTER TABLE [dbo].[EntityExtension] DROP CONSTRAINT [FK_EntityExtensionObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityExtension] DROP CONSTRAINT [FK_EntityExtensionExtensionTypeId]
GO

ALTER TABLE [dbo].[EntityExtension] DROP CONSTRAINT [FK_EntityExtensionEntityId]
GO

ALTER TABLE [dbo].[EntityExtension] DROP CONSTRAINT [FK_EntityExtensionEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[ConceptVersion] DROP CONSTRAINT [FK_ConceptVersionStatusConceptId]
GO

ALTER TABLE [dbo].[ConceptVersion] DROP CONSTRAINT [FK_ConceptVersionReplacesVersionId]
GO

ALTER TABLE [dbo].[ConceptVersion] DROP CONSTRAINT [FK_ConceptVersionObsoletedBy]
GO

ALTER TABLE [dbo].[ConceptVersion] DROP CONSTRAINT [FK_ConceptVersionCreatedBy]
GO

ALTER TABLE [dbo].[ConceptVersion] DROP CONSTRAINT [FK_ConceptVersionConceptClass]
GO

ALTER TABLE [dbo].[ConceptVersion] DROP CONSTRAINT [FK_ConceptVersionConcept]
GO

ALTER TABLE [dbo].[EntityVersion] DROP CONSTRAINT [FK_EntityVersionTypeConceptId]
GO

ALTER TABLE [dbo].[EntityVersion] DROP CONSTRAINT [FK_EntityVersionStatusConceptId]
GO

ALTER TABLE [dbo].[EntityVersion] DROP CONSTRAINT [FK_EntityVersionReplacesVersionId]
GO

ALTER TABLE [dbo].[EntityVersion] DROP CONSTRAINT [FK_EntityVersionObsoletedBy]
GO

ALTER TABLE [dbo].[EntityVersion] DROP CONSTRAINT [FK_EntityVersionEntityId]
GO

ALTER TABLE [dbo].[EntityVersion] DROP CONSTRAINT [FK_EntityVersionCreatedBy]
GO

ALTER TABLE [dbo].[EntityTag] DROP CONSTRAINT [FK_EntityTagObsoletedBy]
GO

ALTER TABLE [dbo].[EntityTag] DROP CONSTRAINT [FK_EntityTagEntityId]
GO

ALTER TABLE [dbo].[EntityTag] DROP CONSTRAINT [FK_EntityTagCreatedBy]
GO

ALTER TABLE [dbo].[EntityIdentifier] DROP CONSTRAINT [FK_EntityIdentifierObsoleteVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityIdentifier] DROP CONSTRAINT [FK_EntityIdentifierIdentifierTypeId]
GO

ALTER TABLE [dbo].[EntityIdentifier] DROP CONSTRAINT [FK_EntityIdentifierEntityId]
GO

ALTER TABLE [dbo].[EntityIdentifier] DROP CONSTRAINT [FK_EntityIdentifierEffectiveVersionSequenceId]
GO

ALTER TABLE [dbo].[EntityIdentifier] DROP CONSTRAINT [FK_EntityIdentifierAssigningAuthorityId]
GO

/****** Object:  Table [dbo].[EntityNote]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityNote]
GO

/****** Object:  Table [dbo].[ActExtension]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActExtension]
GO

/****** Object:  Table [dbo].[Act]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Act]
GO

/****** Object:  Table [dbo].[EntityAddressComponent]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityAddressComponent]
GO

/****** Object:  Table [dbo].[EntityAddressComponentValue]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityAddressComponentValue]
GO

/****** Object:  Table [dbo].[ActIdentifier]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActIdentifier]
GO

/****** Object:  Table [dbo].[PatientEncounter]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[PatientEncounter]
GO

/****** Object:  Table [dbo].[PersonLanguageCommunication]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[PersonLanguageCommunication]
GO

/****** Object:  Table [dbo].[PlaceService]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[PlaceService]
GO

/****** Object:  Table [dbo].[IdentifierType]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[IdentifierType]
GO

/****** Object:  Table [dbo].[Observation]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Observation]
GO

/****** Object:  Table [dbo].[Patient]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Patient]
GO

/****** Object:  Table [dbo].[ActRelationship]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActRelationship]
GO

/****** Object:  Table [dbo].[ActTag]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActTag]
GO

/****** Object:  Table [dbo].[ActVersion]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActVersion]
GO

/****** Object:  Table [dbo].[ActParticipation]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActParticipation]
GO

/****** Object:  Table [dbo].[ActPolicy]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActPolicy]
GO

/****** Object:  Table [dbo].[ActProtocol]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActProtocol]
GO

/****** Object:  Table [dbo].[ControlAct]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ControlAct]
GO

/****** Object:  Table [dbo].[DeviceEntity]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[DeviceEntity]
GO

/****** Object:  Table [dbo].[EntityAddress]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityAddress]
GO

/****** Object:  Table [dbo].[ApplicationEntity]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ApplicationEntity]
GO

/****** Object:  Table [dbo].[CodedObservation]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[CodedObservation]
GO

/****** Object:  Table [dbo].[ConceptRelationship]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ConceptRelationship]
GO

/****** Object:  Table [dbo].[Protocol]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Protocol]
GO

/****** Object:  Table [dbo].[StockBalance]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[StockBalance]
GO

/****** Object:  Table [dbo].[QuantifiedEntityAssociation]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[QuantifiedEntityAssociation]
GO

/****** Object:  Table [dbo].[QuantifiedActParticipation]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[QuantifiedActParticipation]
GO

/****** Object:  Table [dbo].[SecurityUserClaim]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityUserClaim]
GO

/****** Object:  Table [dbo].[SecurityUserLogins]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityUserLogins]
GO

/****** Object:  Table [dbo].[QuantityObservation]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[QuantityObservation]
GO

/****** Object:  Table [dbo].[StockLedger]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[StockLedger]
GO

/****** Object:  Table [dbo].[ActNote]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ActNote]
GO

/****** Object:  Table [dbo].[SecurityDevicePolicy]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityDevicePolicy]
GO

/****** Object:  Table [dbo].[ProtocolHandler]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ProtocolHandler]
GO

/****** Object:  Table [dbo].[TemplateDefinition]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[TemplateDefinition]
GO

/****** Object:  Table [dbo].[SubstanceAdministration]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SubstanceAdministration]
GO

/****** Object:  Table [dbo].[TextObservation]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[TextObservation]
GO

/****** Object:  Table [dbo].[ConceptRelationshipType]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ConceptRelationshipType]
GO

/****** Object:  Table [dbo].[SecurityUserRole]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityUserRole]
GO

/****** Object:  Table [dbo].[SecurityRole]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityRole]
GO

/****** Object:  Table [dbo].[SecurityRolePolicy]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityRolePolicy]
GO

/****** Object:  Table [dbo].[AssigningAuthority]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[AssigningAuthority]
GO

/****** Object:  Table [dbo].[UserEntity]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[UserEntity]
GO

/****** Object:  Table [dbo].[AssigningAuthorityScope]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[AssigningAuthorityScope]
GO

/****** Object:  Table [dbo].[SecurityUserClass]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityUserClass]
GO

/****** Object:  Table [dbo].[ConceptClass]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ConceptClass]
GO

/****** Object:  Table [dbo].[PhoneticAlgorithm]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[PhoneticAlgorithm]
GO

/****** Object:  Table [dbo].[Person]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Person]
GO

/****** Object:  Table [dbo].[SecurityApplication]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityApplication]
GO

/****** Object:  Table [dbo].[ExtensionType]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ExtensionType]
GO

/****** Object:  Table [dbo].[Organization]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Organization]
GO

/****** Object:  Table [dbo].[ManufacturedMaterial]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ManufacturedMaterial]
GO

/****** Object:  Table [dbo].[Provider]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Provider]
GO

/****** Object:  Table [dbo].[SecurityApplicationPolicy]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityApplicationPolicy]
GO

/****** Object:  Table [dbo].[SecurityDevice]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityDevice]
GO

/****** Object:  Table [dbo].[Policy]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Policy]
GO

/****** Object:  Table [dbo].[CodeSystem]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[CodeSystem]
GO

/****** Object:  Table [dbo].[Material]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Material]
GO

/****** Object:  Table [dbo].[ConceptSet]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ConceptSet]
GO

/****** Object:  Table [dbo].[Concept]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Concept]
GO

/****** Object:  Table [dbo].[EntityTelecomAddress]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityTelecomAddress]
GO

/****** Object:  Table [dbo].[Place]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Place]
GO

/****** Object:  Table [dbo].[SecurityUser]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[SecurityUser]
GO

/****** Object:  Table [dbo].[ReferenceTermDisplayName]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ReferenceTermDisplayName]
GO

/****** Object:  Table [dbo].[ConceptSetMember]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ConceptSetMember]
GO

/****** Object:  Table [dbo].[PhoneticValues]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[PhoneticValues]
GO

/****** Object:  Table [dbo].[Entity]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[Entity]
GO

/****** Object:  Table [dbo].[ConceptReferenceTerm]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ConceptReferenceTerm]
GO

/****** Object:  Table [dbo].[EntityNameComponent]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityNameComponent]
GO

/****** Object:  Table [dbo].[EntityName]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityName]
GO

/****** Object:  Table [dbo].[ConceptName]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ConceptName]
GO

/****** Object:  Table [dbo].[ReferenceTerm]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ReferenceTerm]
GO

/****** Object:  Table [dbo].[EntityAssociation]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityAssociation]
GO

/****** Object:  Table [dbo].[EntityExtension]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityExtension]
GO

/****** Object:  Table [dbo].[ConceptVersion]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[ConceptVersion]
GO

/****** Object:  Table [dbo].[EntityVersion]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityVersion]
GO

/****** Object:  Table [dbo].[EntityTag]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityTag]
GO

/****** Object:  Table [dbo].[EntityIdentifier]    Script Date: 8/16/2016 11:54:25 PM ******/
DROP TABLE [dbo].[EntityIdentifier]
GO

/****** Object:  View [dbo].[ConceptView]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[ConceptView]
GO

/****** Object:  View [dbo].[ConceptSetMembersView]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[ConceptSetMembersView]
GO

/****** Object:  View [dbo].[MaterialCurrentVersion]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[MaterialCurrentVersion]
GO

/****** Object:  View [dbo].[UserEntityCurrentVersion]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[UserEntityCurrentVersion]
GO

/****** Object:  View [dbo].[ProviderCurrentVersion]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[ProviderCurrentVersion]
GO

/****** Object:  View [dbo].[PersonCurrentVersion]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[PersonCurrentVersion]
GO

/****** Object:  View [dbo].[PlaceCurrentVersion]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[PlaceCurrentVersion]
GO

/****** Object:  View [dbo].[EntityCurrentVersion]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[EntityCurrentVersion]
GO

/****** Object:  View [dbo].[EntityTelecomAddressValue]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[EntityTelecomAddressValue]
GO

/****** Object:  View [dbo].[EntityAddressValue]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[EntityAddressValue]
GO

/****** Object:  View [dbo].[EntityNameValue]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[EntityNameValue]
GO

/****** Object:  View [dbo].[ConceptCurrentVersion]    Script Date: 8/16/2016 11:55:44 PM ******/
DROP VIEW [dbo].[ConceptCurrentVersion]
GO

/****** Object:  StoredProcedure [dbo].[sp_Authenticate]    Script Date: 8/16/2016 11:56:07 PM ******/
DROP PROCEDURE [dbo].[sp_Authenticate]
GO


/****** Object:  UserDefinedFunction [dbo].[fn_OpenIzSchemaVersion]    Script Date: 8/16/2016 11:56:23 PM ******/
DROP FUNCTION [dbo].[fn_OpenIzSchemaVersion]
GO

/****** Object:  UserDefinedFunction [dbo].[fn_IsConceptSetMember]    Script Date: 8/16/2016 11:56:23 PM ******/
DROP FUNCTION [dbo].[fn_IsConceptSetMember]
GO

/****** Object:  UserDefinedFunction [dbo].[fn_IsAccountLocked]    Script Date: 8/16/2016 11:56:23 PM ******/
DROP FUNCTION [dbo].[fn_IsAccountLocked]
GO

/****** Object:  UserDefinedFunction [dbo].[fn_AssertEntityClass]    Script Date: 8/16/2016 11:56:23 PM ******/
DROP FUNCTION [dbo].[fn_AssertEntityClass]
GO

/****** Object:  UserDefinedFunction [dbo].[fn_AssertConceptClass]    Script Date: 8/16/2016 11:56:23 PM ******/
DROP FUNCTION [dbo].[fn_AssertConceptClass]
GO


/****** Object:  Sequence [dbo].[EntityVersionSequenceId]    Script Date: 8/16/2016 11:56:59 PM ******/
DROP SEQUENCE [dbo].[EntityVersionSequenceId]
GO

USE [OpenIZ]
GO

/****** Object:  Sequence [dbo].[ConceptVersionSequence]    Script Date: 8/16/2016 11:56:59 PM ******/
DROP SEQUENCE [dbo].[ConceptVersionSequence]
GO

USE [OpenIZ]
GO

/****** Object:  Sequence [dbo].[ActVersionSequenceId]    Script Date: 8/16/2016 11:56:59 PM ******/
DROP SEQUENCE [dbo].[ActVersionSequenceId]
GO
