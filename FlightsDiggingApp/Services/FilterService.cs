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

        public GetRoundtripsResponseDTO FilterFlightsFromGetRoundtripsResponseDTO(Filter filter,GetRoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            if (filter == null)
            {
                return getRoundtripsResponseDTO;
            } 
            if (filter.maxPrice != 0)
            {
                FilterByMaxPrice(filter.maxPrice, getRoundtripsResponseDTO);
            }
            if (filter.minPrice != 0)
            {
                FilterByMinPrice(filter.minPrice, getRoundtripsResponseDTO);
            }
            if (filter.minDuration != 0)
            {
                FilterByMinDuration(filter.minDuration, getRoundtripsResponseDTO);
            }
            if (filter.maxDuration != 0)
            {
                FilterByMaxDuration(filter.maxDuration, getRoundtripsResponseDTO);
            }
            if (filter.maxStops != 0)
            {
                FilterByMaxStops(filter.maxStops, getRoundtripsResponseDTO);
            }

            SortByScore(getRoundtripsResponseDTO);

            FilterByMaxFlights(filter.maxFlights, getRoundtripsResponseDTO);

            // Returns filtered content
            return getRoundtripsResponseDTO;
        }

        private void SortByScore(GetRoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.OrderByDescending(flight => flight.score).ToList();
        }

        private void FilterByMaxFlights(int maxFlights, GetRoundtripsResponseDTO getRoundtripsResponseDTO)
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

        private void FilterByMaxStops(int maxStops, GetRoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.stops <= maxStops).ToList();
        }

        private void FilterByMaxDuration(int maxDuration, GetRoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.hours <= maxDuration).ToList();
        }

        private void FilterByMinDuration(int minDuration, GetRoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.hours >= minDuration).ToList();
        }

        private void FilterByMinPrice(double minPrice, GetRoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.rawPrice >= minPrice).ToList();
        }

        private void FilterByMaxPrice(double maxPrice, GetRoundtripsResponseDTO getRoundtripsResponseDTO)
        {
           getRoundtripsResponseDTO.data.flights = getRoundtripsResponseDTO.data.flights.Where(flight => flight.rawPrice <= maxPrice).ToList();
        }
    }
}
