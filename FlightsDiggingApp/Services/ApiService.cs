using FlightsDiggingApp.Models;
using System.Text.Json;
using System.Text;
using FlightsDiggingApp.Controllers;
using Microsoft.OpenApi.Any;
using System.Threading;
using FlightsDiggingApp.Mappers;

namespace FlightsDiggingApp.Services
{
    public class ApiService : IApiService
    {
        private readonly string _rapidapi_key = "441f8260camsh5ee529fad4a52c9p1cadf2jsnd01e83b82152";
        private readonly ILogger<ApiService> _logger;

        public ApiService(ILogger<ApiService> logger)
        {
            _logger = logger;
        }

        public async Task<GetAirportsResponse> GetAirportsAsync(string query)
        {
            _logger.LogInformation($"GetAirportsAsync with query: {query}");
            //trim whitespaces of query at ends
            query = query.Trim();
            query = query.Replace(" ", "%20");
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://sky-scanner3.p.rapidapi.com/flights/auto-complete?query={query}"),
                Headers =
                {
                    { "x-rapidapi-key", _rapidapi_key },
                    { "x-rapidapi-host", "sky-scanner3.p.rapidapi.com" },
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
                        GetAirportsResponse finalResponse = JsonSerializer.Deserialize<GetAirportsResponse>(jsonString);
                        if (finalResponse != null)
                        {
                            return finalResponse;
                        }
                    }
                    return new GetAirportsResponse() { message = "error" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in ExecuteSearchIncompleteAsync " + ex.ToString());
                return new GetAirportsResponse() { message = "error" };
            }
        }

        public async Task<GetRoundtripsResponse> GetRoundtripAsync(GetRoundtripsRequest request)
        {
            var errorDescription = "Unexpected error/status";

            SearchRoundTripResponse resultSearchRoundTrip = await ExecuteSearchRoundTripAsync(request);

            if (resultSearchRoundTrip == null || resultSearchRoundTrip.data == null || resultSearchRoundTrip.data.context == null)
            {
                errorDescription = $"ExecuteSearchRoundTripAsync with NULL objects";
                _logger.LogInformation(errorDescription);
                return new GetRoundtripsResponse() { status = new () { hasError = true, errorDescription = errorDescription } };
            }
            var searchRoundTripData = resultSearchRoundTrip.data;
            //_logger.LogInformation($"resultSearchRoundTrip Status: {searchRoundTripData.context.status}");

            if (searchRoundTripData.context.status == "failure")
            {
                errorDescription = $"ExecuteSearchRoundTripAsync with status FAILURE";
                _logger.LogInformation(errorDescription);
                return new GetRoundtripsResponse() { status = new() { hasError = true, errorDescription = errorDescription } };
            }
            else if (searchRoundTripData.context.status == "complete")
            {
                // todo: handle complete responses from ExecuteSearchRoundTripAsync
                errorDescription = $"ExecuteSearchRoundTripAsync with status COMPLETE. How to handle this?";
                _logger.LogInformation(errorDescription);
                return new GetRoundtripsResponse() { status = new() { hasError = true, errorDescription = errorDescription } };
            }
            else if (searchRoundTripData.context.status == "incomplete")
            {
                request.sessionId = searchRoundTripData.context.sessionId.TrimEnd('=');

                //_logger.LogInformation($"sessionId: {sessionId}");

                // tries 5 times
                int maxTries = 5;
                var delayMillisecondsDefault = 5000;

                for (int i = 1; i <= maxTries; i++)
                {
                    var delayMilliseconds = delayMillisecondsDefault * Math.Max(1, (maxTries - i - 1));

                    _logger.LogInformation($"ExecuteSearchIncompleteAsync for dates: {request.departDate} -> {request.returnDate}. Try number: {i} of {maxTries}, with delay of {delayMilliseconds / 1000} seconds");

                    // wait 
                    Task.Delay(delayMilliseconds).Wait();

                    var resultSearchIncomplete = await ExecuteSearchIncompleteAsync(request);

                    if (resultSearchIncomplete == null || resultSearchIncomplete.data == null || resultSearchIncomplete.data.context == null)
                    {
                        errorDescription = $"ExecuteSearchIncompleteAsync with NULL objects";
                        _logger.LogInformation(errorDescription);
                        return new GetRoundtripsResponse() { status = new() { hasError = true, errorDescription = errorDescription } };
                    }

                    var searchIncompleteData = resultSearchIncomplete.data;
                    //_logger.LogInformation($"resultSearchIncomplete Status: {searchIncompleteData?.context?.status}");

                    if (searchIncompleteData?.context.status == "complete")
                    {
                        return GetRoundtripsMapper.MapSearchIncompleteResponseToGetRoundtripsResponse(resultSearchIncomplete, request);

                    }
                    else if (i >= 5)
                    {
                        errorDescription = $"ExecuteSearchRoundTripAsync run out of maxTries.";
                        _logger.LogInformation(errorDescription);
                        return new GetRoundtripsResponse() { status = new() { hasError = true, errorDescription = errorDescription } };
                    }
                }
            }
            _logger.LogInformation(errorDescription);
            return new GetRoundtripsResponse() { status = new() { hasError = true, errorDescription = errorDescription } };
        }

