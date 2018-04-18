using System;
using System.Collections.Generic;
using System.Text;

namespace WMoSS.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public int MovieSessionId { get; set; }
        public MovieSession MovieSession { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string SeatNumber { get; set; }
    }
}
