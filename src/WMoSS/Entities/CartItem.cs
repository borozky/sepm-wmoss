using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WMoSS.Entities
{
    // 1 cart item per movie session
    public class CartItem
    {
        public int MovieSessionId { get; set; }
        public MovieSession MovieSession { get; set; }
        public int TicketQty { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
    }

    public class CartItemByMovie
    {
        public int MovieId;
        public Movie Movie;

        public IEnumerable<CartItem> CartItemsByMovieSession { get; set; }
    }

    


}
