using System;
using System.Collections.Generic;
using System.Text;

namespace WMoSS.Entities
{
    public class MovieSession
    {
        public const double DEFAULT_TICKET_PRICE = 20.00;

        public int Id { get; set; }
        public double TicketPrice { get; set; } = DEFAULT_TICKET_PRICE;
        public string ScheduledAt { get; set; }

        public int TheaterId { get; set; }
        public Theater Theater { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int ScheduledById { get; set; }
        public ApplicationUser ScheduledBy { get; set; }

        public string CreatedAt { get; set; }
    }
}
