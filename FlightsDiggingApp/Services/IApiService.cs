using FlightsDiggingApp.Models;
using FlightsDiggingApp.Models.RapidApi;

namespace FlightsDiggingApp.Services
{
    public interface IApiService
    {
        public Task<IApiServiceResponse> GetAirportsAsync(string query, int tries = 3);

        public Task<IApiServiceResponse> GetRoundtripAsync(RoundtripsRequest request, int tries = 3, string errorDescription = "Unexpected error/status");
    }
}
