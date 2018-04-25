using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WMoSS.Entities
{
    public class Theater
    {
        public const int DEFAULT_CAPACITY = 50;

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Capacity { get; set; } = 50;

        [Required]
        public string Address { get; set; }
        
        [NotMapped]
        public Seat[] Seats
        {
            get
            {
                var seats = new List<Seat>();
                var maxCol = 10;
                var maxRow = 'E';

                var columnPerGroup = 5;

                for (char row = 'A'; row <= maxRow; row++)
                {
                    for (int col = 1; col <= maxCol; col++)
                    {
                        seats.Add(new Seat
                        {
                            Column = col,
                            Row = row,
                            Group = 1 + (col % columnPerGroup)
                        });
                    }
                }

                return seats.ToArray();
            }
        }

        public IEnumerable<IGrouping<int, Seat>> SeatingArrangement => Seats.GroupBy(s => s.Group);
    }

    public class Seat
    {
        public int Column { get; set; }
        public char Row { get; set; }
        public int Group { get; set; }
    }
}
