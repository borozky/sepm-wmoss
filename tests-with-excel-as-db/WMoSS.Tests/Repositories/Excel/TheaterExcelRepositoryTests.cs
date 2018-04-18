using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Repositories.Excel;
using Xunit;

namespace WMoSS.Tests.Repositories.Excel
{
    public class TheaterExcelRepositoryTests : ExcelRepositoryTestCase
    {
        ExcelWorksheet theatersWorksheet;

        // system under test
        TheaterExcelRepository theatersRepo;

        public TheaterExcelRepositoryTests() : base()
        {
            theatersRepo = new TheaterExcelRepository(excelPackage);
            theatersWorksheet = excelPackage.Workbook.Worksheets["THEATERS"];
        }

        
        [Fact]
        public void SmokeTest_TheaterWorksheetExists()
        {
            Assert.NotNull(theatersWorksheet);
        }

        [Fact]
        public void Test_FindAll_Returns_1OrMoreTheaters()
        {
            var theaters = theatersRepo.FindAll();
            Assert.NotEmpty(theaters);
        }
        
    }
}
