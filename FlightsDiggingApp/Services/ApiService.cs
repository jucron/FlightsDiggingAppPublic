using FlightsDiggingApp.Models;
using System.Text.Json;
using System.Text;
using FlightsDiggingApp.Controllers;
using Microsoft.OpenApi.Any;

namespace FlightsDiggingApp.Services
{
    public class ApiService
    {
        private readonly string _rapidapi_key = "441f8260camsh5ee529fad4a52c9p1cadf2jsnd01e83b82152";
        private readonly ILogger _logger;

        public ApiService(ILogger logger)
        {
            _logger = logger;
        }

        public string getRoundTrip(string from, string to, string currency, string departDateString, string returnDateString, int limitFlightHour, CancellationToken cancellationToken)
        {
            string sessionId;
            string resultToReturn = "";

            var resultSearchRoundTrip = ExecuteSearchRoundTripAsync(from, to, currency, departDateString, returnDateString);

            if (resultSearchRoundTrip.Result == null || resultSearchRoundTrip.Result.data == null)
            {
                //_logger.LogInformation($"ExecuteSearchRoundTripAsync with null objects: {resultSearchRoundTrip.Result.ToString()}");
                resultToReturn = "error\n";
                return resultToReturn;
            }
            var searchRoundTripData = resultSearchRoundTrip.Result.data;
            //_logger.LogInformation($"resultSearchRoundTrip Status: {searchRoundTripData.context.status}");

            if (searchRoundTripData.context.status == "failure")
            {
                resultToReturn = "error\n";
                return resultToReturn;
            }
            else if (searchRoundTripData.context.status == "complete")
            {
                resultToReturn += ProcessResultRoundTrip(searchRoundTripData, limitFlightHour);
            }
            else if (searchRoundTripData.context.status == "incomplete")
            {
                sessionId = searchRoundTripData.context.sessionId.TrimEnd('=');

                //_logger.LogInformation($"sessionId: {sessionId}");

                // tries 5 times
                int maxTries = 5;
                var delayMillisecondsDefault = 5000;

                for (int i = 1; i <= maxTries; i++)
                {
                    // Check if process is cancelled by Client
                    cancellationToken.ThrowIfCancellationRequested();

                    var delayMilliseconds = delayMillisecondsDefault * Math.Max(1, (maxTries - i - 1));

                    _logger.LogInformation($"Trying ExecuteSearchIncompleteAsync number: {i} of {maxTries}, with delay of {delayMilliseconds / 1000} seconds");

                    // wait 
                    Task.Delay(delayMilliseconds).Wait();

                    var resultSearchIncomplete = ExecuteSearchIncompleteAsync(sessionId, currency);

                    var searchIncompleteData = resultSearchIncomplete?.Result?.data;
                    //_logger.LogInformation($"resultSearchIncomplete Status: {searchIncompleteData?.context?.status}");

                    if (searchIncompleteData?.context.status == "complete")
                    {
                        resultToReturn += ProcessResultSearchIncomplete(resultSearchIncomplete.Result, limitFlightHour);
                        break;

                    }
                    else if (i == 5)
                    {
                        return resultToReturn;
                    }
                }
            }

            return resultToReturn;
        }

        private string ProcessResultRoundTrip(SearchRoundTripResponse.Data result, int limitFlightHour)
        {
            StringBuilder resultProcessed = new StringBuilder();

            // For each itineraty:
            int count = 0;
            int countLimit = 5;
            foreach (var itinerary in result.itineraries)
            {
                if (count > countLimit)
                {
                    break;
                }
                // Add count
                count++;

                var company = "not defined";

                // Sum duration of all legs
                int durationHours = 0;
                itinerary.legs.ForEach(l => durationHours = +l.durationInMinutes / 60);
                if (durationHours > limitFlightHour)
                {
                    continue;
                }

                var price = itinerary.price.raw;
                if (itinerary.legs.Count > 0 && itinerary.legs[0].carriers != null && itinerary.legs[0].carriers.marketing.Count > 0)
                {
                    company = itinerary.legs[0].carriers.marketing[0].name;
                }
                resultProcessed.Append($"Price: {price} - Hours: {durationHours} - Company: {company}\n");
            }


            return resultProcessed.ToString();
        }

        private string ProcessResultSearchIncomplete(SearchIncompleteResponse result, int limitFlightHour)
        {
            StringBuilder resultProcessed = new StringBuilder();

            // For each itineraty:
            int count = 0;
            int countLimit = 5;
            foreach (var itinerary in result.data.itineraries)
            {
                if (count > countLimit)
                {
                    break;
                }
                // Add count
                count++;

                var company = "not defined";

                // Sum duration of all legs
                int durationHours = 0;
                itinerary.legs.ForEach(l => durationHours = +l.durationInMinutes / 60);
                if (durationHours > limitFlightHour)
                {
                    continue;
                }

                var price = itinerary.price.raw;
                if (itinerary.legs.Count > 0 && itinerary.legs[0].carriers != null && itinerary.legs[0].carriers.marketing.Count > 0)
                {
                    company = itinerary.legs[0].carriers.marketing[0].name;
                }
                resultProcessed.Append($"Price: {price} - Hours: {durationHours} - Company: {company}\n");
            }


            return resultProcessed.ToString();
        }

        private async Task<SearchRoundTripResponse> ExecuteSearchRoundTripAsync(string from, string to, string currency, string departDate, string returnDate)
        {
            var client = new HttpClient();
            var uri = $"https://sky-scanner3.p.rapidapi.com/flights/search-roundtrip?fromEntityId={from}&toEntityId={to}&departDate={departDate}&returnDate={returnDate}&currency={currency}&stops=direct%2C1stop&adults=2&sort=cheapest_first";
            //_logger.LogInformation($"ExecuteSearchRoundTripAsync with URI: {uri}");

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

        private async Task<SearchIncompleteResponse> ExecuteSearchIncompleteAsync(string sessionId, string currency)
        {
            var client = new HttpClient();
            var uri = $"https://sky-scanner3.p.rapidapi.com/flights/search-incomplete?sessionId={sessionId}&stops=direct%2C1stop&sort=cheapest_first&currency={currency}";
            //_logger.LogInformation($"ExecuteSearchIncompleteAsync with URI: {uri}");


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
            string jsonString = "tbd";
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
    }
}
