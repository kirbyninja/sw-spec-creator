--This Script Is Created By SpecCreator
/*---------------------------------
 作者 : {0}
 日期 : {1}
 內容 : {2}
---------------------------------*/

BEGIN TRANSACTION
{3}
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{2}]') AND type in (N'U'))
	EXEC sp_backup_data '{2}', 'Tmp_{2}'

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{2}]') AND type in (N'U'))
	DROP TABLE {2};

EXEC sp_rename 'Tmp_{2}', '{2}', 'OBJECT'
GO
{4}
COMMIT;

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('sp_app_grant') AND OBJECTPROPERTY(object_id,'IsProcedure')=1)
	EXEC sp_app_grant '{2}', 'ALL';

SET NOCOUNT ON; BEGIN TRAN; DECLARE @tbname VARCHAR(40), @loguser VARCHAR(20), @dt DATETIME, @owfield BIT, @owoption BIT;
SELECT @tbname = '{2}', @loguser = USER, @dt = ROUND(CONVERT(FLOAT, GETDATE()), 0, 1);
SELECT @owfield = 1, @owoption = 1;
DELETE app_table WHERE tablename = @tbname;
INSERT app_table SELECT @tbname, '{5}';
SELECT * INTO #appTableField FROM app_table_field WHERE 1 = 0;
{6}
IF @owfield=1
BEGIN
	DELETE app_table_field WHERE tablename = @tbname;
	INSERT app_table_field SELECT * FROM #appTableField;
END
ELSE
	INSERT app_table_field SELECT * FROM #appTableField a WHERE NOT EXISTS (SELECT 1 FROM app_table_field WHERE tablename = a.tablename AND fieldname = a.fieldname);
DROP TABLE #appTableField;

UPDATE a SET a.field_idx=c.colorder
FROM sysobjects m, syscolumns c, systypes t, app_table_field a
WHERE m.id = c.id AND c.xtype = t.xtype AND OBJECTPROPERTY(m.id, 'IsUserTable') = 1
AND m.name = a.tablename AND c.name = a.fieldname AND m.name = '{2}';

SELECT * INTO #appTableFieldo FROM app_table_field_option WHERE 1 = 0;
SELECT * INTO #appTableFieldoi FROM app_table_field_option_item WHERE 1 = 0;
{7}
IF @owoption=1
BEGIN
	DELETE app_table_field_option WHERE opt_no IN (SELECT opt_no FROM #appTableFieldo);
	INSERT app_table_field_option SELECT * FROM #appTableFieldo;
	DELETE app_table_field_option_item WHERE opt_no IN (SELECT opt_no FROM #appTableFieldo);
	INSERT app_table_field_option_item SELECT * FROM #appTableFieldoi;
END
ELSE
BEGIN
	INSERT app_table_field_option SELECT a.* FROM #appTableFieldo a
	WHERE NOT EXISTS (SELECT 1 FROM app_table_field_option WHERE opt_no = a.opt_no);
	INSERT app_table_field_option_item SELECT a.* FROM #appTableFieldoi a
	WHERE NOT EXISTS (SELECT 1 FROM app_table_field_option_item WHERE opt_no = a.opt_no AND item_no = a.item_no);
END;
DROP TABLE #appTableFieldo, #appTableFieldoi;

IF @@ERROR>0 ROLLBACK
ELSE COMMIT
GO

PRINT 'Update Table {2} Completely'
GO
SET ANSI_PADDING OFF
GO