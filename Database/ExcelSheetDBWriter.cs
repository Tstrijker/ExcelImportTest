using ExcelImportTest.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelImportTest.Database
{
    public class ExcelSheetDBWriter
    {
        public static async Task WriteXLSXFileData(XLSXFileData fileData, ExcelSheetsContext context, CancellationToken ct)
        {
            await context.Database.OpenConnectionAsync(ct);

            try
            {
                foreach (XLSXSheetData sheet in fileData.sheets)
                {
                    string tableName = await GetTableName(sheet, context, ct);

                    if (string.IsNullOrEmpty(tableName))
                        tableName = await CreateTable(sheet, context, ct);

                    if (string.IsNullOrEmpty(tableName))
                        throw new Exception("The table name is empty, no table could be found or created.");

                    await InsertExcelData(tableName, sheet, context, ct);
                }
                
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }
        }

        private static async Task<string> GetTableName(XLSXSheetData sheet, ExcelSheetsContext context, CancellationToken ct)
        {
            DataTable columnsData = new DataTable();

            columnsData.Columns.Add("id", typeof(int));
            columnsData.Columns.Add("columnName");
            columnsData.Columns.Add("dataType");

            columnsData.Rows.Add(1, "id", "int");

            for (int i = 0; i < sheet.columnNames.Count && i < sheet.rowDataTypes.Count; i++)
            {
                string columnName = sheet.columnNames[i].Replace(" ", "_");

                string columnDataType = ConvertXLSXCellTypesToSQLCheckType(sheet.rowDataTypes[i]);

                columnsData.Rows.Add(i + 2, columnName, columnDataType);
            }

            SqlParameter columnsTable = new SqlParameter()
            {
                ParameterName = "@CheckColumns",
                SqlDbType = SqlDbType.Structured,
                Direction = ParameterDirection.Input,
                TypeName = "dbo.CheckColumnsTable",
                Value = columnsData
            };

            SqlParameter foundTableName = new SqlParameter()
            {
                ParameterName = "@FoundTableName",
                SqlDbType = SqlDbType.VarChar,
                Size = 50,
                Direction = ParameterDirection.Output,
            };

            // The sql procedure dbo.GetTableNameFromColumnsData code can befound in the files DBProcedures/CheckColumnsTable.sql and DBProcedures/GetTableNameFromColumnsData.sql
            await context.Database.ExecuteSqlRawAsync("dbo.GetTableNameFromColumnsData @CheckColumns, @FoundTableName OUTPUT", new SqlParameter[] { columnsTable, foundTableName }, ct);

            if (foundTableName.Value != null && foundTableName.Value != DBNull.Value)
            {
                return foundTableName.Value.ToString();
            }

            return null;
        }
        //@CheckColumns, @FoundTableName
        private static async Task<string> CreateTable(XLSXSheetData sheet, ExcelSheetsContext context, CancellationToken ct)
        {
            string tableName = Guid.NewGuid().ToString();

            tableName = tableName.Replace("-", "");

            string columns = "id int  NOT NULL IDENTITY(1, 1)";

            for (int i = 0; i < sheet.columnNames.Count && i < sheet.rowDataTypes.Count; i++)
            {
                columns += ", ";

                // Make sure that we don't have any spaces in the column names
                string columnName = sheet.columnNames[i].Replace(" ", "_");

                string columnDataType = ConvertXLSXCellTypesToSQLType(sheet.rowDataTypes[i]);

                columns += $"{columnName} {columnDataType}";
            }

            string createTableSQL = $"CREATE TABLE {tableName} ({columns});";

            await context.Database.ExecuteSqlRawAsync(createTableSQL, ct);

            return tableName;
        }

        private static async Task InsertExcelData(string tableName, XLSXSheetData sheet, ExcelSheetsContext context, CancellationToken ct)
        {
            string columns = "";

            for (int i = 0; i < sheet.columnNames.Count; i++)
            {
                if (i > 0)
                    columns += ", ";

                // Make sure that we don't have any spaces in the column names
                string columnName = sheet.columnNames[i].Replace(" ", "_");

                columns += columnName;
            }

            string values = "";
            for (int i = 0; i < sheet.rows.Count; i++)
            {
                XLSXRowData rowData = sheet.rows[i];
                if (i > 0)
                    values += ", ";

                string row = "(";

                for (int n = 0; n < rowData.cells.Count; n++)
                {
                    if (n > 0)
                        row += ", ";

                    string cellData = rowData.cells[n];

                    if (string.IsNullOrEmpty(rowData.cells[n]))
                        row += "NULL";
                    else
                        row += $"'{cellData}'";
                }

                row += ")";

                values += row;
            }

            string insertSQL = $"INSERT INTO {tableName} ({columns}) VALUES {values}";

            await context.Database.ExecuteSqlRawAsync(insertSQL, ct);
        }

        private static string ConvertXLSXCellTypesToSQLType(XLSXCellTypes cellType)
        {
            switch (cellType)
            {
                case XLSXCellTypes.Text:
                    return "VARCHAR(8000)";
                case XLSXCellTypes.Number:
                    return "DECIMAL";
                case XLSXCellTypes.Boolean:
                    return "BOOLEAN";
                case XLSXCellTypes.DateTime:
                    return "DATETIME";
                case XLSXCellTypes.TimeSpan:
                    return "TIMESTAMP";
            }

            return "VARCHAR(8000)";
        }

        private static string ConvertXLSXCellTypesToSQLCheckType(XLSXCellTypes cellType)
        {
            switch (cellType)
            {
                case XLSXCellTypes.Text:
                    return "varchar";
                case XLSXCellTypes.Number:
                    return "decimal";
                case XLSXCellTypes.Boolean:
                    return "boolean";
                case XLSXCellTypes.DateTime:
                    return "datetime";
                case XLSXCellTypes.TimeSpan:
                    return "timespan";
            }

            return "varchar";
        }
    }
}
