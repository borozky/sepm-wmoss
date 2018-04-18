using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMoSS.Entities;
using WMoSS.Repositories;
using WMoSS.Web.Extensions;

namespace WMoSS.Web.Helpers
{
    public class CartManager : ICartManager
    {
        IDictionary<int, int> CartItems;

        private ISession _session;
        private IMovieRepository _movieRepository;
        private IMovieSessionRepository _movieSessionRepository;
        private ITheaterRepository _theaterRepository;

        public CartManager(ISession session,
            IMovieRepository movieRepository,
            IMovieSessionRepository movieSessionRepository,
            ITheaterRepository theaterRepository)
        {
            _session = session;
            _movieRepository = movieRepository;
            _movieSessionRepository = movieSessionRepository;
            _theaterRepository = theaterRepository;

            CartItems = session.Get<IDictionary<int, int>>("cart") ?? new Dictionary<int, int>();
        }

        private void Save()
        {
            _session.Set("cart", CartItems);
        }

        private void _AddToCart(int movieSessionId, int numTickets)
        {
            var hasKey = CartItems.ContainsKey(movieSessionId);
            if (hasKey == false)
            {
                CartItems[movieSessionId] = 0;
            }

            CartItems[movieSessionId] += numTickets;
        }

        public void AddToCart(CartItem cartItem)
        {
            _AddToCart(cartItem.MovieSessionId, cartItem.TicketQty);
            Save();
        }

        public void AddToCart(int movieSessionId, int numTickets)
        {
            _AddToCart(movieSessionId, numTickets);
            Save();
        }

        public void AddMultipleItemsToCart(IEnumerable<CartItem> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                _AddToCart(cartItem.MovieSessionId, cartItem.TicketQty);
            }
            Save();
        }

        public bool ModifyNumTickets(int movieSessionId, int numTickets)
        {
            if (numTickets <= 0)
            {
                return false;
            }

            var hasKey = CartItems.ContainsKey(movieSessionId);
            if (hasKey)
            {
                CartItems[movieSessionId] = numTickets;
                Save();
                return true;
            }

            return false;
        }

        public bool RemoveFromCart(int movieSessionId)
        {
            if (CartItems.Remove(movieSessionId))
            {
                Save();
                return true;
            }
            return false;
        }

        public Entities.Cart GetCart()
        {
            var movieSessionIds = CartItems.Select(kvp => kvp.Key);
            var movieSessions = _movieSessionRepository.FindByMultipleIds(movieSessionIds.ToArray());

            var movieIds = movieSessions.Select(ms => ms.MovieId).Distinct();
            var movies = _movieRepository.FindByMultipleIds(movieIds.ToArray());

            var theaterIds = movieSessions.Select(ms => ms.TheaterId).Distinct();
            var theaters = _theaterRepository.FindAll().Where(t => theaterIds.Contains(t.Id));

            // include movie and theater information in each movie sessions
            foreach (var movieSession in movieSessions)
            {
                movieSession.Movie = movies.FirstOrDefault(m => m.Id == movieSession.MovieId);
                movieSession.Theater = theaters.FirstOrDefault(t => t.Id == movieSession.TheaterId);
            }

            var tickets = new List<Ticket>();
            foreach (var cartItem in CartItems)
            {
                for (int i = 0; i < cartItem.Value; i++)
                {
                    var ticket = new Ticket
                    {
                        MovieSessionId = cartItem.Key,
                        MovieSession = movieSessions.FirstOrDefault(ms => ms.Id == cartItem.Key)
                    };
                    tickets.Add(ticket);
                }
            }

            return new Entities.Cart { Tickets = tickets };

        }
    }
}
