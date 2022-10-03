using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [ApiController, Authorize]
    public class AdminApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        public AdminApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            var flight = _context.Flights
                .Include(f => f.To)
                .Include(f => f.From)
                .FirstOrDefault(f => f.Id == id);
            //var flight = FlightStorage.GetFlight(id);
             
             if (flight == null)
             {
                 return NotFound();
             }
             
             return Ok(flight);
        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            _context.SaveChanges();
            
            /*if (FlightStorage.IsWrongValues(flight) ||
                flight.From.AirportCode.Trim().ToUpper().Equals(flight.To.AirportCode.Trim().ToUpper()) ||
                DateTime.Parse(flight.DepartureTime) >= DateTime.Parse(flight.ArrivalTime))
            {
                return BadRequest();
            }

            if (FlightStorage.IsExistingFlight(flight))
            {
               return Conflict();
            }

            flight = FlightStorage.AddFlight(flight);*/
            return Created("", flight);
        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            var flight = _context.Flights.FirstOrDefault(f => f.Id == id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                _context.SaveChanges();
            }
            //FlightStorage.DeleteFlight(id);
            return Ok(id);
        }
    }
}
