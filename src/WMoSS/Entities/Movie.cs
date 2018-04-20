using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WMoSS.Entities
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime? ReleaseDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? RuntimeMinutes { get; set; }
        
        public string Genre { get; set; }
        
        public string Classification { get; set; }
        
        public string PosterFileName { get; set; }

        [Range(0.0, 5.0)]
        public double? Rating { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        
        public IEnumerable<MovieSession> MovieSessions { get; set; }
        
    }
}
