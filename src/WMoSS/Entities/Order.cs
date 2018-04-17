using System;
using System.Collections.Generic;
using System.Text;

namespace WMoSS.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string MailingAddress { get; set; }
        public string EmailAddress { get; set; }
        public double? TotalPrice { get; set; }
        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public DateTime? CreatedAt { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }
    }
}
