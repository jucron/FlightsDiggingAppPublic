using System.Net;
using FlightsDiggingApp.Mappers;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Properties;
using FlightsDiggingApp.Services.Filters.Helpers;
using Microsoft.Extensions.Options;
using static FlightsDiggingApp.Models.Filter;

namespace FlightsDiggingApp.Services.Filters
{
    public class FilterService : IFilterService
    {
        private readonly ILogger<FilterService> _logger;

        private readonly int _maxRoundTrips;
        private readonly List<FilterRule> _rules;
        public FilterService(ILogger<FilterService> logger, IOptions<AmadeusApiProperties> amadeusApiProperties)
        {
            _logger = logger;
            _maxRoundTrips = amadeusApiProperties.Value.limit_roundtrip_flights;
            _rules = FilterRule.BuildRules();
        }

        public RoundtripResponseDTO FilterRoundtripResponseDTO(Filter filter, RoundtripResponseDTO responseDTO)
        {

            var filteredResponseDTO = RoundtripMapper.CreateCopyOfRoundtripResponseDTO(responseDTO);

            if (filter == null)
                return filteredResponseDTO;

            static bool hasAnyData(RoundtripResponseDTO rt) => rt?.data?.Count > 0;

            if (filter.selectedFilter != FilterType.None)
            {
                FilterRule? selectedRule = _rules
                    .FirstOrDefault(r => r.Type == filter.selectedFilter && r.Condition(filter));

                if (hasAnyData(filteredResponseDTO))
                {
                    // Fix the range, because the filter might start out of range
                    selectedRule?.FixFilterRange(filter, filteredResponseDTO);
                    // ApplyFilter selected field first if any
                    selectedRule?.ApplyFilter(filter, filteredResponseDTO);

                    // Order by selected type before reduzing size
                    selectedRule?.OrderBySelectedType(filteredResponseDTO);
                }
            }

            
            // ApplyFilter remaining filters in priority order, excluding the selected one
            // Filters are fixed within the new limits after priority filtering
            foreach (var rule in _rules
                .Where(r => r.Type != filter.selectedFilter && r.Condition(filter))
                .OrderBy(r => r.Priority))
            {
                if (!hasAnyData(filteredResponseDTO)) { break; }

                rule.FixFilterRange(filter, filteredResponseDTO);
                rule.ApplyFilter(filter, filteredResponseDTO);
            }

            if (hasAnyData(filteredResponseDTO))
            {
                // Reducing data size
                FilterOperator.FilterByMaxRoundTrips(filter.maxRoundTrips, _maxRoundTrips, filteredResponseDTO);

                // Order from min to max price before sending to frond
                FilterOrdenator.OrderByMinPrice(filteredResponseDTO);
            }

            bool isFiltered = true;
            ApplyMetrics(filteredResponseDTO, isFiltered);

            // Returns filtered content
            return filteredResponseDTO;
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
                    min = GetMinDurationFromLongestFlights(data),
                    max = GetMaxDurationFromLongestFlights(data)
                },
                maxPrice = data.Max(roundTrip => roundTrip.price.total),
                minPrice = data.Min(roundTrip => roundTrip.price.total),
                maxStops = data.Max(roundTrip => roundTrip.maxStops),
                departureTimeOriginMinutes = CalculateOriginTimeMinutes(data),
                departureTimeReturnMinutes = CalculateReturnTimeMinutes(data)
            };
        }

        private static int GetMinDurationFromLongestFlights(List<RoundTripDTO> data)
        {
            var minOrigin = data.Where(rt => rt.departureFlight.segments?.Count > 0)
                                    .Min(rt => rt.durationStatsMinutes.max);
            var minReturn = data.Where(rt => rt.returnFlight.segments?.Count > 0)
                                    .Min(rt => rt.durationStatsMinutes.max);
            return Math.Min(minOrigin, minReturn);
        }
        private static int GetMaxDurationFromLongestFlights(List<RoundTripDTO> data)
        {
            var minOrigin = data.Where(rt => rt.departureFlight.segments?.Count > 0)
                                    .Max(rt => rt.durationStatsMinutes.max);
            var minReturn = data.Where(rt => rt.returnFlight.segments?.Count > 0)
                                    .Max(rt => rt.durationStatsMinutes.max);
            return Math.Max(minOrigin, minReturn);
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

      

       
    }
}
