CREATE PROCEDURE dbo.GetTableNameFromColumnsData
    @CheckColumns CheckColumnsTable READONLY,
    @FoundTableName VARCHAR(50) OUTPUT
AS
BEGIN

--INSERT INTO @CheckColumns VALUES (1, 'id', 'int'), (2, 'Collom_1', 'decimal'), (3, 'Collom_2', 'varchar'), (4, 'Collom_3', 'varchar'), (5, 'Collom_4', 'varchar'), (6, 'Collom_5', 'varchar'), (7, 'Collom_6', 'varchar'), (8, 'Collom_7', 'varchar'), (9, 'Collom_8', 'varchar'), (10, 'Collom_9', 'varchar'), (11, 'Collom_10', 'varchar')

DECLARE @TableName VARCHAR(50)
DECLARE @ColumnName VARCHAR(50)
DECLARE @ColumnDataType NVARCHAR(128)
DECLARE @CheckColumnName VARCHAR(50)
DECLARE @CheckColumnDataType NVARCHAR(128)
DECLARE @ColumnCount INT
DECLARE @ColumnTotal INT
DECLARE @ISTABLE BIT

SET @ColumnTotal = 11;

DECLARE table_cursor CURSOR FOR 
SELECT TABLE_NAME FROM information_schema.tables 

OPEN table_cursor  
FETCH NEXT FROM table_cursor INTO @TableName  
 
WHILE @@FETCH_STATUS = 0  
BEGIN  
    SET @ColumnCount = 1

    DECLARE column_cursor CURSOR FOR 
    SELECT COLUMN_NAME, DATA_TYPE FROM information_schema.COLUMNS 
    WHERE TABLE_NAME = @TableName;
    
    OPEN column_cursor  
    FETCH NEXT FROM column_cursor INTO @ColumnName, @ColumnDataType
    
    WHILE @@FETCH_STATUS = 0  
    BEGIN

    SELECT @CheckColumnName=columnName, @CheckColumnDataType=dataType FROM @CheckColumns WHERE id = @ColumnCount 

    IF @ColumnName = @CheckColumnName AND @ColumnDataType = @CheckColumnDataType AND @ColumnCount <= @ColumnTotal
        SET @ISTABLE = 1;
    ELSE
    BEGIN
        SET @ISTABLE = 0;
        BREAK
    END

    SET @ColumnCount = @ColumnCount + 1

    FETCH NEXT FROM column_cursor INTO @ColumnName, @ColumnDataType
    END 

    CLOSE column_cursor
    DEALLOCATE column_cursor;

    IF @ISTABLE = 1
    BEGIN
        SET @FoundTableName = @TableName;
        BREAK
    END

    FETCH NEXT FROM table_cursor INTO @TableName
END  
CLOSE table_cursor
DEALLOCATE table_cursor;

END

RETURN 0;