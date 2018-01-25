/** 
 * <update id="20171003-01" applyRange="0.2.0.4-0.9.0.4"  invariantName="npgsql">
 *	<summary>Adds several performance enhancing indexes</summary>
 *	<remarks></remarks>
 *	<isInstalled>select ck_patch('20171003-01')</isInstalled>
 * </update>
 */

 BEGIN TRANSACTION ;

CREATE INDEX ent_addr_cmp_typ_cd_idx ON ent_addr_cmp_tbl(typ_cd_id);
CREATE INDEX act_ptcpt_rol_cd_idx ON act_ptcpt_tbl(rol_cd_id);

 -- GET THE SCHEMA VERSION
CREATE OR REPLACE FUNCTION GET_SCH_VRSN() RETURNS VARCHAR(10) AS
$$
BEGIN
	RETURN '0.9.0.5';
END;
$$ LANGUAGE plpgsql;

SELECT REG_PATCH('20171003-01');

COMMIT;