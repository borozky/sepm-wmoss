using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WMoSS.Data;

namespace WMoSS.Entities
{
    public class MovieSession
    {
        public const double DEFAULT_TICKET_PRICE = 20.00;

        [Key]
        public int Id { get; set; }

        [Range(1.0, int.MaxValue)]
        public double TicketPrice { get; set; } = DEFAULT_TICKET_PRICE;
        
        [Required]
        public DateTime? ScheduledAt { get; set; }

        [Required]
        [ForeignKey("Theater")]
        public int TheaterId { get; set; }
        public Theater Theater { get; set; }

        [Required]
        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [ForeignKey("ScheduledBy")]
        public int? ScheduledById { get; set; }
        public ApplicationUser ScheduledBy { get; set; }





    }
}
