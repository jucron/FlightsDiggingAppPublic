using System.Linq;
using System.Text;
using System.Text.Json;
using FlightsDiggingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightsDiggingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlightsDiggerController : ControllerBase
    {
        private readonly ILogger<FlightsDiggerController> _logger;
        private readonly string _rapidapi_key = "441f8260camsh5ee529fad4a52c9p1cadf2jsnd01e83b82152";

        public FlightsDiggerController(ILogger<FlightsDiggerController> logger)
        {
            _logger = logger;
        }


        [HttpGet(Name = "FlightsDigger")]
        public string GetFlights()
        {
            string resultToReturn = "no processing";

            var initDepartDate = "2025-06-01";
            var endDepartDate = "2025-06-10";

            var initReturnDate = "2025-06-15";
            var endReturnDate = "2025-06-28";

            var from = "GIG";
            var to = "OPO";
            var currency = "BRL";
            var departDate = "2025-06-06";
            var returnDate = "2025-06-25";
            var limitFlightHour = 16;

            string sessionId;

            resultToReturn = $"From {from} to {to}: Departure: {departDate} Return date: {returnDate}. Limit flight time: {limitFlightHour}\n" ;

            var resultSearchRoundTrip = ExecuteSearchRoundTripAsync(from, to, currency, departDate, returnDate);
            
            if (resultSearchRoundTrip.Result == null || resultSearchRoundTrip.Result.data == null)
            {
                _logger.LogInformation($"ExecuteSearchRoundTripAsync with null objects: {resultSearchRoundTrip.Result.ToString()}");
                return resultToReturn;
            }
            var searchRoundTripData = resultSearchRoundTrip.Result.data;
            _logger.LogInformation($"resultSearchRoundTrip Status: {searchRoundTripData.context.status}");

            if (searchRoundTripData.context.status == "failure")
            {
                return resultToReturn;
            } 
            else if (searchRoundTripData.context.status == "incomplete")
            {
                sessionId = searchRoundTripData.context.sessionId.TrimEnd('=');

                _logger.LogInformation($"sessionId: {sessionId}");

                // tries 5 times
                int maxTries = 5;
                for (int i = 1; i <= maxTries; i++)
                {
                    var delayMilliseconds = 5000;
                    _logger.LogInformation($"Trying ExecuteSearchIncompleteAsync number: {i} of {maxTries}, with delay of {delayMilliseconds/1000} seconds");

                    // wait 
                    Task.Delay(delayMilliseconds).Wait();

                    var resultSearchIncomplete = ExecuteSearchIncompleteAsync(sessionId, currency);

                    var searchIncompleteData = resultSearchIncomplete?.Result?.data;
                    _logger.LogInformation($"resultSearchIncomplete Status: {searchIncompleteData?.context?.status}");

                    if (searchIncompleteData?.context.status == "complete")
                    {
                        resultToReturn += ProcessResult(resultSearchIncomplete.Result, limitFlightHour);
                        break;

                    } else if (i == 5)
                    {
                        return resultToReturn;
                    }
                }

            }

            return resultToReturn;
            
            
        }

        private string ProcessResult(SearchIncompleteResponse result, int limitFlightHour)
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
                itinerary.legs.ForEach(l => durationHours = +l.durationInMinutes/60);
                if (durationHours > limitFlightHour)
                {
                    continue;
                }

                var price = itinerary.price.raw;
                if (itinerary.legs.Count > 0 && itinerary.legs[0].carriers != null && itinerary.legs[0].carriers.marketing.Count >0)
                {
                    company = itinerary.legs[0].carriers.marketing[0].name;
                }
                resultProcessed.Append($"Price: {price} - Hours: {durationHours} - Company: {company}\n" );
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
                _logger.LogInformation("Error in ExecuteSearchRoundTripAsync "+ ex.ToString());
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
                //_logger.LogInformation("Content in jsonString: " + jsonString);
                _logger.LogInformation("Error in ExecuteSearchIncompleteAsync " + ex.ToString());
            }
            return new SearchIncompleteResponse();
        }
    }
}
