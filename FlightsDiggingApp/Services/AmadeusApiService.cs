using FlightsDiggingApp.Mappers;
using System.Text.Json;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Models.RapidApi;
using FlightsDiggingApp.Properties;
using FlightsDiggingApp.Helpers;
using FlightsDiggingApp.Models.Amadeus;
using System.Net;

namespace FlightsDiggingApp.Services
{
    public class AmadeusApiService : IApiService
    {
        private readonly ILogger<AmadeusApiService> _logger;
        private readonly IAuthService _authService;

        public AmadeusApiService(ILogger<AmadeusApiService> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public async Task<IApiServiceResponse> GetAirportsAsync(string query, int tries = 3)
        {
            var triesLeft = tries-1;
            _logger.LogInformation($"GetAirportsAsync with query: <{query}>. Tries left: {triesLeft}");

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };

            var parameters = new Dictionary<string, string>
            {
                { "keyword", query },
                { "subType", "CITY,AIRPORT" }
            };

            var bearerToken = _authService.GetToken();

            var baseUrl = "https://test.api.amadeus.com/v1/reference-data/locations";

            var apiResponse = await ApiCallUtility.GetAsync<AmadeusAirportResponse>(baseUrl,parameters,headers, bearerToken);

            if (apiResponse.operationStatus.hasError || apiResponse.data == null)
            {
                _logger.LogError("Error in GetAirportsAsync: " + apiResponse.operationStatus.errorDescription);

                // If tries still available
                if (triesLeft > 0)
                {
                    // If token expired, clear it from cache. The getToken will renew in the next call
                    if (apiResponse.operationStatus.status.httpStatus == HttpStatusCode.Unauthorized)
                    {
                        _authService.ClearToken();
                    }
                    Task.Delay(2000).Wait();
                    return await GetAirportsAsync(query, triesLeft);
                }
                // API tries expired
                return new AmadeusAirportResponse() { operationStatus = apiResponse.operationStatus };
            }
            apiResponse.data.operationStatus = apiResponse.operationStatus;
            return apiResponse.data;
        }

        public Task<IApiServiceResponse> GetRoundtripAsync(RoundtripsRequest request, int tries = 3, string errorDescription = "Unexpected error/operationStatus")
        {
            throw new NotImplementedException();
        }
    }
}
