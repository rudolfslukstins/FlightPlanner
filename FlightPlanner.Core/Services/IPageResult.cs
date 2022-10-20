using FlightPlanner.Core.Models;
using FlightPlanner.Models;

namespace FlightPlanner.Core.Services
{
    public interface IPageResult : IEntityService<PageResult>
    {
        PageResult NewPageResult(SearchFlightRequest search);
    }
}