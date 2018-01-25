/** 
 * <update id="20171011-01" applyRange="0.2.0.4-0.9.0.5"  invariantName="npgsql">
 *	<summary>Adds trigger constraints to ensure that relationships are of proper type</summary>
 *	<remarks></remarks>
 *	<isInstalled>select ck_patch('20171011-01')</isInstalled>
 * </update>
 */

BEGIN TRANSACTION ;
-- VERIFY RULES FOR ENTITY RELATIONSHIP CHECKING
CREATE TABLE IF NOT EXISTS ent_rel_vrfy_cdtbl (
	ent_rel_vrfy_id UUID NOT NULL DEFAULT uuid_generate_V4(),
	rel_typ_cd_id UUID NOT NULL, -- THE TYPE OF RELATIONSHIP
	src_cls_cd_id UUID NOT NULL, -- THE CLASS CODE OF THE SOURCE ENTITY
	trg_cls_cd_id UUID NOT NULL, -- THE CLASS CODE OF THE TARGET ENTITY
	err_desc VARCHAR(64) NOT NULL, -- THE ERROR CONDITION
	CONSTRAINT pk_ent_rel_vrfy_cdtbl PRIMARY KEY (ent_rel_vrfy_id),
	CONSTRAINT fk_ent_rel_vrfy_rel_typ_cd FOREIGN KEY (rel_typ_cd_id) REFERENCES cd_tbl(cd_id),
	CONSTRAINT fk_ent_rel_vrfy_src_cls_cd FOREIGN KEY (src_cls_cd_id) REFERENCES cd_tbl(cd_id),
	CONSTRAINT fk_ent_rel_vrfy_trg_cls_cd FOREIGN KEY (trg_cls_cd_id) REFERENCES cd_tbl(cd_id)
);

CREATE UNIQUE INDEX ent_rel_vrfy_src_trg_unq ON ent_rel_vrfy_cdtbl(rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id);

-- RULE 1. -> SERVICE DELIVERY LOCATIONS CAN ONLY HAVE SERVICE DELIVERY LOCATIONS FOR PARENTS
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_sdl_parent_sdlOnly');

-- RULE 2. -> NON-SERVICE DELIVERY LOCATIONS CAN ONLY HAVE NON-SERVICE DELIVERY LOCATIONS FOR PARENTS
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '79dd4f75-68e8-4722-a7f5-8bc2e08f5cd6', '21ab7873-8ef3-4d78-9c19-4582b3c40631', 'err_place_parent_placeOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '48b2ffb3-07db-47ba-ad73-fc8fb8502471', '21ab7873-8ef3-4d78-9c19-4582b3c40631', 'err_place_parent_placeOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', 'd9489d56-ddac-4596-b5c6-8f41d73d8dc5', '21ab7873-8ef3-4d78-9c19-4582b3c40631', 'err_place_parent_placeOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '8cf4b0b0-84e5-4122-85fe-6afa8240c218', '21ab7873-8ef3-4d78-9c19-4582b3c40631', 'err_place_parent_placeOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '21ab7873-8ef3-4d78-9c19-4582b3c40631', '21ab7873-8ef3-4d78-9c19-4582b3c40631', 'err_place_parent_placeOnly');

-- RULE 3. -> STATES CAN ONLY HAVE COUNTRY AS PARENT
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '8cf4b0b0-84e5-4122-85fe-6afa8240c218', '48b2ffb3-07db-47ba-ad73-fc8fb8502471', 'err_state_parent_countryOnly');

-- RULE 4. -> COUNTY CAN ONLY HAVE STATE AS PARENT
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', 'd9489d56-ddac-4596-b5c6-8f41d73d8dc5', '8cf4b0b0-84e5-4122-85fe-6afa8240c218', 'err_county_parent_stateOnly');

-- RULE 5. -> CITY CAN ONLY HAVE COUNTY OR STATE AS PARENT
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '79dd4f75-68e8-4722-a7f5-8bc2e08f5cd6', '8cf4b0b0-84e5-4122-85fe-6afa8240c218', 'err_city_parent_stateOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '79dd4f75-68e8-4722-a7f5-8bc2e08f5cd6', 'd9489d56-ddac-4596-b5c6-8f41d73d8dc5', 'err_city_parent_countyOnly');

