using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services.Filters
{
    public interface IFilterService
    {
        public RoundtripResponseDTO FilterRoundtripResponseDTO(Filter filter, RoundtripResponseDTO responseDTO);
        public void ApplyMetrics(RoundtripResponseDTO responseDTO, bool isFiltered = false);
    }
}
