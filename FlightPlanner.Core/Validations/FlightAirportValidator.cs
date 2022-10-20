namespace FlightPlanner.Core.Validations
{
    public class FlightAirportValidator : IFlightValidator
    {
        public bool IsValid(Flight flight)
        {
            if (flight?.From != null && flight?.To != null)
            {
               return flight.From.AirportCode?.ToLower().Trim() 
                      != flight.To.AirportCode?.ToLower().Trim();
            }

            return false;
        }
    }
}