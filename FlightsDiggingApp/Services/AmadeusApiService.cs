using FlightsDiggingApp.Mappers;
using System.Text.Json;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Models.RapidApi;
using FlightsDiggingApp.Properties;
using FlightsDiggingApp.Helpers;
using FlightsDiggingApp.Models.Amadeus;

namespace FlightsDiggingApp.Services
{
    public class AmadeusApiService : IApiService
    {
        private readonly string _amadeusKey = "441f8260camsh5ee529fad4a52c9p1cadf2jsnd01e83b82152";
        private readonly ILogger<AmadeusApiService> _logger;

        public AmadeusApiService(ILogger<AmadeusApiService> logger)
        {
            _logger = logger;
        }

        public async Task<IApiServiceResponse> GetAirportsAsync(string query, int tries = 3)
        {
            _logger.LogInformation($"GetAirportsAsync with query: <{query}>. Tries left: {tries - 1}");

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };

            var parameters = new Dictionary<string, string>
            {
                { "keyword", query },
                { "subType", "CITY,AIRPORT" }
            };

            var baseUrl = "https://test.api.amadeus.com/v1/reference-data/locations";

            var apiResponse = await ApiCallUtility.GetAsync<AmadeusAirportResponse>(baseUrl,parameters,headers);

            if (apiResponse.status.hasError || apiResponse.data == null)
            {
                _logger.LogError("Error in GetAirportsAsync: " + apiResponse.status.errorDescription);
                return new AmadeusAirportResponse() { Status = apiResponse.status};
            }
            return apiResponse.data;
        }

        public Task<IApiServiceResponse> GetRoundtripAsync(RoundtripsRequest request, int tries = 3, string errorDescription = "Unexpected error/status")
        {
            throw new NotImplementedException();
        }
    }
}
