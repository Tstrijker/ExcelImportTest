using ExcelImportTest.Database;
using ExcelImportTest.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelImportText.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class XLSXImporterController : ControllerBase
    {
        private const string XLSX_FILE_EXTENSION = ".xlsx";

        private readonly ExcelSheetsContext context;

        public XLSXImporterController(ExcelSheetsContext context)
        {
            this.context = context;
        }

        public Task WriteXLSXFileData { get; private set; }

        [HttpPost("single-file")]
        public async Task<IActionResult> UploadXLSXFile(IFormFile file, CancellationToken ct)
        {
            if (file == null)
                throw new Exception("No XLSX file was uploaded");

            if (Path.GetExtension(file.FileName) != XLSX_FILE_EXTENSION)
                throw new Exception("The file is not of the XLSX file extension");

            XLSXFileData fileData = XLSXReader.ReadXLSXFileSteam(file.OpenReadStream());

            await ExcelSheetDBWriter.WriteXLSXFileData(fileData, context, ct);

            return Ok();
        }
    }
}
