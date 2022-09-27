using System.Collections.Generic;

namespace FlightPlanner
{
    public class PageResult
    { 
        public int page { get; set; }
        public List<Flight> Items { get; set; }
        public int totalItems => Items.Count;
    }
}
