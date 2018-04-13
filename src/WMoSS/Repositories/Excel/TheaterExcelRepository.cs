using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories.Excel
{
    public class TheaterExcelRepository : ITheaterRepository
    {
        private ExcelPackage package;
        public TheaterExcelRepository(ExcelPackage package)
        {
            this.package = package;
        }

        public IEnumerable<Theater> FindAll()
        {
            var worksheet = package.Workbook.Worksheets["THEATERS"];
            var theaterIds = worksheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(id => id).Skip(1);
            return theaterIds.Select(id => GetTheaterByRowId(worksheet, id)).Where(theater => theater != null);
        }


        private Theater GetTheaterByRowId(ExcelWorksheet worksheet, int rowId)
        {
            var theater = new Theater
            {
                Id = rowId,
                Name = worksheet.Cells[rowId, 1].GetValue<string>(),
                Capacity = worksheet.Cells[rowId, 2].GetValue<int?>() ?? 0,
                Address = worksheet.Cells[rowId, 3].GetValue<string>()
            };

            return HasEmptyDetails(theater) ? null : theater;
        }

        private bool HasEmptyDetails(Theater theater)
        {
            return string.IsNullOrWhiteSpace(theater.Name) 
                && theater.Capacity == 0 
                && string.IsNullOrWhiteSpace(theater.Address);
        }

    }
}