-- RULE 6. -> PLACES CAN HAVE ANY PLACE AS PARENT
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '21ab7873-8ef3-4d78-9c19-4582b3c40631', '8cf4b0b0-84e5-4122-85fe-6afa8240c218', 'err_place_parent_placeOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '21ab7873-8ef3-4d78-9c19-4582b3c40631', 'd9489d56-ddac-4596-b5c6-8f41d73d8dc5', 'err_place_parent_placeOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '21ab7873-8ef3-4d78-9c19-4582b3c40631', '48b2ffb3-07db-47ba-ad73-fc8fb8502471', 'err_place_parent_placeOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('BFCBB345-86DB-43BA-B47E-E7411276AC7C', '21ab7873-8ef3-4d78-9c19-4582b3c40631', '79dd4f75-68e8-4722-a7f5-8bc2e08f5cd6', 'err_place_parent_placeOnly');

-- RULE 7. -> MATERIALS CAN ONLY HAVE MANUFACTURED MATERIALS AS INSTANCE
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('AC45A740-B0C7-4425-84D8-B3F8A41FEF9F', 'd39073be-0f8f-440e-b8c8-7034cc138a95', 'fafec286-89d5-420b-9085-054aca9d1eef', 'err_material_instance_manufacturedMaterialOnly');

-- RULE 8. -> ONLY ORGANIZATIONS CAN MANUFACTURE MATERIALS
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('6780DF3B-AFBD-44A3-8627-CBB3DC2F02F6', '7c08bd55-4d42-49cd-92f8-6388d6c4183f', 'fafec286-89d5-420b-9085-054aca9d1eef', 'err_organization_manufactures_manufacturedMaterialOnly');

-- RULE 9. -> ONLY SDLS CAN BE DEDICATED SERVICE DELIVERY LOCATIONS
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', '9de2a846-ddf2-4ebc-902e-84508c5089ea', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', '6b04fed8-c164-469c-910b-f824c2bda4f0', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', 'bacd9c6f-3fa9-481e-9636-37457962804d', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', '8ba5e5c9-693b-49d4-973c-d7010f3a23ee', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', 'e29fcfad-ec1d-4c60-a055-039a494248ae', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', '61fcbf42-b5e0-4fb5-9392-108a5c6dbec7', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', '21ab7873-8ef3-4d78-9c19-4582b3c40631', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', '79dd4f75-68e8-4722-a7f5-8bc2e08f5cd6', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');

-- RULE 10. -> ONLY SDLS CAN OWN STOCK
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('117da15c-0864-4f00-a987-9b9854cba44e', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'fafec286-89d5-420b-9085-054aca9d1eef', 'err_organization_manufactures_manufacturedMaterialOnly');

-- RULE 11. -> ONLY PERSONS OR PATIENTS CAN BE MOTHERS OR NEXT OF KIN
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc)
	SELECT cd_id, 'bacd9c6f-3fa9-481e-9636-37457962804d', '9de2a846-ddf2-4ebc-902e-84508c5089ea', 'err_patient_nok_personOnly'
	FROM cd_set_mem_vw
	WHERE set_mnemonic = 'FamilyMember';
	
-- TRIGGER - ENSURE THAT ANY VALUE INSERTED INTO THE ENT_REL_TBL HAS THE PROPER PARENT
CREATE OR REPLACE FUNCTION trg_vrfy_ent_rel_tbl () RETURNS TRIGGER AS $$
DECLARE 
	err_ref varchar(128)[];
	
BEGIN
	IF NOT EXISTS (
		SELECT * 
		FROM 
			ent_rel_vrfy_cdtbl 
			INNER JOIN ent_tbl src_ent ON (src_ent.ent_id = NEW.src_ent_id)
			INNER JOIN ent_tbl trg_ent ON (trg_ent.ent_id = NEW.trg_ent_id)
		WHERE 
			rel_typ_cd_id = NEW.rel_typ_cd_id 
			AND src_cls_cd_id = src_ent.cls_cd_id 
			AND trg_cls_cd_id = trg_ent.cls_cd_id
	) THEN
		SELECT DISTINCT 
			('{' || rel_cd.mnemonic || ',' || src_cd.mnemonic || ',' || trg_cd.mnemonic || '}')::VARCHAR[] INTO err_ref
		FROM 
			ent_tbl src_ent 
			CROSS JOIN ent_tbl trg_ent
			CROSS JOIN CD_VRSN_TBL REL_CD
			LEFT JOIN CD_VRSN_TBL SRC_CD ON (SRC_ENT.CLS_CD_ID = SRC_CD.CD_ID)
			LEFT JOIN CD_VRSN_TBL TRG_CD ON (TRG_ENT.CLS_CD_ID = TRG_CD.CD_ID)
		WHERE
			src_ent.ent_id = NEW.src_ent_id
			AND trg_ent.ent_id = NEW.trg_ent_id
			AND REL_CD.CD_ID = NEW.REL_TYP_CD_ID;

		IF err_ref[1] IS NULL OR err_ref[2] IS NULL OR err_ref[3] IS NULL THEN
			RETURN NEW; -- LET THE FK WORK
		ELSE 
			RAISE EXCEPTION 'Validation error: Relationship % [%] between % [%] > % [%] is invalid', NEW.rel_typ_cd_id, err_ref[1], NEW.src_ent_id, err_ref[2], NEW.trg_ent_id, err_ref[3]
				USING ERRCODE = 'O9001';
		END IF;
	END IF;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- TRIGGER
