using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WMoSS.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "The mailing address is required")]
        public string MailingAddress { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public double? TotalPrice { get; set; }

        [Required(ErrorMessage = "Card number is required")]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        
        public string Expiry { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        [Required]
        public string CVV {get;set;}

        public IEnumerable<Ticket> Tickets { get; set; }
    }
}
