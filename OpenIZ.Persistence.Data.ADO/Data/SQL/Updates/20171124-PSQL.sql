/** 
 * <update id="20171123-01" applyRange="0.9.0.0-0.9.0.11"  invariantName="npgsql">
 *	<summary>Add entity relationship validation for providers and state/county</summary>
 *	<remarks></remarks>
 *	<isInstalled>select ck_patch('20171123-01')</isInstalled>
 * </update>
 */

BEGIN TRANSACTION ;

-- RULE 14: PROVIDERS CAN BE HEALTHCARE PROVIDERS
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('77b7a04b-c065-4faf-8ec0-2cdad4ae372b', '9de2a846-ddf2-4ebc-902e-84508c5089ea', '6b04fed8-c164-469c-910b-f824c2bda4f0', 'err_healthCareProviders_PersonsOnly');

-- RULE 15: NOK IS BETWEEN PATIENTS AND PERSONS
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('1ee4e74f-542d-4544-96f6-266a6247f274', 'bacd9c6f-3fa9-481e-9636-37457962804d', '9de2a846-ddf2-4ebc-902e-84508c5089ea', 'err_nextOfKin_PersonsOnly');

-- RULE 16: DEDICATED SDL AND STATE / COUNTY
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455f1772-f580-47e8-86bd-b5ce25d351f9', '8cf4b0b0-84e5-4122-85fe-6afa8240c218', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_stateDedicatedSDL');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455f1772-f580-47e8-86bd-b5ce25d351f9', 'd9489d56-ddac-4596-b5c6-8f41d73d8dc5', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_stateDedicatedSDL');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455f1772-f580-47e8-86bd-b5ce25d351f9', '48b2ffb3-07db-47ba-ad73-fc8fb8502471', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_stateDedicatedSDL');

SELECT REG_PATCH('20171123-01');

COMMIT;