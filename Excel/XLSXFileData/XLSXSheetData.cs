using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelImportTest.Excel
{
    public class XLSXSheetData
    {
        public string name;
        public List<string> columnNames = new List<string>();
        public List<XLSXCellTypes> rowDataTypes = new List<XLSXCellTypes>();
        public List<XLSXRowData> rows = new List<XLSXRowData>();
    }
}
