using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories.Excel
{
    public class OrderExcelRepository : IOrderRepository
    {
        private ExcelPackage excelPackage;
        private ExcelWorksheet ordersWorksheet;
        private IEnumerable<int> orderRowIds;

        public OrderExcelRepository(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
            ordersWorksheet = excelPackage.Workbook.Worksheets["ORDERS"];
            orderRowIds = ordersWorksheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(id => id).Skip(1); ;
        }

        public IEnumerable<Order> FindAll()
        {
            return orderRowIds.Select(id => GetOrderByRowId(id));
        }

        public Order FindById(int orderId)
        {
            return Exists(orderId) ? GetOrderByRowId(orderId) : null;
        }

        public IEnumerable<Order> SearchByKeyword(string keyword)
        {
            return orderRowIds.Where(id =>
                    ordersWorksheet.Cells[id, 1].GetValue<string>().ToLower().Contains(keyword.ToLower())
                || ordersWorksheet.Cells[id, 2].GetValue<string>().ToLower().Contains(keyword.ToLower())
                || ordersWorksheet.Cells[id, 3].GetValue<string>().ToLower().Contains(keyword.ToLower())
            ).Select(id => GetOrderByRowId(id));
        }

        public void Create(Order order)
        {
            var lastRowId = orderRowIds.Max();
            var newRowId = lastRowId + 1;
            ordersWorksheet.InsertRow(newRowId, 1);

            AssignOrderValues(newRowId, order);
            excelPackage.Save();
            order.Id = newRowId;
        }

        public bool Exists(int orderId)
        {
            return orderRowIds.Contains(orderId);
        }

        private Order GetOrderByRowId(int rowId)
        {
            return new Order
            {
                FullName = ordersWorksheet.Cells[rowId, 1].GetValue<string>(),
                MailingAddress = ordersWorksheet.Cells[rowId, 2].GetValue<string>(),
                EmailAddress = ordersWorksheet.Cells[rowId, 3].GetValue<string>(),
                TotalPrice = ordersWorksheet.Cells[rowId, 4].GetValue<double?>(),
                CardNumber = ordersWorksheet.Cells[rowId, 5].GetValue<string>(),
                Expiry = ordersWorksheet.Cells[rowId, 6].GetValue<string>(),
                CreatedAt = ordersWorksheet.Cells[rowId, 7].GetValue<string>()
            };
        }

        private void AssignOrderValues(int rowId, Order order)
        {
            ordersWorksheet.Cells[rowId, 1].Value = order.FullName;
            ordersWorksheet.Cells[rowId, 2].Value = order.MailingAddress;
            ordersWorksheet.Cells[rowId, 3].Value = order.EmailAddress;
            ordersWorksheet.Cells[rowId, 4].Value = order.TotalPrice;
            ordersWorksheet.Cells[rowId, 5].Value = order.CardNumber;
            ordersWorksheet.Cells[rowId, 6].Value = order.Expiry;
            ordersWorksheet.Cells[rowId, 7].Value = order.CreatedAt;
        }
    }
}
