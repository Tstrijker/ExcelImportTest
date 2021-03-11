using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelImportTest.Database
{
    public class ExcelSheetsContext : DbContext
    {
        public ExcelSheetsContext(DbContextOptions<ExcelSheetsContext> options)
            : base(options)
        {
        }
    }
}
