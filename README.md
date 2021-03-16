# ExcelImportTest
Asp.net Core Excel file upload test project

How to setup the database for the ExcelImportTest project.
Because the project uses SQL procedure for checking if table exits with given column names and types there a few extra step nessery to setup the database.
Setup steps:
1 Have a target SQL server ready
2 Check the database connection string in appsettings.json file at line ExcelSheetsContext so it connects to SQL server
3 Run in the Package Manager Console: script-migration
4 Run DBProcedures/CheckColumnsTable.sql script on the traget database
5 Run DBProcedures/GetTableNameFromColumnsData.sql script on the traget database
