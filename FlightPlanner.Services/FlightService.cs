using System.Linq;
using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Services
{
    public class FlightService : EntityService<Flight>, IFlightService
    {
        public FlightService(IFlightDbContext context) : base((FlightPlannerDbContext)context)
        {
        }

        public Flight GetCompleteFlightById(int id)
        {
           return _context.Flights.Include(f => f.From)
                .Include(f => f.To)
                .SingleOrDefault(f => f.Id == id);
        }

        public bool Exists(Flight flight)
        {
            return _context.Flights.Any(f => f.ArrivalTime == flight.ArrivalTime &&
                                             f.Carrier == flight.Carrier &&
                                             f.DepartureTime == flight.DepartureTime &&
                                             f.From.AirportCode == flight.From.AirportCode &&
                                             f.To.AirportCode == flight.To.AirportCode);
        }

        public void DeleteAll()
        {
            _context.Flights.RemoveRange(_context.Flights);
            _context.Airports.RemoveRange(_context.Airports);
            _context.SaveChanges();
        }
    }
}