using System.Collections.Generic;

namespace FlightPlanner.Core.Models
{
    public class PageResult : Entity
    {
        public int page { get; set; }
        public List<Flight> Items { get; set; }
        public int totalItems => Items.Count;
    }
}
