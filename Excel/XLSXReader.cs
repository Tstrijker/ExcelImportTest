using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelImportTest.Excel
{
    public class XLSXReader
    {
        public static XLSXFileData ReadXLSXFileSteam(Stream stream)
        {
            XLSXFileData fileData = new XLSXFileData();

            using (XLWorkbook workbook = new XLWorkbook(stream))
            {
                foreach (IXLWorksheet workSheet in workbook.Worksheets)
                {
                    // If there is not more then 1 row of data then we don't have any data rows to convert
                    if (workSheet.RowCount() < 2)
                        continue;

                    XLSXSheetData fileSheetData = new XLSXSheetData();

                    fileSheetData.name = workSheet.Name;

                    IXLRow columnRow = workSheet.FirstRow();

                    bool foundEmptyColomn = false;
                    foreach (IXLCell cell in columnRow.Cells())
                    {
                        // Skip empty columns (at the end)
                        if (cell.IsEmpty())
                        {
                            foundEmptyColomn = true;
                            continue;
                        }

                        // If there is a empty comlumn between filled columns making the colums data corrupt
                        if (foundEmptyColomn)
                            throw new Exception("There empty column(s) between two filled columns.");

                        fileSheetData.columnNames.Add(cell.GetString());
                    }

                    // Check for duplicate column names
                    List<string> duplicatecolumnNames = fileSheetData.columnNames.GroupBy(x => x)
                             .Where(g => g.Count() > 1)
                             .Select(y => y.Key).ToList();

                    if (duplicatecolumnNames.Count > 0)
                        throw new Exception($"There is a duplicate named column name(s): {string.Join(", ", duplicatecolumnNames)}");

                    // We start at the second row because the first row has column data stored in it.
                    for (int i = 2; i < workSheet.RowCount(); i++)
                    {
                        IXLRow row = workSheet.Row(i);

                        // Checking the data type of the first data row and register to row data types list.
                        if (i == 2)
                        {
                            // Make sure that the number of cells don't exceed the number of columns
                            for (int n = 1; n < row.CellCount() && n <= fileSheetData.columnNames.Count(); n++)
                            {
                                IXLCell cell = row.Cell(n);

                                if (Enum.TryParse(typeof(XLSXCellTypes), cell.DataType.ToString(), out object dataType))
                                    fileSheetData.rowDataTypes.Add((XLSXCellTypes)dataType);
                            }

                            // Check if the number of found columns names and types are in sync
                            if (fileSheetData.columnNames.Count() != fileSheetData.rowDataTypes.Count())
                                throw new Exception("The number of column names and types are not equal. Number of found names {} and types {}");
                        }

                        if (row.IsEmpty())
                            continue;

                        XLSXRowData fileRowData = new XLSXRowData();

                        // Make sure that the number of cells don't exceed the number of columns
                        for (int n = 1; n < row.CellCount() && n <= fileSheetData.columnNames.Count(); n++)
                        {
                            IXLCell cell = row.Cell(n);

                            fileRowData.cells.Add(cell.GetString());
                        }

                        fileSheetData.rows.Add(fileRowData);
                    }

                    fileData.sheets.Add(fileSheetData);
                }
            }

            return fileData;
        }
    }
}
