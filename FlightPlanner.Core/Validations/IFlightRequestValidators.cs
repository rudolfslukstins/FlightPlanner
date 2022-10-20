namespace FlightPlanner.Models.Validations
{
    public interface IFlightRequestValidators
    {
        bool IsValid(SearchFlightRequest flight);
    }
}