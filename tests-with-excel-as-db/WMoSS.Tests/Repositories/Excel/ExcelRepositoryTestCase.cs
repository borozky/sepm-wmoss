using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace WMoSS.Tests.Repositories.Excel
{
    public abstract class ExcelRepositoryTestCase
    {
        protected ExcelPackage excelPackage;
        protected FileInfo excelSourceFile;
        protected string excelFilePath;

        protected ExcelRepositoryTestCase()
        {
            excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\src\", @"WMOSS-EXCELDB-TEST.xlsx");
            //File.Copy(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\src\", @"WMOSS-EXCELDB-SAMPLE.xlsx"), excelFilePath, true);

            excelSourceFile = new FileInfo(excelFilePath);
            excelPackage = new ExcelPackage(excelSourceFile);
        }

        [Fact]
        public void SmokeTest_ExcelFileExists()
        {
            bool excelExists = File.Exists(excelFilePath);
            Assert.True(excelExists);
        }

        public void Dispose()
        {
            excelPackage.Dispose();
        }
    }
}
