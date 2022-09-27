using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightPlanner
{
    public class FlightStorage
    {
        private static List<Flight> _flights = new List<Flight>();
        private static int _id = 0;
        private static object _balanceLock = new ();

        public static Flight AddFlight(Flight flight)
        {
            lock (_balanceLock)
            {
                flight.Id = ++_id;
                _flights.Add(flight);
            }
            return flight;
        }

        public static Flight GetFlight(int id)
        {
            return _flights.FirstOrDefault(f => f.Id == id);
        }

        public static void Clear()
        {
            lock (_balanceLock)
            {
                _flights.Clear();
                _id = 0;
            }
        }

        public static bool IsExistingFlight(Flight flight)
        {
            lock (_balanceLock)
            {
                foreach (var f in _flights)
                {
                    if (flight.ArrivalTime == f.ArrivalTime &&
                        flight.DepartureTime == f.DepartureTime &&
                        flight.From.City == f.From.City &&
                        flight.From.AirportCode == f.From.AirportCode &&
                        flight.From.Country == f.From.Country &&
                        flight.Carrier == f.Carrier &&
                        flight.To.AirportCode == f.To.AirportCode &&
                        flight.To.Country == f.To.Country &&
                        flight.To.City == f.To.City)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static bool IsWrongValues(Flight flight)
        {
            if (flight.From == null ||
                flight.To == null ||
                string.IsNullOrEmpty(flight.To.Country) ||
                string.IsNullOrEmpty(flight.To.AirportCode) ||
                string.IsNullOrEmpty(flight.To.City) ||
                string.IsNullOrEmpty(flight.From.Country) ||
                string.IsNullOrEmpty(flight.From.AirportCode) ||
                string.IsNullOrEmpty(flight.From.City) ||
                string.IsNullOrEmpty(flight.ArrivalTime) ||
                string.IsNullOrEmpty(flight.Carrier) ||
                string.IsNullOrEmpty(flight.DepartureTime))
            {
                return true;
            }

            return false;
        }

        public static void DeleteFlight(int id)
        {
            lock (_balanceLock)
            {
                _flights.RemoveAll(f => f.Id == id);
            }
        }

        public static Airport[] SearchAirport(string keyword)
        {
            lock (_balanceLock)
            {
                List<Airport> airportList = new List<Airport>();
                keyword = keyword.ToUpper().Trim();

                foreach (var flight in _flights)
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
                return airportList.ToArray();
            }
        }

        public static List<Flight> SearchFlights(SearchFlightRequest search)
        {
            return _flights.Where(x => x.From.AirportCode == search.From &&
                                       x.To.AirportCode == search.To &&
                                       DateTime.Parse(x.DepartureTime).Date == DateTime.Parse(search.DepartureDate).Date).ToList();
        }

    }
}
