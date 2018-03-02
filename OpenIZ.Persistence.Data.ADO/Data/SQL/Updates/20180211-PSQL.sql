/** 
 * <update id="20180211-01" applyRange="1.1.0.0-1.2.0.0"  invariantName="npgsql">
 *	<summary>Add relationship "replaces" between all entities of the same class</summary>
 *	<remarks>Any entity is technically allowed to replace itself :)</remarks>
 *	<check>select ck_patch('20180211-01')</check>
 * </update>
 */

BEGIN TRANSACTION ;

INSERT INTO ent_rel_vrfy_cdtbl (rel_typ_cd_id, src_cls_cd_id, trg_cls_cd_id, err_desc) VALUES ('C6B92576-1D62-4896-8799-6F931F8AB607', '7C08BD55-4D42-49CD-92F8-6388D6C4183F', '21AB7873-8EF3-4D78-9C19-4582B3C40631', 'err_stateDedicatedSDL');

ALTER TABLE ent_rel_vrfy_cdtbl ALTER COLUMN err_desc TYPE VARCHAR(128);

UPDATE ENT_REL_VRFY_CDTBL 
	SET err_desc = (
		SELECT SRC.MNEMONIC || ' ==[' || TYP.MNEMONIC || ']==> ' || TRG.MNEMONIC 
		FROM 
			ENT_REL_VRFY_CDTBL VFY
			INNER JOIN CD_VRSN_TBL TYP ON (REL_TYP_CD_ID = TYP.CD_ID)
			INNER JOIN CD_VRSN_TBL SRC ON (SRC_CLS_CD_ID = SRC.CD_ID)
			INNER JOIN CD_VRSN_TBL TRG ON (TRG_CLS_CD_ID = TRG.CD_ID)
		WHERE 
			VFY.ENT_REL_VRFY_ID = ENT_REL_VRFY_CDTBL.ENT_REL_VRFY_ID
		FETCH FIRST 1 ROWS ONLY
	);

SELECT REG_PATCH('20180211-01');
COMMIT;
