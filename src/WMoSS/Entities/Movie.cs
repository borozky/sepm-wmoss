using System;
using System.Collections.Generic;
using System.Text;

namespace WMoSS.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ReleaseYear { get; set; }
        public string Genre { get; set; }
        public string Classification { get; set; }
        public double? Rating { get; set; }
        public string PosterFileName { get; set; }
        public int? RuntimeMinutes { get; set; }
        public string Description { get; set; }

        public IEnumerable<MovieSession> MovieSessions { get; set; }

    }
}
