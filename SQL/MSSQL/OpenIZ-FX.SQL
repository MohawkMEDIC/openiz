-- OPEN IZ DATABASE STORED PROCEDURES
-- COPYRIGHT (c) 2015, OPEN IZ
-- PORTIONS COPYRIGHT (C) 2015, FYFE SOFTWARE INC.
-- LICENSED UNDER THE APACHE 2.0 LICENSE

USE OpenIZ;

/*
 * Creates a concept class scheme
 */
CREATE PROCEDURE sp_CreateConceptClass (@Name NVARCHAR(50), @Mnemonic NVARCHAR(16), @ClassId NUMERIC OUTPUT)
AS BEGIN
	INSERT INTO ConceptClass (ConceptClassId, Name, Mnemonic) VALUES (@ClassId, @Name, @Mnemonic);
	SET @ClassId = SCOPE_IDENTITY();
END;

GO 

/*
 * Creates a concept (new concept and new version of that concept) returning the concept identifier and concept version created
 */
CREATE PROCEDURE sp_CreateConcept(@IsSystemConcept BIT, @CreatedBy UNIQUEIDENTIFIER, @ClassId NUMERIC, @ConceptId NUMERIC OUTPUT, @ConceptVersionId NUMERIC OUTPUT)
AS BEGIN
	INSERT INTO Concept (IsSystemConcept) VALUES (@IsSystemConcept);
	SET @ConceptId =  SCOPE_IDENTITY();
	INSERT INTO ConceptVersion (ConceptId, CreatedBy, ConceptClassId) VALUES (@ConceptId, @CreatedBy, @ClassId);
	SET @ConceptVersionId = SCOPE_IDENTITY();
END;

GO

/*
 * Create a concept name assigned to the current version of the concept
 */
CREATE PROCEDURE sp_CreateConceptName(@ConceptId NUMERIC, @LanguageCode CHAR(2), @Name NVARCHAR(256), @PhoneticCode NVARCHAR(32), @PhoneticAlgorithmId NUMERIC(2), @ConceptNameId NUMERIC OUTPUT)
AS 
DECLARE
	@ConceptVersionId NUMERIC(15);
BEGIN
	SET @ConceptVersionId = (SELECT ConceptVersionId FROM ConceptCurrentVersion WHERE ConceptId = @ConceptId);
	INSERT INTO ConceptName (EffectiveVersionId, LanguageCode, Name, PhoneticCode, PhoneticAlgorithmId) VALUES
		(@ConceptVersionId, @LanguageCode, @Name, COALESCE(@PhoneticCode, SOUNDEX(@Name)), COALESCE(@PhoneticAlgorithmId, 1));
	SET @ConceptNameId = SCOPE_IDENTITY();

END;

GO 
