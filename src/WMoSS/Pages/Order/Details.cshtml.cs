using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using WMoSS.Entities;

namespace WMoSS.Pages.Order
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public OrderDetailsViewModel OrderDetails { get; set; }

        public async Task<IActionResult> OnGet(int id, CancellationToken ct)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, ct);
            
            if (order == null)
            {
                return NotFound();
            }

            // get all tickets, include movie sessions, movie details and theater details
            var tickets = await _context.Tickets
                .Include(t => t.MovieSession)
                    .ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieSession)
                    .ThenInclude(ms => ms.Theater)
                .AsNoTracking()
                .Where(t => t.OrderId == order.Id)
                .ToListAsync();
            
            // group tickets into order items
            // Definition of OrderItem class is located below
            var orderItems = tickets.GroupBy(t => t.MovieSessionId).Select(g => new OrderItem
            {
                MovieSession = g.First().MovieSession,
                Tickets = g.AsEnumerable()
            });

            // model for order details
            OrderDetails = new OrderDetailsViewModel
            {
                OrderItems = orderItems,
                Order = order,
                DeliveryInformation = "Order receipt will be sent via email. Tickets will be sent via mail",
                PaymentInformation = string.Format("Credit card: xxxx-xxxx-xxxx-{0}", order.CardNumber.Substring(order.CardNumber.Length-4))
            };

            return Page();
                
        }
    }

    public class OrderItem
    {
        public MovieSession MovieSession { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public double SubTotal => MovieSession.TicketPrice * Tickets.Count();
    }

    public class OrderDetailsViewModel
    {
        public Entities.Order Order { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
        public double GrantTotal => Order.TotalPrice ?? OrderItems.Sum(oi => oi.SubTotal);
        public double GstRate { get; set; } = 0.1;
        public double GstAmount => GrantTotal * GstRate;
        public string DeliveryInformation { get; set; } = "";
        public string PaymentInformation { get; set; } = "";
    }
}