using FlightsDiggingApp.Mappers;
using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public class FilterService : IFilterService
    {
        ILogger<FilterService> _logger;
        public FilterService(ILogger<FilterService> logger)
        {
            _logger = logger;
        }

        public RoundtripsResponseDTO FilterFlightsFromGetRoundtripsResponseDTO(Filter filter,RoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            var filteredResponse = RoundtripsMapper.CreateCopyOfGetRoundtripsResponseDTO(getRoundtripsResponseDTO);

            if (filter == null)
            {
                return new RoundtripsResponseDTO { status = OperationStatus.CreateStatusFailure("Could not apply filter, because Filter object is null!")};
            } 
            if (filter.maxPrice != 0)
            {
                FilterByMaxPrice(filter.maxPrice, filteredResponse);
            }
            if (filter.minPrice != 0)
            {
                FilterByMinPrice(filter.minPrice, filteredResponse);
            }
            if (filter.minDuration != 0)
            {
                FilterByMinDuration(filter.minDuration, filteredResponse);
            }
            if (filter.maxDuration != 0)
            {
                FilterByMaxDuration(filter.maxDuration, filteredResponse);
            }
            if (filter.maxStops != 0)
            {
                FilterByMaxStops(filter.maxStops, filteredResponse);
            }

            SortByScore(filteredResponse);

            FilterByMaxFlights(filter.maxFlights, filteredResponse);

            ApplyMetrics(filteredResponse);

            // Returns filtered content
            return filteredResponse;
        }

        private void ApplyMetrics(RoundtripsResponseDTO filteredResponse)
        {
            filteredResponse.metrics = new RoundTripsMetrics() { 
                totalFlights = filteredResponse.data.flights.Count,
                maxHours = filteredResponse.data.flights.Max(flight => flight.hours),
                minHours = filteredResponse.data.flights.Min(flight => flight.hours),
                maxPrice = filteredResponse.data.flights.Max(flight => flight.rawPrice),
                minPrice = filteredResponse.data.flights.Min(flight => flight.rawPrice),
                maxScore = filteredResponse.data.flights.Max(flight => flight.score),
                minScore = filteredResponse.data.flights.Min(flight => flight.score)
            };
        }

        private void SortByScore(RoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.OrderByDescending(flight => flight.score).ToList();
        }

        private void FilterByMaxFlights(int maxFlights, RoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            int maxFlightsCap = 100;
            // If maxFlights is 0 or larger than the limit, set it to the limit
            maxFlights = (maxFlights == 0 || maxFlights > maxFlightsCap) ? maxFlightsCap : maxFlights;

            // Take only the maxFlights number and remove exceeding:
            if (getRoundtripsResponseDTO.data.flights.Count > maxFlights)
            {
                getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Take(maxFlights).ToList();
            }
        }

        private void FilterByMaxStops(int maxStops, RoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.stops <= maxStops).ToList();
        }

        private void FilterByMaxDuration(int maxDuration, RoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.hours <= maxDuration).ToList();
        }

        private void FilterByMinDuration(int minDuration, RoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.hours >= minDuration).ToList();
        }

        private void FilterByMinPrice(double minPrice, RoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.rawPrice >= minPrice).ToList();
        }

        private void FilterByMaxPrice(double maxPrice, RoundtripsResponseDTO getRoundtripsResponseDTO)
        {
           getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.rawPrice <= maxPrice).ToList();
        }
    }
}
