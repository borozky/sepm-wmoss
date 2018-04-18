using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WMoSS.Entities
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("MovieSession")]
        public int MovieSessionId { get; set; }
        public MovieSession MovieSession { get; set; }

        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public string SeatNumber { get; set; }
    }
}
