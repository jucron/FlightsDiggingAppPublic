
using System.Text.Json;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Services;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tests
{
    public class FilterServiceTest
    {
        private readonly ILogger<FilterService> _logger;
        private readonly FilterService _filterService;
        public FilterServiceTest(ITestOutputHelper output)
        {
            _logger = new Logger<FilterService>(new LoggerFactory());
            _filterService = new FilterService(_logger);
        }

        [Fact]
        public void TestFilterService()
        {
            string jsonPath = "C:\\Users\\997588\\source\\repos\\FlightsDiggingApp\\Tests\\FilterServiceTestData.json"; // Ensure this file exists in your app directory
            string jsonContent = File.ReadAllText(jsonPath);

            GetRoundtripsResponseDTO getRoundtripsResponseDTO = JsonSerializer.Deserialize<GetRoundtripsResponseDTO>(jsonContent);

            var expectedMaxDuration = 16;
            var expectedMaxFlights = 15;
            var expectedMaxPrice = 10000;

            var filter = new Filter
            {
                maxDuration = expectedMaxDuration,
                maxFlights = expectedMaxFlights,
            };

            getRoundtripsResponseDTO = _filterService.FilterFlightsFromGetRoundtripsResponseDTO(filter, getRoundtripsResponseDTO);

            getRoundtripsResponseDTO.data.flights.ForEach(f => Console.WriteLine("WRITE LINE: >>>>>>>>>" + f.score));
            
            
            Assert.Equal(expectedMaxFlights, getRoundtripsResponseDTO.data.flights.Count);

            // Expect all flights to not pass the max duration:

            Assert.True(getRoundtripsResponseDTO.data.flights.All(f => f.hours <= expectedMaxDuration));
        }
    }
}