CREATE TRIGGER ent_rel_tbl_vrfy BEFORE INSERT OR UPDATE ON ent_rel_tbl
	FOR EACH ROW EXECUTE PROCEDURE trg_vrfy_ent_rel_tbl();

  -- CHECK CONSTRAINT FUNCTION WHICH WILL RAISE MORE DESCRIPTIVE EXCEPTION
 CREATE OR REPLACE FUNCTION ck_is_cd_set_mem ( cd_id_in IN UUID, set_mnemonic_in IN VARCHAR, allow_null_in IN BOOLEAN ) RETURNS BOOLEAN AS $$
 BEGIN 
	IF IS_CD_SET_MEM(cd_id_in, set_mnemonic_in) OR allow_null_in AND IS_CD_SET_MEM(cd_id_in, 'NullReason') THEN
		RETURN TRUE;
	ELSE 
		RAISE EXCEPTION 'Codification Error: Concept % is not in set %', cd_id_in, set_mnemonic_in
			USING ERRCODE = 'O9002';
	END IF;
 END;
 $$ LANGUAGE plpgsql;

 -- UPGRADE CONSTRAINTS - THIS MAY TAKE SOME TIME
 ALTER TABLE CD_VRSN_TBL DROP CONSTRAINT IF EXISTS  CK_CD_VRSN_STS_CD_ID;
 ALTER TABLE CD_VRSN_TBL ADD CONSTRAINT CK_CD_VRSN_STS_CD_ID CHECK (CK_IS_CD_SET_MEM(STS_CD_ID, 'ConceptStatus', FALSE));
	
 ALTER TABLE ACT_VRSN_TBL DROP CONSTRAINT IF EXISTS  CK_ACT_VRSN_STS_CD;
 ALTER TABLE ACT_VRSN_TBL ADD CONSTRAINT CK_ACT_VRSN_STS_CD CHECK (CK_IS_CD_SET_MEM(STS_CD_ID, 'ActStatus', FALSE));
	
 ALTER TABLE ENT_VRSN_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_VRSN_STS_CD;
 ALTER TABLE ENT_VRSN_TBL ADD CONSTRAINT CK_ENT_VRSN_STS_CD CHECK (CK_IS_CD_SET_MEM(STS_CD_ID, 'EntityStatus', FALSE));

 ALTER TABLE ACT_TBL DROP CONSTRAINT IF EXISTS  CK_ACT_CLS_CD;
 ALTER TABLE ACT_TBL DROP CONSTRAINT IF EXISTS  CK_ACT_MOD_CD;
 ALTER TABLE ACT_TBL ADD CONSTRAINT CK_ACT_CLS_CD CHECK (CK_IS_CD_SET_MEM(CLS_CD_ID, 'ActClass', FALSE));
 ALTER TABLE ACT_TBL ADD CONSTRAINT CK_ACT_MOD_CD CHECK (CK_IS_CD_SET_MEM(MOD_CD_ID, 'ActMood', FALSE));
 
 ALTER TABLE ACT_VRSN_TBL DROP CONSTRAINT IF EXISTS  CK_ACT_VRSN_RSN_CD;
 ALTER TABLE ACT_VRSN_TBL ADD CONSTRAINT CK_ACT_VRSN_RSN_CD CHECK (RSN_CD_ID IS NULL OR CK_IS_CD_SET_MEM(RSN_CD_ID, 'ActReason', TRUE));

 ALTER TABLE ACT_REL_TBL DROP CONSTRAINT IF EXISTS FK_ACT_REL_REL_TYP_CD;
 ALTER TABLE ACT_REL_TBL DROP CONSTRAINT IF EXISTS CK_ACT_REL_REL_TYP_CD;
 ALTER TABLE ACT_REL_TBL ADD CONSTRAINT CK_ACT_REL_REL_TYP_CD CHECK (CK_IS_CD_SET_MEM(REL_TYP_CD_ID, 'ActRelationshipType', TRUE));
 
 ALTER TABLE OBS_TBL DROP CONSTRAINT IF EXISTS  CK_OBS_INT_CD;
 ALTER TABLE OBS_TBL ADD CONSTRAINT CK_OBS_INT_CD CHECK (INT_CD_ID IS NULL OR CK_IS_CD_SET_MEM(INT_CD_ID, 'ActInterpretation', TRUE));

 ALTER TABLE ENT_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_CLS_CD;
 ALTER TABLE ENT_TBL ADD CONSTRAINT CK_ENT_CLS_CD CHECK (CK_IS_CD_SET_MEM(CLS_CD_ID, 'EntityClass', FALSE));

 ALTER TABLE ENT_REL_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_REL_REL_TYPE_CD;
 ALTER TABLE ENT_REL_TBL ADD CONSTRAINT CK_ENT_REL_REL_TYPE_CD CHECK (CK_IS_CD_SET_MEM(REL_TYP_CD_ID, 'EntityRelationshipType', FALSE));

 ALTER TABLE ENT_ADDR_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_ADDR_USE_CD;
 ALTER TABLE ENT_ADDR_TBL ADD CONSTRAINT CK_ENT_ADDR_USE_CD CHECK (CK_IS_CD_SET_MEM(USE_CD_ID, 'AddressUse', FALSE));

 ALTER TABLE ENT_ADDR_CMP_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_ADDR_CMP_TYP_CD;
 ALTER TABLE ENT_ADDR_CMP_TBL ADD CONSTRAINT CK_ENT_ADDR_CMP_TYP_CD CHECK (TYP_CD_ID IS NULL OR CK_IS_CD_SET_MEM(TYP_CD_ID, 'AddressComponentType', FALSE));

 ALTER TABLE ENT_NAME_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_NAME_USE_CD;
 ALTER TABLE ENT_NAME_TBL ADD CONSTRAINT CK_ENT_NAME_USE_CD CHECK (CK_IS_CD_SET_MEM(USE_CD_ID, 'NameUse', FALSE));

 ALTER TABLE ENT_NAME_CMP_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_NAME_CMP_TYP_CD;
 ALTER TABLE ENT_NAME_CMP_TBL ADD CONSTRAINT CK_ENT_NAME_CMP_TYP_CD CHECK (TYP_CD_ID IS NULL OR CK_IS_CD_SET_MEM(TYP_CD_ID, 'NameComponentType', FALSE));

 ALTER TABLE ENT_TEL_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_TEL_TYP_CD;
 ALTER TABLE ENT_TEL_TBL ADD CONSTRAINT CK_ENT_TEL_TYP_CD CHECK (TYP_CD_ID IS NULL OR CK_IS_CD_SET_MEM(TYP_CD_ID, 'TelecomAddressType', TRUE));

 ALTER TABLE ENT_TEL_TBL DROP CONSTRAINT IF EXISTS  CK_ENT_TEL_USE_CD;
 ALTER TABLE ENT_TEL_TBL ADD CONSTRAINT CK_ENT_TEL_USE_CD CHECK (CK_IS_CD_SET_MEM(USE_CD_ID, 'TelecomAddressUse', FALSE));

 ALTER TABLE PLC_SVC_TBL DROP CONSTRAINT IF EXISTS  CK_PLC_SVC_CD;
 ALTER TABLE PLC_SVC_TBL ADD CONSTRAINT CK_PLC_SVC_CD CHECK (CK_IS_CD_SET_MEM(SVC_CD_ID, 'ServiceCode', FALSE));

 ALTER TABLE ORG_TBL DROP CONSTRAINT IF EXISTS  CK_ORG_IND_CD;
 ALTER TABLE ORG_TBL ADD CONSTRAINT CK_ORG_IND_CD CHECK (IND_CD_ID IS NULL OR CK_IS_CD_SET_MEM(IND_CD_ID, 'IndustryCode', TRUE)); 

 ALTER TABLE PAT_TBL DROP CONSTRAINT IF EXISTS  CK_PAT_GNDR_CD;
 ALTER TABLE PAT_TBL ADD CONSTRAINT CK_PAT_GNDR_CD CHECK (CK_IS_CD_SET_MEM(GNDR_CD_ID, 'AdministrativeGenderCode', TRUE));

 ALTER TABLE ACT_PTCPT_TBL DROP CONSTRAINT IF EXISTS  CK_ACT_PTCPT_ROL_CD;
 ALTER TABLE ACT_PTCPT_TBL ADD 	CONSTRAINT CK_ACT_PTCPT_ROL_CD CHECK (CK_IS_CD_SET_MEM(ROL_CD_ID, 'ActParticipationType', TRUE));
 
 
 
 -- GET THE SCHEMA VERSION
CREATE OR REPLACE FUNCTION GET_SCH_VRSN() RETURNS VARCHAR(10) AS
$$
BEGIN
	RETURN '0.9.0.6';
END;
$$ LANGUAGE plpgsql;

SELECT REG_PATCH('20171011-01');

COMMIT;