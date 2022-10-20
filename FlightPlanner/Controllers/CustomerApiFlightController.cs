using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Core.Validations;
using FlightPlanner.Data;
using FlightPlanner.Models;
using FlightPlanner.Models.Validations;
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
        private readonly IFlightService _flightService;
        private readonly IAirportService _airportService;
        private readonly IFlightRequestValidators _flightRequestValidatorsValidator;
        private readonly IPageResult _pageResult;
        private readonly IEnumerable<IAirportValidator> _airportValidators;
        private readonly IMapper _mapper;
        private static readonly object _lock = new object();

        public CustomerApiFlightController( 
            IFlightService flightService,
            IFlightRequestValidators flightRequestValidatorsValidator,
            IPageResult pageResult,
            IEnumerable<IAirportValidator> airportValidators,
            IAirportService airportService,
            IMapper mapper)
        {
            _flightService = flightService;
            _flightRequestValidatorsValidator = flightRequestValidatorsValidator;
            _pageResult = pageResult;
            _airportValidators = airportValidators;
            _airportService = airportService;
            _mapper = mapper;

        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            var airports = _airportService.SearchAirport(search);
            var response = airports.Select(a => _mapper.Map<AirportRequest>(a)).ToList();
            
            return Ok(response.ToArray());
        }

        [Route("flights/search")]
        [HttpPost]
        public IActionResult SearchFlights(SearchFlightRequest search)
        {
            if (_flightRequestValidatorsValidator.IsValid(search))
            {
                return BadRequest();
            }

            lock (_lock)
            {
                var pages = _pageResult.NewPageResult(search);
                
                return Ok(pages);
            }
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            var search = _flightService.GetCompleteFlightById(id);

            if (search == null)
            {
                return NotFound(id);
            }
            var response = _mapper.Map<FlightRequest>(search);

            return Ok(response);
        }
    }
}
