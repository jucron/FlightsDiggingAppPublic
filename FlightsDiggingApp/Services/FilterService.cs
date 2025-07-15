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
            if (filter.maxPrice > 0)
            {
                FilterByMaxPrice(filter.maxPrice, filteredResponseDTO);
            }
            if (filter.minPrice > 0)
            {
                FilterByMinPrice(filter.minPrice, filteredResponseDTO);
            }
            if (filter.maxDurationMinutes > 0)
            {
                FilterByMaxDuration(filter.maxDurationMinutes, filteredResponseDTO);
            }
            if (filter.maxStops > 0)
            {
                FilterByMaxStops(filter.maxStops, filteredResponseDTO);
            }
            static bool IsMinMaxRelevant(MinMax<int> minMax) => (minMax.min > 0 || minMax.max > 0);

            if (IsMinMaxRelevant(filter.departureTimeOriginMinutes))
            {
                FilterByDepHourOrigin(filter.departureTimeOriginMinutes, filteredResponseDTO);
            }
            if (IsMinMaxRelevant(filter.departureTimeReturnMinutes))
            {
                FilterByDepHourReturn(filter.departureTimeReturnMinutes, filteredResponseDTO);
            }

            FilterByMaxRoundTrips(filter.maxRoundTrips, filteredResponseDTO);

            bool isFiltered = true;
            ApplyMetrics(filteredResponseDTO, isFiltered);

            // Returns filtered content
            return filteredResponseDTO;
        }

        private void FilterByDepHourReturn(MinMax<int> departureTimeReturnMinutes, RoundtripResponseDTO filteredResponseDTO)
        {
            filteredResponseDTO.data = filteredResponseDTO.data
                .Where(rt => IsFlightWithinDepartureRange(rt.returnFlight, departureTimeReturnMinutes))
                .ToList();
        }
        private void FilterByDepHourOrigin(MinMax<int> departureTimeOriginMinutes, RoundtripResponseDTO filteredResponseDTO)
        {
            filteredResponseDTO.data = filteredResponseDTO.data
                .Where(rt => IsFlightWithinDepartureRange(rt.departureFlight, departureTimeOriginMinutes))
                .ToList();
        }

        private bool IsFlightWithinDepartureRange(FlightDTO flight, MinMax<int> departureHourReturn)
        {
            var segments = flight.segments;
            if (segments == null || segments.Count == 0)
                return false;

            int maxMinutes = (departureHourReturn.max > 0) ? departureHourReturn.max : 24 * 60;
            int minMinutes = departureHourReturn.min;

            if (minMinutes > maxMinutes) //defensive check
                return false;

            var departureDateTime = segments[0].departure.at;
            int departureTotalMinutes = departureDateTime.Hour * 60 + departureDateTime.Minute;

            return departureTotalMinutes >= minMinutes &&
                   departureTotalMinutes <= maxMinutes;
        }


        public void ApplyMetrics(RoundtripResponseDTO responseDTO, bool isFiltered = false)
        {
            responseDTO.metrics ??= new RoundTripMetrics();
            var hasData = responseDTO?.data?.Count > 0;

            if (hasData)
            {
                if (isFiltered)
                {
                    responseDTO.metrics.filteredMetrics = CalculateMetrics(responseDTO.data);
                }
                else
                {
                    responseDTO.metrics.originalMetrics = CalculateMetrics(responseDTO.data);
                }
            }
            else
            {
                responseDTO.metrics.filteredMetrics = new RoundTripMetrics.Metrics()
                {
                    totalFlights = 0,
                    flightsDurationMinutes = new MinMax<int>{ min = 0, max = 0 },
                    maxPrice = 0,
                    minPrice = 0,
                    maxStops = 0,
                };
            }
        }

        private static RoundTripMetrics.Metrics CalculateMetrics(List<RoundTripDTO> data)
        {
            return new RoundTripMetrics.Metrics()
            {
                totalFlights = data.Count,
                flightsDurationMinutes = new MinMax<int>()
                {
                    min = data.Where(rt => rt.departureFlight.segments?.Count > 0)
                                    .Min(rt => (rt.durationStatsMinutes.min)),
                    max = data.Where(rt => rt.departureFlight.segments?.Count > 0)
                                    .Max(rt => (rt.durationStatsMinutes.max))
                },
                maxPrice = data.Max(roundTrip => roundTrip.price.total),
                minPrice = data.Min(roundTrip => roundTrip.price.total),
                maxStops = data.Max(roundTrip => roundTrip.maxStops),
                departureTimeOriginMinutes = CalculateOriginTimeMinutes(data),
                departureTimeReturnMinutes = CalculateReturnTimeMinutes(data)
            };
        }

        private static MinMax<int> CalculateReturnTimeMinutes(List<RoundTripDTO> data)
        {
            var validReturnSegments = data
                .Where(rt => rt.returnFlight?.segments != null && rt.returnFlight.segments.Count > 0)
                .Select(rt => rt.returnFlight.segments[0].departure.at)
                .ToList();

            return CreateMinMaxMinutes(validReturnSegments);
        }

        private static MinMax<int> CalculateOriginTimeMinutes(List<RoundTripDTO> data)
        {
            var validReturnSegments = data
                .Where(rt => rt.departureFlight?.segments != null && rt.departureFlight.segments.Count > 0)
                .Select(rt => rt.departureFlight.segments[0].departure.at)
                .ToList();

            return CreateMinMaxMinutes(validReturnSegments);
        }
        private static MinMax<int> CreateMinMaxMinutes(List<DateTime> validSegments)
        {
            if (validSegments == null ||  validSegments.Count == 0)
            {
                // Handle edge case: no valid return segments
                return new MinMax<int>
                {
                    min = 0,
                    max = 0
                };
            }

            var earliestFlight = validSegments.Min();
            var latestFlight = validSegments.Max();

            static int ToTotalMinutes(DateTime dt) => dt.Hour * 60 + dt.Minute;

            return new MinMax<int>
            {
                min = ToTotalMinutes(earliestFlight),
                max = ToTotalMinutes(latestFlight)
            };
        }

        private void FilterByMaxRoundTrips(int maxFlights, RoundtripResponseDTO roundtripResponseDTO)
        {
            if (roundtripResponseDTO?.data == null)
                return;

            int maxFlightsCap = _amadeusApiProperties.limit_roundtrip_flights;
            int flightsToTake = (maxFlights == 0) ? maxFlightsCap : Math.Min(maxFlights, maxFlightsCap);

            roundtripResponseDTO.data = roundtripResponseDTO.data
                .Take(flightsToTake)
                .ToList();
        }

        private void FilterByMaxStops(int maxStops, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(roundTrip => roundTrip.maxStops <= maxStops).ToList();
        }

        private void FilterByMaxDuration(int maxDurationMinutes, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data
                .Where(flight => (flight.durationStatsMinutes.max <= maxDurationMinutes)).ToList();
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
