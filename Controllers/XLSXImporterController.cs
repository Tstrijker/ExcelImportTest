using ExcelImportTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelImportText.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class XLSXImporterController : ControllerBase
    {
        private const string XLSX_FILE_EXTENSION = ".xlsx";

        private readonly ILogger<XLSXImporterController> logger;

        public XLSXImporterController(ILogger<XLSXImporterController> logger)
        {
            this.logger = logger;
        }

        [HttpPost("single-file")]
        public async Task<IActionResult> UploadXLSXFile(IFormFile file)
        {
            if (Path.GetExtension(file.FileName) != XLSX_FILE_EXTENSION)
                return BadRequest();



            return Ok();
        }
    }
}
