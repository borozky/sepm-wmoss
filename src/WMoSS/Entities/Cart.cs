using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WMoSS.Extensions;

namespace WMoSS.Entities
{
    public class Cart
    {
        public static Cart GetFrom(ISession session)
        {
            var cart = session.Get<Cart>("cart");
            if (cart == null)
            {
                cart = new Entities.Cart
                {
                    CartItems = new List<CartItem>()
                };
            }
            return cart;
        }

        public IList<CartItem> CartItems { get; set; } = new List<CartItem>();

        public virtual void Add(CartItem cartItem)
        {
            var foundCartItem = CartItems.FirstOrDefault(ci => ci.MovieSessionId == cartItem.MovieSessionId);
            if (foundCartItem != null)
            {
                foundCartItem.TicketQuantity += cartItem.TicketQuantity;
                foundCartItem.Seats = cartItem.Seats;
            }
            else
            {
                CartItems.Add(cartItem);
            }
        }

        public virtual void Add(int movieSessionId, int quantity, string[] seats)
        {
            Add(new CartItem
            {
                MovieSessionId = movieSessionId,
                TicketQuantity = quantity,
                Seats = seats.ToList()
            });
        }

        public virtual bool Modify(int movieSessionId, CartItem cartItem)
        {
            if (movieSessionId != cartItem.MovieSessionId)
            {
                return false;
            }

            if (cartItem.TicketQuantity <= 0)
            {
                return false;
            }

            var foundCartItem = CartItems.FirstOrDefault(ci => ci.MovieSessionId == movieSessionId);
            if (foundCartItem == null)
            {
                return false;
            }

            foundCartItem.TicketQuantity = cartItem.TicketQuantity;

            // Remove exceeeding seats, keep existing ones
            foundCartItem.Seats = foundCartItem.Seats.Where((seat, i) => i < cartItem.TicketQuantity).ToArray();
            return true;
        }

        public virtual bool Remove(int movieSessionId)
        {
            var foundCartItem = CartItems.FirstOrDefault(ci => ci.MovieSessionId == movieSessionId);
            if (foundCartItem != null)
            {
                CartItems.Remove(foundCartItem);
                return true;
            }

            return false;
        }

        public double TotalPrice
        {
            get
            {
                return CartItems.Sum(ci => ci.TotalPrice);
            }
        }

        public virtual void SaveTo(ISession session)
        {
            session.Set("cart", this);
        }

        public int RemainingSeats => CartItems.Sum(c => c.RemainingSeats);

        public double GstRate { get; set; } = 0.1;
        public double GstPrice => TotalPrice * GstRate;
    }

    public class CartItem
    {
        [Required(ErrorMessage = "Movie session is required")]
        public int? MovieSessionId { get; set; }
        public MovieSession MovieSession { get; set; }

        [Required(ErrorMessage = "Ticket quantity is required")]
        [Range(1, 15)]
        public int? TicketQuantity { get; set; }

        public IList<string> Seats { get; set; }

        public double TotalPrice
        {
            get
            {
                var unitPrice = MovieSession.DEFAULT_TICKET_PRICE;
                if (MovieSession != null)
                {
                    unitPrice = MovieSession.TicketPrice;
                }

                var ticketQty = 0;
                if (TicketQuantity.HasValue)
                {
                    ticketQty = TicketQuantity.Value;
                }

                return unitPrice * ticketQty;
            }
        }

        public int RemainingSeats => ((TicketQuantity ?? 0) - (Seats == null ? 0 : Seats.Count));
    }
}
