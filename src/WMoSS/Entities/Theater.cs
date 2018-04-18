using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
