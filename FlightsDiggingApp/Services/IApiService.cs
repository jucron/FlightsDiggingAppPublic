using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface IApiService
    {
        public Task<AirportsResponseDTO> GetAirportsAsync(string query, int tries = 3);

        public Task<RoundtripsResponse> GetRoundtripAsync(RoundtripsRequest request, int tries = 3, string errorDescription = "Unexpected error/status");
    }
}
