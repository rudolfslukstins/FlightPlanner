using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiFlightController : ControllerBase
    {
        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            var airportArray = FlightStorage.SearchAirport(search);
            return Ok(airportArray);
        }

        [Route("flights/search")]
        [HttpPost]
        public IActionResult SearchFlights(SearchFlightRequest search)
        {
            if (search.From == null ||
                search.To == null ||
                search.DepartureDate == null || search.From == search.To)
            {
                return BadRequest(search);
            }

            var pages = new PageResult
            {
                Items = FlightStorage.SearchFlights(search)
            };
            
            return Ok(pages);
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult SearchFlightById(int id)
        {
            var search = FlightStorage.GetFlight(id);
            return search == null ? NotFound(id) : Ok(search);
        }
    }
}
