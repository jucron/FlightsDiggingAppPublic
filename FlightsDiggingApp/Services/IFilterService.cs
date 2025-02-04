using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface IFilterService
    {
        public RoundtripsResponseDTO FilterFlightsFromGetRoundtripsResponseDTO(Filter filter, RoundtripsResponseDTO getRoundtripsResponseDTO);
    }
}
