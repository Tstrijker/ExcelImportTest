using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelImportText.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class XLSXImporterController : ControllerBase
    {
        private readonly ILogger<XLSXImporterController> logger;

        public XLSXImporterController(ILogger<XLSXImporterController> logger)
        {
            this.logger = logger;
        }

    }
}
