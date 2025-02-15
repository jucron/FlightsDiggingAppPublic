using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface IFilterService
    {
        public RoundtripResponseDTO FilterRoundtripResponseDTO(Filter filter, RoundtripResponseDTO responseDTO);
    }
}
