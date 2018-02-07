/** 
 * <update id="20180126-01" applyRange="1.0.0.0-1.0.0.0" invariantName="npgsql">
 *	<summary>Fixes trigger to allow obsoletion relations to exist</summary>
 *	<remarks>This is necessary to correct older openiz databases which already had bad data in them</remarks>
 *	<isInstalled>select ck_patch('20180126-01')</isInstalled>
 * </update>
 */
BEGIN TRANSACTION;

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
	) AND NEW.obslt_vrsn_seq_id IS NULL THEN
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

SELECT REG_PATCH('20180126-01');

COMMIT;