/** 
 * <update id="20170913-01" applyRange="0.2.0.3-0.9.0.3"  invariantName="npgsql">
 *	<summary>Adds BAD to the type of name uses</summary>
 *	<remarks>Fixes issue with locked accounts</remarks>
 *	<isInstalled>select ck_patch('20170913-01')</isInstalled>
 * </update>
 */

 BEGIN TRANSACTION ;

INSERT INTO CD_TBL VALUES('3efd3b6e-02d5-4cc9-9088-ef8f31e32efa',TRUE);
INSERT INTO CD_SET_MEM_ASSOC_TBL (CD_ID, SET_ID) VALUES ('3efd3b6e-02d5-4cc9-9088-ef8f31e32efa', '8df14280-3d05-45a6-bfae-15b63dfc379f');
INSERT INTO CD_VRSN_TBL (CD_ID, STS_CD_ID, CRT_USR_ID, MNEMONIC, CLS_ID) VALUES ('3efd3b6e-02d5-4cc9-9088-ef8f31e32efa', 'c8064cbd-fa06-4530-b430-1a52f1530c27', 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8', 'BadName', '0d6b3439-c9be-4480-af39-eeb457c052d0');
CREATE INDEX ACT_PTCPT_ROL_CD_ID_IDX ON ACT_PTCPT_TBL(ROL_CD_ID);

 -- GET THE SCHEMA VERSION
CREATE OR REPLACE FUNCTION GET_SCH_VRSN() RETURNS VARCHAR(10) AS
$$
BEGIN
	RETURN '0.9.0.4';
END;
$$ LANGUAGE plpgsql;

SELECT REG_PATCH('20170913-01');


COMMIT;