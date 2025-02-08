using FlightsDiggingApp.Mappers;
using System.Text.Json;
using FlightsDiggingApp.Models;

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

        public async Task<AirportsResponseDTO> GetAirportsAsync(string query, int tries = 3)
        {
            _logger.LogInformation($"GetAirportsAsync with query: <{query}>. Tries left: {tries - 1}");
            //trim whitespaces of query at ends
            query = query.Trim();
            query = query.Replace(" ", "%20");
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://test.api.amadeus.com/v1/reference-data/locations?keyword={query}&subType=CITY,AIRPORT"),
                Headers =
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },
                    
                },
            };
            try
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (jsonString != null)
                    {
                        AirportsResponse finalResponse = JsonSerializer.Deserialize<AirportsResponse>(jsonString);
                        if (finalResponse != null)
                        {
                            finalResponse.operationStatus = OperationStatus.CreateStatusSuccess();
                            return AirportsMapper.MapGetAirportsResponseToDTO(finalResponse);
                        }
                    }
                    return new AirportsResponseDTO() { status = OperationStatus.CreateStatusFailure("Unexpected error -> jsonstring from response API is null") };
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in ExecuteSearchIncompleteAsync " + ex.ToString());
                if (tries > 1)
                {
                    return await GetAirportsAsync(query, tries - 1);
                }
                return new AirportsResponseDTO() { status = OperationStatus.CreateStatusFailure("Unexpected error -> " + ex.ToString()) };
            }
        }

        public Task<RoundtripsResponse> GetRoundtripAsync(RoundtripsRequest request, int tries = 3, string errorDescription = "Unexpected error/status")
        {
            throw new NotImplementedException();
        }
    }
}
