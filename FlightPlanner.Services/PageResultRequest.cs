using System;
using System.Linq;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using FlightPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Services
{
    public class PageResultRequest : EntityService<PageResult> , IPageResult
    {
        public PageResultRequest(FlightPlannerDbContext context) : base(context)
        {
        }

        public PageResult NewPageResult(SearchFlightRequest search)
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
            return pages;
        }
    }
}