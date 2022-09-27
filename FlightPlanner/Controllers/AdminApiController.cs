using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [ApiController, Authorize]
    public class AdminApiController : ControllerBase
    {
        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            var flight = FlightStorage.GetFlight(id);
            
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
            if (FlightStorage.IsWrongValues(flight) ||
                flight.From.AirportCode.Trim().ToUpper().Equals(flight.To.AirportCode.Trim().ToUpper()) ||
                DateTime.Parse(flight.DepartureTime) >= DateTime.Parse(flight.ArrivalTime))
            {
                return BadRequest();
            }

            if (FlightStorage.IsExistingFlight(flight))
            {
               return Conflict();
            }

            flight = FlightStorage.AddFlight(flight);
            return Created("", flight);
        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            FlightStorage.DeleteFlight(id);
            return Ok(id);
        }
    }
}