        private async Task<SearchIncompleteResponse> ExecuteSearchIncompleteAsync(GetRoundtripsRequest roundTripRequest)
        {
            // Data to be send
            var sessionId = roundTripRequest.sessionId;
            var currency = roundTripRequest.currency;

            var client = new HttpClient();
            var uri = $"https://sky-scanner3.p.rapidapi.com/flights/search-incomplete?sessionId={sessionId}&sort=cheapest_first&currency={currency}";
            _logger.LogInformation($"ExecuteSearchIncompleteAsync with URI: {uri}");


            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
                Headers =
                    {
                        { "x-rapidapi-key", _rapidapi_key },
                        { "x-rapidapi-host", "sky-scanner3.p.rapidapi.com" },
                    },
            };
            string jsonString = "not defined yet";
            try
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    //return await response.Content.ReadAsStringAsync();
                    jsonString = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<SearchIncompleteResponse>(jsonString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in ExecuteSearchIncompleteAsync " + ex.ToString());
                _logger.LogInformation("Content in jsonString: " + jsonString);
            }
            return new SearchIncompleteResponse();
        }

        private async Task<SearchRoundTripResponse> ExecuteSearchRoundTripAsync(GetRoundtripsRequest roundTripRequest)
        {
            // Data to be send
            var from = roundTripRequest.from;
            var to = roundTripRequest.to;
            var departDate = roundTripRequest.departDate;
            var returnDate = roundTripRequest.returnDate;
            var currency = roundTripRequest.currency;
            var adults = roundTripRequest.adults;
            var cabinclass = roundTripRequest.cabinclass;
            var infants = roundTripRequest.infants;
            var children = roundTripRequest.children.Count;

            //https://sky-scanner3.p.rapidapi.com/flights/search-roundtrip?fromEntityId=GIG&toEntityId=OPO&departDate=2025-04-16&returnDate=2025-04-18&currency=BRL&adults=2&infants=1&cabinClass=economy&children=2&sort=cheapest_first
            
            var client = new HttpClient();
            var uri = $"https://sky-scanner3.p.rapidapi.com/flights/search-roundtrip?fromEntityId={from}&toEntityId={to}&departDate={departDate}&returnDate={returnDate}&currency={currency}&adults={adults}&cabinClass={cabinclass}&infants={infants}&children={children}&sort=cheapest_first";
            _logger.LogInformation($"ExecuteSearchRoundTripAsync with URI: {uri}");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
                Headers =
                    {
                        { "x-rapidapi-key", _rapidapi_key },
                        { "x-rapidapi-host", "sky-scanner3.p.rapidapi.com" },
                    },
            };

            try
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    //return await response.Content.ReadAsStringAsync();
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SearchRoundTripResponse>(jsonString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in ExecuteSearchRoundTripAsync " + ex.ToString());
            }
            return new SearchRoundTripResponse();
        }
    }
}
