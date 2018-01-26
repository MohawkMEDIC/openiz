/** 
 * <update id="20171108-01" applyRange="0.9.0.0-1.0.0.0"  invariantName="npgsql">
 *	<summary>Add relationship types between patients and locations</summary>
 *	<remarks></remarks>
 *	<check>select ck_patch('20171108-01')</check>
 * </update>
 */

BEGIN TRANSACTION ;
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('455F1772-F580-47E8-86BD-B5CE25D351F9', '4d1a5c28-deb7-411e-b75f-d524f90dfa63', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');

-- RULE 10. -> SDL IS VALID BETWEEN PATIENTS AND LOCATIONS
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'bacd9c6f-3fa9-481e-9636-37457962804d', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('41baf7aa-5ffd-4421-831f-42d4ab3de38a', 'bacd9c6f-3fa9-481e-9636-37457962804d', 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c', 'err_organization_manufactures_manufacturedMaterialOnly');

-- RULE 12. -> Materials may use other Materials
INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('08fff7d9-bac7-417b-b026-c9bee52f4a37', 'd39073be-0f8f-440e-b8c8-7034cc138a95', 'd39073be-0f8f-440e-b8c8-7034cc138a95', 'err_materials_associate_materials');

SELECT REG_PATCH('20171108-01');


COMMIT;