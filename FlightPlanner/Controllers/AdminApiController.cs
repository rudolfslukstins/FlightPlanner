using AutoMapper;
using FlightPlanner.Core.Services;
using FlightPlanner.Core.Validations;
using FlightPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [ApiController, Authorize]
    public class AdminApiController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IEnumerable<IFlightValidator> _flightValidator;
        private readonly IEnumerable<IAirportValidator> _airportValidators;
        private readonly IMapper _mapper;
        private static readonly object _lock = new object();
        public AdminApiController(IFlightService flightService,
            IEnumerable<IAirportValidator> airportValidators,
            IEnumerable<IFlightValidator> flightValidator,
            IMapper mapper)
        {
            _flightService = flightService;
            _airportValidators = airportValidators;
            _flightValidator = flightValidator;
            _mapper = mapper;
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            var flight = _flightService.GetCompleteFlightById(id);

            if (flight == null)
            {
                return NotFound(id);
            }

            var response = _mapper.Map<FlightRequest>(flight);
            return Ok(response);
        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(FlightRequest request)
        {
            var flight = _mapper.Map<Flight>(request);
            if (!_flightValidator.All(f => f.IsValid(flight)) ||
                !_airportValidators.All(f => f.IsValid(flight?.From)) ||
                !_airportValidators.All(f => f.IsValid(flight?.To)))
            {
                return BadRequest();
            }

            lock (_lock)
            {
                if (_flightService.Exists(flight))
                {
                    return Conflict();
                }

                _flightService.Create(flight);
                request = _mapper.Map<FlightRequest>(flight);
                return Created("", request);
            }

        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            lock (_lock)
            {
                var flight = _flightService.GetById(id);

                if (flight != null)
                {
                    _flightService.Delete(flight);
                }

                return Ok(id);
            }
        }
    }
}
