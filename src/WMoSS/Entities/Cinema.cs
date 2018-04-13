using System;
using System.Collections.Generic;
using System.Text;

namespace WMoSS.Entities
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public IEnumerable<MovieSession> MovieSessions { get; set; }
        public IEnumerable<Seat> Seats { get; set; }
    }
}
