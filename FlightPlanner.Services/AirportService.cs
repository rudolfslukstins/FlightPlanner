using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Services
{
    public class AirportService : EntityService<Airport>, IAirportService
    {
        public AirportService(FlightPlannerDbContext context) : base(context)
        {
        }

        public List<Airport> SearchAirport(string search)
        {
            var airportList = new List<Airport>();
            var airportArray = _context.Flights
                .Include(f => f.From)
                .Include(f => f.To)
                .ToList();
            var keyword = search.ToUpper().Trim();

            foreach (var flight in airportArray)
            {
                if (flight.From.AirportCode.ToUpper().Contains(keyword) ||
                    flight.From.City.ToUpper().Contains(keyword) ||
                    flight.From.Country.ToUpper().Contains(keyword))
                {
                    airportList.Add(flight.From);
                }

                if (flight.To.AirportCode.ToUpper().Contains(keyword) ||
                    flight.To.City.ToUpper().Contains(keyword) ||
                    flight.To.Country.ToUpper().Contains(keyword))
                {
                    airportList.Add(flight.To);
                }
            }
            return airportList;
        }
    }
}