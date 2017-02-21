--This Script Is Created By SpecCreator
/*---------------------------------
 作者 : Tester
 日期 : 2017-02-09
 內容 : test_table
---------------------------------*/

BEGIN TRANSACTION
CREATE TABLE [dbo].[Tmp_test_table] (
	[key1]    	VARCHAR(20) NOT NULL,
	[key2]    	INT NOT NULL,
	[date1]   	DATE NOT NULL,
	[date2]   	DATETIME NOT NULL,
	[date3]   	SMALLDATETIME NOT NULL,
	[char1]   	CHAR(10) NOT NULL,
	[char2]   	NCHAR(10) NOT NULL,
	[char3]   	VARCHAR(10) NOT NULL,
	[char4]   	NVARCHAR(10) NOT NULL,
	[char5]   	NVARCHAR(MAX) NOT NULL,
	[bit1]    	BIT NOT NULL,
	[opt1]    	SMALLINT NOT NULL,
	[opt2]    	SMALLINT NOT NULL,
	[int1]    	INT NOT NULL,
	[numeric1]	NUMERIC(20,5) NOT NULL
);

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[test_table]') AND type in (N'U'))
	EXEC sp_backup_data 'test_table', 'Tmp_test_table'

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[test_table]') AND type in (N'U'))
	DROP TABLE test_table;

EXEC sp_rename 'Tmp_test_table', 'test_table', 'OBJECT'
GO

ALTER TABLE [dbo].[test_table] ADD CONSTRAINT [PK_test_table] PRIMARY KEY CLUSTERED
(
    [key1],[key2]
)
ON [PRIMARY]
GO

COMMIT;

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('sp_app_grant') AND OBJECTPROPERTY(object_id,'IsProcedure')=1)
	EXEC sp_app_grant 'test_table', 'ALL';

SET NOCOUNT ON; BEGIN TRAN; DECLARE @tbname VARCHAR(40), @loguser VARCHAR(20), @dt DATETIME, @owfield BIT, @owoption BIT;
SELECT @tbname = 'test_table', @loguser = USER, @dt = ROUND(CONVERT(FLOAT, GETDATE()), 0, 1);
SELECT @owfield = 1, @owoption = 1;
DELETE app_table WHERE tablename = @tbname;
INSERT app_table SELECT @tbname, '測試資料表';
SELECT * INTO #appTableField FROM app_table_field WHERE 1 = 0;
INSERT #appTableField SELECT @tbname, 'key1'    ,  1, '測試主鍵1'  , '測試主鍵1'  , 1, 1, 0, '', 0,   0, 0, '', 1,  1, 150, 'TextEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'key2'    ,  2, '測試主鍵2'  , '測試主鍵2'  , 1, 1, 0, '', 0,   0, 0, '', 1,  2, 150, 'CalcEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'date1'   ,  3, '測試日期1'  , '測試日期1'  , 0, 0, 0, '', 0,   0, 0, '', 1,  3, 150, 'DateEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'date2'   ,  4, '測試日期2'  , '測試日期2'  , 0, 0, 0, '', 0,   0, 0, '', 1,  4, 150, 'DateEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'date3'   ,  5, '測試日期3'  , '測試日期3'  , 0, 0, 0, '', 0,   0, 0, '', 1,  5, 150, 'DateEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'char1'   ,  6, '測試字串1'  , '測試字串1'  , 0, 0, 0, '', 0,   0, 0, '', 1,  6, 150, 'TextEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'char2'   ,  7, '測試字串2'  , '測試字串2'  , 0, 0, 0, '', 0,   0, 0, '', 1,  7, 150, 'TextEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'char3'   ,  8, '測試字串3'  , '測試字串3'  , 0, 0, 0, '', 0,   0, 0, '', 1,  8, 150, 'TextEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'char4'   ,  9, '測試字串4'  , '測試字串4'  , 0, 0, 0, '', 0,   0, 0, '', 1,  9, 150, 'TextEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'char5'   , 10, '測試字串5'  , '測試字串5'  , 0, 0, 0, '', 0,   0, 0, '', 1, 10, 150, 'TextEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'bit1'    , 11, '測試二元1'  , '測試二元1'  , 0, 0, 0, '', 0,   0, 0, '', 1, 11, 150, 'CheckEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'opt1'    , 12, '測試選項1'  , '測試選項1'  , 0, 0, 0, '', 0,   2, 0, '', 1, 12, 150, 'LookUpEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'opt2'    , 13, '測試選項2'  , '測試選項2'  , 0, 0, 0, '', 0,   8, 0, '', 1, 13, 150, 'LookUpEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'int1'    , 14, '測試整數1'  , '測試整數1'  , 0, 0, 0, '', 0,   0, 0, '', 1, 14, 150, 'CalcEdit', @loguser, @dt;
INSERT #appTableField SELECT @tbname, 'numeric1', 15, '測試浮點數1', '測試浮點數1', 0, 0, 0, '', 0,   0, 0, '', 1, 15, 150, 'CalcEdit', @loguser, @dt;

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
AND m.name = a.tablename AND c.name = a.fieldname AND m.name = 'test_table';

SELECT * INTO #appTableFieldo FROM app_table_field_option WHERE 1 = 0;
SELECT * INTO #appTableFieldoi FROM app_table_field_option_item WHERE 1 = 0;

INSERT #appTableFieldo SELECT 2, '2.測試選項1', 1;
INSERT #appTableFieldoi SELECT 2, 1, '1.買進', @loguser, @dt;
INSERT #appTableFieldoi SELECT 2, 2, '2.賣出', @loguser, @dt;

INSERT #appTableFieldo SELECT 8, '8.測試選項2', 1;
INSERT #appTableFieldoi SELECT 8, 1, '1.市價', @loguser, @dt;
INSERT #appTableFieldoi SELECT 8, 2, '2.限價', @loguser, @dt;

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

PRINT 'Update Table test_table Completely'
GO
SET ANSI_PADDING OFF
GO