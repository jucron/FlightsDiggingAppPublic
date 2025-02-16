using System.Net;
using FlightsDiggingApp.Mappers;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Properties;
using Microsoft.Extensions.Options;

namespace FlightsDiggingApp.Services
{
    public class FilterService : IFilterService
    {
        private readonly ILogger<FilterService> _logger;
        private readonly AmadeusApiProperties _amadeusApiProperties;
        public FilterService(ILogger<FilterService> logger, IOptions<AmadeusApiProperties> amadeusApiProperties)
        {
            _logger = logger;
            _amadeusApiProperties = amadeusApiProperties.Value;
        }

        public RoundtripResponseDTO FilterRoundtripResponseDTO(Filter filter, RoundtripResponseDTO responseDTO)
        {

            var filteredResponseDTO = RoundtripMapper.CreateCopyOfRoundtripResponseDTO(responseDTO);

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

            bool isFiltered = true;
            ApplyMetrics(filteredResponseDTO, isFiltered);

            // Returns filtered content
            return filteredResponseDTO;
        }

        public void ApplyMetrics(RoundtripResponseDTO responseDTO, bool isFiltered = false)
        {
            responseDTO.metrics ??= new RoundTripMetrics();

            if (isFiltered)
            {
                responseDTO.metrics.filteredMetrics = new RoundTripMetrics.Metrics()
                {
                    totalFlights = responseDTO.data.Count,
                    maxHours = responseDTO.data.Any() ? responseDTO.data.Max(flight => flight.duration.hours) : 0,
                    minHours = responseDTO.data.Any() ? responseDTO.data.Min(flight => flight.duration.hours) : 0,
                    maxPrice = responseDTO.data.Any() ? responseDTO.data.Max(flight => flight.price.total) : 0,
                    minPrice = responseDTO.data.Any() ? responseDTO.data.Min(flight => flight.price.total) : 0,
                };
            }
            else
            {
                responseDTO.metrics.originalMetrics = new RoundTripMetrics.Metrics()
                {
                    totalFlights = responseDTO.data.Count,
                    maxHours = responseDTO.data.Any() ? responseDTO.data.Max(flight => flight.duration.hours) : 0,
                    minHours = responseDTO.data.Any() ? responseDTO.data.Min(flight => flight.duration.hours) : 0,
                    maxPrice = responseDTO.data.Any() ? responseDTO.data.Max(flight => flight.price.total) : 0,
                    minPrice = responseDTO.data.Any() ? responseDTO.data.Min(flight => flight.price.total) : 0,
                };
            }
        
        }

        private void FilterByMaxFlights(int maxFlights, RoundtripResponseDTO roundtripResponseDTO)
        {
            int maxFlightsCap = _amadeusApiProperties.limit_roundtrip_flights;
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
