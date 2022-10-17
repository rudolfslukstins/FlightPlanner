using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiFlightController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        private static readonly object _lock = new object();

        public CustomerApiFlightController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            var airportList = new List<Airport>();
            var keyword = search.ToUpper().Trim();

            var airportArray = _context.Flights
                .Include(f => f.From)
                .Include(f => f.To)
                .ToList();

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

            return Ok(airportList.ToArray());
        }

        [Route("flights/search")]
        [HttpPost]
        public IActionResult SearchFlights(SearchFlightRequest search)
        {
            if (search.From == null ||
                search.To == null ||
                search.DepartureDate == null || search.From == search.To)
            {
                return BadRequest();
            }

            lock (_lock)
            {
                var pages = new PageResult
                {
                    Items = _context.Flights
                        .Include(f => f.To)
                        .Include(f => f.From).ToList()
                        .Where(f => f.From.AirportCode == search.From &&
                                    f.To.AirportCode == search.To &&
                                    DateTime.Parse(f.DepartureTime).Date == DateTime.Parse(search.DepartureDate).Date)
                        .ToList()
                };

                return Ok(pages);
            }
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult SearchFlightById(int id)
        {
            var search = _context.Flights
                .Include(f => f.From)
                .Include(f => f.To)
                .FirstOrDefault(f => f.Id == id);

            if (search == null)
            {
                return NotFound();
            }

            return Ok(search);
        }
    }
}
