using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMoSS.Repositories.Excel
{
    public abstract class ExcelRepository<TEntity> where TEntity : class
    {
        protected ExcelPackage excelPackage;
        protected ExcelWorksheet worksheet;
        protected IEnumerable<int> rowIds;

        protected ExcelRepository(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
            worksheet = GetWorksheet();
            rowIds = worksheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(id => id).Skip(1);
        }

        public virtual IEnumerable<TEntity> FindAll()
        {
            return rowIds.Select(id => GetEntityByRowId(id));
        }

        public virtual TEntity FindById(int id)
        {
            return Exists(id) ? GetEntityByRowId(id) : null;
        }

        public virtual void Create(TEntity entity)
        {
            var lastRowId = rowIds.Max();
            var newRowId = lastRowId + 1;
            worksheet.InsertRow(newRowId, 1);

            AssignCellValues(newRowId, entity);
            excelPackage.Save();
            SetEntityId(entity, newRowId);
        }


        public virtual bool Exists(int rowId)
        {
            return rowIds.Contains(rowId);
        }
        

        protected abstract ExcelWorksheet GetWorksheet();
        protected abstract void AssignCellValues(int newRowId, TEntity entity);
        protected abstract TEntity GetEntityByRowId(int rowId);
        protected abstract void SetEntityId(TEntity entity, int id);


    }
}
