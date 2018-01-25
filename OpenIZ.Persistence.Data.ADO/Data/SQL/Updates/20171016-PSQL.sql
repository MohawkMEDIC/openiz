/** 
 * <update id="20171016-01" applyRange="0.2.0.4-0.9.0.6"  invariantName="npgsql">
 *	<summary>Adds FIRST aggregation function</summary>
 *	<remarks></remarks>
 *	<isInstalled>select ck_patch('20171016-01')</isInstalled>
 * </update>
 */

BEGIN TRANSACTION ;

-- Create a function that always returns the first non-NULL item
CREATE OR REPLACE FUNCTION public.first_agg ( anyelement, anyelement )
RETURNS anyelement AS $$
        SELECT $1;
$$ LANGUAGE SQL IMMUTABLE STRICT ;

-- And then wrap an aggregate around it
CREATE AGGREGATE public.FIRST (
        sfunc    = public.first_agg,
        basetype = anyelement,
        stype    = anyelement
);
 
 -- GET THE SCHEMA VERSION
CREATE OR REPLACE FUNCTION GET_SCH_VRSN() RETURNS VARCHAR(10) AS
$$
BEGIN
	RETURN '0.9.0.7';
END;
$$ LANGUAGE plpgsql;

SELECT REG_PATCH('20171016-01');

COMMIT;