# ExcelImportTest
Asp.net Core Excel file upload test project

How to setup the database for the ExcelImportTest project. <br/>
Because the project uses SQL procedure for checking if table exits with given column names and types there a few extra step nessery to setup the database.<br/>
Setup steps:<br/>
1 Have a target SQL server ready<br/>
2 Check the database connection string in appsettings.json file at line ExcelSheetsContext so it connects to SQL server<br/>
3 Run in the Package Manager Console: script-migration<br/>
4 Run DBProcedures/CheckColumnsTable.sql script on the traget database<br/>
5 Run DBProcedures/GetTableNameFromColumnsData.sql script on the traget database<br/>
