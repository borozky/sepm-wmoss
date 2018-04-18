using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using WMoSS.Entities;
using System.Linq;

namespace WMoSS.Repositories.Excel
{
    public class UserExcelRepository : ExcelRepository<ApplicationUser>, IUserRepository
    {
        public UserExcelRepository(ExcelPackage excelPackage) : base(excelPackage)
        {

        }

        public ApplicationUser FindByEmail(string email)
        {
            return rowIds
                .Where(id => worksheet.Cells[id, 2].GetValue<string>() == email)
                .Select(id => GetEntityByRowId(id))
                .FirstOrDefault();
        }

        public ApplicationUser FindByEmailAndPassword(string email, string passwordHashed)
        {
            return rowIds
                .Where(id => 
                    worksheet.Cells[id, 2].GetValue<string>() == email
                    && worksheet.Cells[id, 3].GetValue<string>() == passwordHashed)
                .Select(id => GetEntityByRowId(id))
                .FirstOrDefault();
        }

        protected override void AssignCellValues(int newRowId, ApplicationUser user)
        {
            worksheet.Cells[newRowId, 1].Value = user.FullName;
            worksheet.Cells[newRowId, 2].Value = user.Email;
            worksheet.Cells[newRowId, 3].Value = user.PasswordEncrypted;
            worksheet.Cells[newRowId, 4].Value = user.Role;
        }

        protected override ApplicationUser GetEntityByRowId(int rowId)
        {
            return new ApplicationUser
            {
                Id = rowId,
                FullName = worksheet.Cells[rowId, 1].GetValue<string>(),
                Email = worksheet.Cells[rowId, 2].GetValue<string>(),
                PasswordEncrypted = worksheet.Cells[rowId, 3].GetValue<string>(),
                Role = worksheet.Cells[rowId, 4].GetValue<string>()
            };
        }

        protected override ExcelWorksheet GetWorksheet()
        {
            return excelPackage.Workbook.Worksheets["ADMINS"];
        }

        protected override void SetEntityId(ApplicationUser user, int id)
        {
            user.Id = id;
        }

        public void Update(int userId, ApplicationUser user)
        {
            if (userId != user.Id)
            {
                throw new Exception("User IDs mismatch");
            }

            if (Exists(userId) == false)
            {
                throw new Exception($"User with ID {userId} doesn't exists");
            }

            AssignCellValues(userId, user);
            excelPackage.Save();
        }


    }
}
