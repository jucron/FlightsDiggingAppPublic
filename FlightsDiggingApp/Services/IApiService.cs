using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface IApiService
    {
        public Task<GetAirportsResponse> GetAirportsAsync(string query);

        public Task<GetRoundtripsResponse> GetRoundtripAsync(GetRoundtripsRequest request);
    }
}
