namespace FlightPlanner.Models.Validations
{
    public class FlightRequestValidators : IFlightRequestValidators
    {
        public bool IsValid(SearchFlightRequest flight)
        {
            return flight.From == null ||
                   flight.To == null ||
                   flight.DepartureDate == null ||
                   flight.From == flight.To;
        }
    }
}
