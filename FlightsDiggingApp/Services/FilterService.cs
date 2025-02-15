using System.Net;
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

        public RoundtripResponseDTO FilterRoundtripResponseDTO(Filter filter, RoundtripResponseDTO responseDTO)
        {

            var filteredResponseDTO = RoundtripMapper.CreateCopyOfRoundtripResponseDTO(responseDTO);
            var originalFlightsCount = responseDTO.data.Count;

            if (filter == null)
            {
                return filteredResponseDTO;
            }
            if (filter.maxPrice != 0)
            {
                FilterByMaxPrice(filter.maxPrice, filteredResponseDTO);
            }
            if (filter.minPrice != 0)
            {
                FilterByMinPrice(filter.minPrice, filteredResponseDTO);
            }
            if (filter.minDurationHours != 0)
            {
                FilterByMinDuration(filter.minDurationHours, filteredResponseDTO);
            }
            if (filter.maxDurationHours != 0)
            {
                FilterByMaxDuration(filter.maxDurationHours, filteredResponseDTO);
            }
            if (filter.maxStops != 0)
            {
                FilterByMaxStops(filter.maxStops, filteredResponseDTO);
            }

            FilterByMaxFlights(filter.maxFlights, filteredResponseDTO);

            ApplyMetrics(filteredResponseDTO, originalFlightsCount);

            // Returns filtered content
            return filteredResponseDTO;
        }

        private void ApplyMetrics(RoundtripResponseDTO filteredResponseDTO, int originalFlightsCount)
        {
            filteredResponseDTO.metrics = new RoundTripMetrics() {
                totalFlightsFiltered = filteredResponseDTO.data.Count,
                totalFlightsOriginal = originalFlightsCount,
                maxHours = filteredResponseDTO.data.Max(flight => flight.duration.hours),
                minHours = filteredResponseDTO.data.Min(flight => flight.duration.hours),
                maxPrice = filteredResponseDTO.data.Max(flight => flight.price.total),
                minPrice = filteredResponseDTO.data.Min(flight => flight.price.total),
            };
        }

        private void FilterByMaxFlights(int maxFlights, RoundtripResponseDTO roundtripResponseDTO)
        {
            int maxFlightsCap = 100;
            // If maxFlights is 0 or larger than the limit, set it to the limit
            maxFlights = (maxFlights == 0 || maxFlights > maxFlightsCap) ? maxFlightsCap : maxFlights;

            // Take only the maxFlights number and remove exceeding:
            if (roundtripResponseDTO.data.Count > maxFlights)
            {
                roundtripResponseDTO.data = roundtripResponseDTO.data.Take(maxFlights).ToList();
            }
        }

        private void FilterByMaxStops(int maxStops, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.stops <= maxStops).ToList();
        }

        private void FilterByMaxDuration(int maxDuration, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.duration.hours <= maxDuration).ToList();
        }

        private void FilterByMinDuration(int minDuration, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.duration.hours >= minDuration).ToList();
        }

        private void FilterByMinPrice(double minPrice, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.price.total >= minPrice).ToList();
        }

        private void FilterByMaxPrice(double maxPrice, RoundtripResponseDTO roundtripResponseDTO)
        {
           roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.price.total <= maxPrice).ToList();
        }

       
    }
}
