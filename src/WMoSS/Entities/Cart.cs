using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WMoSS.Entities
{
    public class Cart
    {
        IList<CartItem> CartItems { get; set; }
    }

    public class CartItem
    {
        [Required]
        public int? MovieSessionId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? TicketQuantity { get; set; }
        
        public IList<string> Seats { get; set; }
    }
}
