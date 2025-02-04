using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface IFilterService
    {
        public GetRoundtripsResponseDTO FilterFlightsFromGetRoundtripsResponseDTO(Filter filter, GetRoundtripsResponseDTO getRoundtripsResponseDTO);
    }
}
