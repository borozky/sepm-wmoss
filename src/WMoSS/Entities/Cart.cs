
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMoSS.Entities
{
    public class Cart
    {
        public ICollection<Ticket> Tickets { get; set; }

        public IEnumerable<Movie> Movies =>
            MovieSessions.Select(ms => ms.Movie).GroupBy(m => m.Id).Select(g => g.First());

        public IEnumerable<MovieSession> MovieSessions => 
            Tickets.Select(t => t.MovieSession).GroupBy(m => m.Id).Select(g => g.First());

        public IEnumerable<int> MovieSessionIds => 
            Tickets.Select(t => t.MovieSessionId).Distinct();

        public IEnumerable<IGrouping<int, MovieSession>> MovieSessionByMovieId =>
            MovieSessions.GroupBy(ms => ms.MovieId);

        public IEnumerable<IGrouping<int, Ticket>> TicketsByMovieSessionId =>
            Tickets.GroupBy(ticket => ticket.MovieSessionId);


        public IEnumerable<CartItem> CartItems
        {
            get
            {
                var movieSessionsFromTickets = MovieSessions;
                return TicketsByMovieSessionId.Select(g => new CartItem
                {
                    MovieSessionId = g.Key,
                    MovieSession = movieSessionsFromTickets.FirstOrDefault(ms => ms.Id == g.Key),
                    Tickets = g.AsEnumerable()
                });
            }
        }

        public IEnumerable<CartItemByMovie> CartItemsByMovie
        {
            get
            {
                var movies = Movies;
                return CartItems.GroupBy(c => c.MovieSession.MovieId).Select(g => new CartItemByMovie
                {
                    MovieId = g.Key,
                    Movie = movies.FirstOrDefault(m => m.Id == g.Key),
                    CartItemsByMovieSession = g.ToList()
                });

            }
        }
    }
}
