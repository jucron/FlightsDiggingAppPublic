using System.Net;
using FlightsDiggingApp.Mappers;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Properties;
using Microsoft.Extensions.Options;
using static FlightsDiggingApp.Models.Filter;

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
            if (filter.maxDurationHours != 0)
            {
                FilterByMaxDuration(filter.maxDurationHours, filteredResponseDTO);
            }
            if (filter.maxStops != 0)
            {
                FilterByMaxStops(filter.maxStops, filteredResponseDTO);
            }
            if (filter.departureHourOrigin != null)
            {
                FilterByDepHourOrigin(filter.departureHourOrigin, filteredResponseDTO);
            }
            if (filter.departureHourReturn != null)
            {
                FilterByDepHourReturn(filter.departureHourReturn, filteredResponseDTO);
            }

            FilterByMaxRoundTrips(filter.maxRoundTrips, filteredResponseDTO);

            bool isFiltered = true;
            ApplyMetrics(filteredResponseDTO, isFiltered);

            // Returns filtered content
            return filteredResponseDTO;
        }

        private void FilterByDepHourReturn(MinMaxHours departureHourReturn, RoundtripResponseDTO filteredResponseDTO)
        {
            int originalMaxHour = departureHourReturn.max;
            int maxHour = (originalMaxHour > 0) ? originalMaxHour : 24;
            filteredResponseDTO.data = [.. filteredResponseDTO.data.Where(roundTrip =>
            {
                var departureDateTime = roundTrip.returnFlight.segments[0].departure.at;
                var departureHour = departureDateTime.Hour;

                return departureHour >= departureHourReturn.min && departureHour <= maxHour;
            })];
        }

        private void FilterByDepHourOrigin(MinMaxHours departureHourOrigin, RoundtripResponseDTO filteredResponseDTO)
        {
            int originalMaxHour = departureHourOrigin.max;
            int maxHour = (originalMaxHour > 0) ? originalMaxHour : 24;
            filteredResponseDTO.data = [.. filteredResponseDTO.data.Where(roundTrip =>
            {
                var departureDateTime = roundTrip.departureFlight.segments[0].departure.at;
                var departureHour = departureDateTime.Hour;

                return departureHour >= departureHourOrigin.min && departureHour <= maxHour;
            })];
        }

        public void ApplyMetrics(RoundtripResponseDTO responseDTO, bool isFiltered = false)
        {
            responseDTO.metrics ??= new RoundTripMetrics();
            var hasData = responseDTO.data.Count > 0;

            if (hasData)
            {
                if (isFiltered)
                {
                    responseDTO.metrics.filteredMetrics = CalculateMetrics(responseDTO.data);
                }
                else
                {
                    responseDTO.metrics.originalMetrics = CalculateMetrics(responseDTO.data);
                    responseDTO.metrics.filteredMetrics = responseDTO.metrics.originalMetrics;
                }
            }
            else
            {
                responseDTO.metrics.filteredMetrics = new RoundTripMetrics.Metrics()
                {
                    totalFlights = 0,
                    totalDuration = new MinMaxHours{ min = 0, max = 0 },
                    maxPrice = 0,
                    minPrice = 0,
                    departureHourOrigin = new MinMaxHours{ min = 0, max = 0 },
                };
            }
        }

        private static RoundTripMetrics.Metrics CalculateMetrics(List<RoundTripDTO> data)
        {
            return new RoundTripMetrics.Metrics()
            {
                totalFlights = data.Count,
                totalDuration = new MinMaxHours()
                        {
                    //todo: check what is happening to duration hours wrong!
                            min = data.Min(roundTrip => roundTrip.totalDuration.hours),
                            max = data.Max(roundTrip => roundTrip.totalDuration.hours),
                        },
                maxPrice = data.Max(roundTrip => roundTrip.price.total),
                minPrice = data.Min(roundTrip => roundTrip.price.total),
                maxStops = data.Max(roundTrip => roundTrip.maxStops),
                departureHourOrigin = new MinMaxHours
                {
                    min = data.Min(roundTrip => roundTrip.departureFlight.segments[0].departure.at.Hour),
                    max = data.Max(roundTrip => roundTrip.departureFlight.segments[0].departure.at.Hour)
                },
                departureHourReturn = new MinMaxHours
                {
                    min = data.Min(roundTrip => roundTrip.returnFlight.segments[0].departure.at.Hour),
                    max = data.Max(roundTrip => roundTrip.returnFlight.segments[0].departure.at.Hour)
                }
            };
        }

        private void FilterByMaxRoundTrips(int maxFlights, RoundtripResponseDTO roundtripResponseDTO)
        {
            int maxFlightsCap = _amadeusApiProperties.limit_roundtrip_flights;
            // If maxRoundTrips is 0 or larger than the limit, set it to the limit
            maxFlights = (maxFlights == 0 || maxFlights > maxFlightsCap) ? maxFlightsCap : maxFlights;

            // Take only the maxRoundTrips number and remove exceeding:
            if (roundtripResponseDTO.data.Count > maxFlights)
            {
                roundtripResponseDTO.data = roundtripResponseDTO.data.Take(maxFlights).ToList();
            }
        }

        private void FilterByMaxStops(int maxStops, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(roundTrip => roundTrip.maxStops <= maxStops).ToList();
        }

        private void FilterByMaxDuration(int maxDuration, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.totalDuration.hours <= maxDuration).ToList();
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
