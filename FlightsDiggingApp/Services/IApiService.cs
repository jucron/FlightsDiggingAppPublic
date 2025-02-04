using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface IApiService
    {
        public Task<GetAirportsResponse> GetAirportsAsync(string query, int tries = 3);

        public Task<GetRoundtripsResponse> GetRoundtripAsync(GetRoundtripsRequest request, int tries = 3, string errorDescription = "Unexpected error/status");
    }
}
