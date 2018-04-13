using System;
using System.Collections.Generic;
using System.Text;

namespace WMoSS.Entities
{
    public class Theater
    {
        public const string DEFAULT_ADDRESS = "123 Lygon Street Melbourne VIC 3000";
        public const int MAX_CAPACITY = 50;

        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; } = MAX_CAPACITY;
        public string Address { get; set; } = DEFAULT_ADDRESS;
    }
}
