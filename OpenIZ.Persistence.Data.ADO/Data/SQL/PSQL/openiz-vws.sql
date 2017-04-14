/*
 * OPENIZ DATA VIEWS
 */

-- VIEW FOR CURRENT VERSIONS OF ENTITIES
CREATE OR REPLACE VIEW ENT_CUR_VRSN_VW AS
	SELECT * FROM ENT_VRSN_TBL INNER JOIN ENT_TBL USING (ENT_ID)
	WHERE OBSLT_UTC IS NULL;

-- VIEW FOR CURRENT VERSION OF PERSONS
CREATE OR REPLACE VIEW PSN_CUR_VRSN_VW AS
	SELECT ENT_CUR_VRSN_VW.*, PSN_TBL.DOB, PSN_TBL.DOB_PREC, STS_CD.MNEMONIC AS STS_CS FROM ENT_CUR_VRSN_VW INNER JOIN PSN_TBL USING (ENT_VRSN_ID)
	INNER JOIN CD_CUR_VRSN_VW AS STS_CD on (ENT_CUR_VRSN_VW.STS_CD_ID = STS_CD.CD_ID);
	
-- VIEW FOR CURRENT VERSION OF PATIENTS
CREATE OR REPLACE VIEW PAT_CUR_VRSN_VW AS 
	SELECT PSN_CUR_VRSN_VW.*, PAT_TBL.GNDR_CD_ID, PAT_TBL.DCSD_UTC, PAT_TBL.DCSD_PREC, PAT_TBL.MB_ORD, GNDR_CD.MNEMONIC AS GNDR_CS FROM PSN_CUR_VRSN_VW INNER JOIN PAT_TBL USING (ENT_VRSN_ID)
	LEFT JOIN CD_CUR_VRSN_VW AS GNDR_CD on (PAT_TBL.GNDR_CD_ID = GNDR_CD.CD_ID);

-- VIEW FOR CURRENT VERSION OF PROVIDERS
CREATE OR REPLACE VIEW PVDR_CUR_VRSN_VW AS
	SELECT * FROM PSN_CUR_VRSN_VW INNER JOIN PVDR_TBL USING (ENT_VRSN_ID);

-- VIEW FOR CURRENT VERSION OF MATERIALS
CREATE OR REPLACE VIEW MAT_CUR_VRSN_VW AS 
	SELECT * FROM ENT_CUR_VRSN_VW INNER JOIN MAT_TBL USING (ENT_VRSN_ID);

-- VIEW FOR CURRENT VERSION OF MANUFACTURED MATERIALS
CREATE OR REPLACE VIEW MMAT_CUR_VRSN_VW AS
	SELECT * FROM MAT_CUR_VRSN_VW INNER JOIN MMAT_TBL USING (ENT_VRSN_ID);

-- VIEW FOR CURRENT VERSION OF PLACES
CREATE OR REPLACE VIEW PLC_CUR_VRSN_VW AS
	SELECT * FROM ENT_CUR_VRSN_VW INNER JOIN PLC_TBL USING (ENT_VRSN_ID);

-- VIEW FOR CURRENT VERSION OF USERS
CREATE OR REPLACE VIEW USR_CUR_VRSN_VW AS
	SELECT PSN_CUR_VRSN_VW.*, SEC_USR_TBL.USR_NAME 
		FROM PSN_CUR_VRSN_VW INNER JOIN USR_ENT_TBL USING (ENT_VRSN_ID)
		INNER JOIN SEC_USR_TBL ON (USR_ID = SEC_USR_ID);

-- VIEW FOR ENTITY NAMES
CREATE OR REPLACE VIEW ent_cur_name_vw as 
select
	ent_name_tbl.name_id,
	ent_name_tbl.ent_id, 
	coalesce(use_cd.mnemonic, 'Other') as use, 
	coalesce(typ_cd.mnemonic, 'Other') as typ, 
	array_to_string(array_agg(phon_val_tbl.val), ' ') as val
from ent_name_tbl inner join ent_name_cmp_tbl using (name_id) 
	inner join phon_val_tbl using (val_id) 
	left join cd_cur_vrsn_vw as use_cd on (use_cd_id = use_cd.cd_id)
	left join cd_cur_vrsn_vw as typ_cd on (typ_cd_id = typ_cd.cd_id) 
where obslt_vrsn_seq_id is null 
group by ent_name_tbl.name_id, ent_id, use, typ;

-- VIEW FOR ADDRESSES
CREATE OR REPLACE VIEW ent_cur_addr_vw as 
select
	ent_addr_tbl.addr_id,
	ent_addr_tbl.ent_id, 
	coalesce(use_cd.mnemonic, 'Other') as use, 
	coalesce(typ_cd.mnemonic, 'Other') as typ, 
	array_to_string(array_agg(ent_addr_cmp_val_tbl.val), ' ') as val
from ent_addr_tbl inner join ent_addr_cmp_tbl using (addr_id) 
	inner join ent_addr_cmp_val_tbl using (val_id) 
	left join cd_cur_vrsn_vw as use_cd on (use_cd_id = use_cd.cd_id)
	left join cd_cur_vrsn_vw as typ_cd on (typ_cd_id = typ_cd.cd_id) 
where obslt_vrsn_seq_id is null 
group by ent_addr_tbl.addr_id, ent_id, use, typ ;

-- ENTITY CURRENT ADDRESS VIEW
CREATE OR REPLACE VIEW ENT_CUR_TEL_VW AS 
	SELECT ENT_ID, TEL_VAL, MNEMONIC AS USE_CS FROM 
		ENT_TEL_TBL INNER JOIN CD_CUR_VRSN_VW AS USE_CD ON (USE_CD.CD_ID = ENT_TEL_TBL.USE_CD_ID) WHERE OBSLT_VRSN_SEQ_ID IS NULL;

-- ALTERNATE IDENTIIFERS
CREATE OR REPLACE VIEW ENT_CUR_ID_VW AS 
	SELECT ent_id_tbl.ent_id, id_val, aut_name, oid, nsid FROM 
		ENT_ID_TBL INNER JOIN ASGN_AUT_TBL USING(AUT_ID)
		WHERE ENT_ID_TBL.OBSLT_VRSN_SEQ_ID IS NULL;