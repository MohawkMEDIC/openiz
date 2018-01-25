/** 
 * <update id="20170803-01" applyRange="0.2.0.0-0.9.0.2"  invariantName="npgsql">
 *	<summary>Adds sequencing to the participation table</summary>
 *	<remarks>This will add sequence identifiers to the participation table so that data can be returned 
 *	in proper order</remarks>
 *	<isInstalled>select ck_patch('20170803-01')</isInstalled>
 * </update>
 */

 BEGIN TRANSACTION ;

 CREATE SEQUENCE ACT_PTCPT_SEQ START WITH 1 INCREMENT BY 1;

 ALTER TABLE ACT_PTCPT_TBL ADD PTCPT_SEQ_ID NUMERIC(20,0) NOT NULL DEFAULT NEXTVAL('ACT_PTCPT_SEQ');

 -- GET THE SCHEMA VERSION
CREATE OR REPLACE FUNCTION GET_SCH_VRSN() RETURNS VARCHAR(10) AS
$$
BEGIN
	RETURN '0.9.0.3';
END;
$$ LANGUAGE plpgsql;

SELECT REG_PATCH('20170803-01');

COMMIT;