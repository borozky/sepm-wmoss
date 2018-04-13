using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories.Excel
{
    public class TicketExcelRepository : ITicketRepository
    {
        private ExcelPackage excelPackage;
        private ExcelWorksheet ticketWorksheet;
        private IEnumerable<int> ticketRowIds;

        public TicketExcelRepository(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
            ticketWorksheet = excelPackage.Workbook.Worksheets["TICKETS"];
            ticketRowIds = ticketWorksheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(id => id).Skip(1);
        }

        public IEnumerable<Ticket> FindAll()
        {
            return ticketRowIds.Select(id => GetTicketByRowId(id));
        }

        public IEnumerable<Ticket> FIndAllByMovieSessionId(int movieSessionId)
        {
            return ticketRowIds
                .Where(id => ticketWorksheet.Cells[id, 1].GetValue<int>() == movieSessionId)
                .Select(id => GetTicketByRowId(id));
        }

        public IEnumerable<Ticket> FindAllByOrderId(int orderId)
        {
            return ticketRowIds
                .Where(id => ticketWorksheet.Cells[id, 2].GetValue<int>() == orderId)
                .Select(id => GetTicketByRowId(id));
        }

        public Ticket FindById(int ticketId)
        {
            return Exists(ticketId) ? GetTicketByRowId(ticketId) : null;
        }

        public void Create(Ticket ticket)
        {
            var lastRowId = ticketRowIds.Max();
            var newRowId = lastRowId + 1;
            ticketWorksheet.InsertRow(newRowId, 1);

            AssignTicketValues(newRowId, ticket);
            excelPackage.Save();
            ticket.Id = newRowId;

        }

        public void Update(int ticketId, Ticket ticket)
        {
            if (ticketId != ticket.Id)
            {
                throw new Exception("Ticket IDs mismatch");
            }

            if (Exists(ticketId) == false)
            {
                throw new Exception(String.Format("Ticket with ID {0} doesn't exists"));
            }

            AssignTicketValues(ticketId, ticket);
            excelPackage.Save();
        }

        public bool Exists(int ticketId)
        {
            return ticketRowIds.Contains(ticketId);
        }

        private Ticket GetTicketByRowId(int rowId)
        {
            return new Ticket
            {
                Id = rowId,
                MovieSessionId = ticketWorksheet.Cells[rowId, 1].GetValue<int>(),
                OrderId = ticketWorksheet.Cells[rowId, 2].GetValue<int>(),
                SeatNumber = ticketWorksheet.Cells[rowId, 3].GetValue<string>()
            };
        }

        private void AssignTicketValues(int rowId, Ticket ticket)
        {
            ticketWorksheet.Cells[rowId, 1].Value = ticket.MovieSessionId;
            ticketWorksheet.Cells[rowId, 2].Value = ticket.OrderId;
            ticketWorksheet.Cells[rowId, 3].Value = ticket.SeatNumber;
        }
    }
}
