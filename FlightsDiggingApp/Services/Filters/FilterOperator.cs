using FlightsDiggingApp.Models;
using FlightsDiggingApp.Properties;

namespace FlightsDiggingApp.Services.Filters
{
    public class FilterOperator
    {
        public static void FilterByDepHourReturn(MinMax<int> departureTimeReturnMinutes, RoundtripResponseDTO filteredResponseDTO)
        {
            filteredResponseDTO.data = filteredResponseDTO.data
                .Where(rt => IsFlightWithinDepartureRange(rt.returnFlight, departureTimeReturnMinutes))
                .ToList();
        }
        public static void FilterByDepHourOrigin(MinMax<int> departureTimeOriginMinutes, RoundtripResponseDTO filteredResponseDTO)
        {
            filteredResponseDTO.data = filteredResponseDTO.data
                .Where(rt => IsFlightWithinDepartureRange(rt.departureFlight, departureTimeOriginMinutes))
                .ToList();
        }

        private static bool IsFlightWithinDepartureRange(FlightDTO flight, MinMax<int> departureHourReturn)
        {
            var segments = flight.segments;
            if (segments == null || segments.Count == 0)
                return false;

            int maxMinutes = departureHourReturn.max > 0 ? departureHourReturn.max : 24 * 60;
            int minMinutes = departureHourReturn.min;

            if (minMinutes > maxMinutes) //defensive check
                return false;

            var departureDateTime = segments[0].departure.at;
            int departureTotalMinutes = departureDateTime.Hour * 60 + departureDateTime.Minute;

            return departureTotalMinutes >= minMinutes &&
                   departureTotalMinutes <= maxMinutes;
        }
        public static void FilterByMaxRoundTrips(int maxFlights, int maxFlightsCap, RoundtripResponseDTO roundtripResponseDTO)
        {
            if (roundtripResponseDTO?.data == null)
                return;

            int flightsToTake = maxFlights == 0 ? maxFlightsCap : Math.Min(maxFlights, maxFlightsCap);

            roundtripResponseDTO.data = roundtripResponseDTO.data
                .Take(flightsToTake)
                .ToList();
        }

        public static void FilterByMaxStops(int maxStops, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(roundTrip => roundTrip.maxStops <= maxStops).ToList();
        }

        public static void FilterByMaxDuration(int maxDurationMinutes, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data
                .Where(flight => flight.durationStatsMinutes.max <= maxDurationMinutes).ToList();
        }

        public static void FilterByMinPrice(double minPrice, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.price.total >= minPrice).ToList();
        }

        public static void FilterByMaxPrice(double maxPrice, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.price.total <= maxPrice).ToList();
        }
        public static void FixFilterRangeMaxDuration(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            var longestMaxDuration = filteredResponseDTO.data.Max(f => f.durationStatsMinutes.max);
            var shortestMaxDuration = filteredResponseDTO.data.Min(f => f.durationStatsMinutes.max);
            if (filter.maxDurationMinutes < shortestMaxDuration)
            {
                filter.maxDurationMinutes = shortestMaxDuration;
            }
            else if (filter.maxDurationMinutes > longestMaxDuration)
            {
                filter.maxDurationMinutes = longestMaxDuration;
            }
        }
        public static void FixFilterRangeMaxStops(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            var largestMaxStops = filteredResponseDTO.data.Max(f => f.maxStops);
            var smallestMaxStops = filteredResponseDTO.data.Min(f => f.maxStops);

            if (filter.maxStops < smallestMaxStops)
            {
                filter.maxStops = smallestMaxStops;
            }
            else if (filter.maxStops > largestMaxStops)
            {
                filter.maxStops = largestMaxStops;
            }
        }
        public static void FixFilterRangeMaxPrice(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            var largestPrice = filteredResponseDTO.data.Max(f => f.price.total);
            if (filter.maxPrice > largestPrice)
            {
                filter.maxPrice = largestPrice;
            }
        }
        public static void FixFilterRangeMinPrice(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            var smallestPrice = filteredResponseDTO.data.Min(f => f.price.total);
            if (filter.minPrice < smallestPrice)
            {
                filter.minPrice = smallestPrice;
            }
        }
        public static void FixFilterRangeDepTimeOrigin(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            var smallestDepTimeOrigin = filteredResponseDTO.data.Min(f => f.departureFlight.segments[0].departure.at);
            var smallestDepTimeOriginMinutes = smallestDepTimeOrigin.Hour * 60 + smallestDepTimeOrigin.Minute;

            var largestDepTimeOrigin = filteredResponseDTO.data.Max(f => f.departureFlight.segments[0].departure.at);
            var largestDepTimeOriginMinutes = largestDepTimeOrigin.Hour * 60 + largestDepTimeOrigin.Minute;

            if (filter.departureTimeOriginMinutes.min < smallestDepTimeOriginMinutes)
            {
                filter.departureTimeOriginMinutes.min = smallestDepTimeOriginMinutes;
            }
            if (filter.departureTimeOriginMinutes.max > largestDepTimeOriginMinutes)
            {
                filter.departureTimeOriginMinutes.max = largestDepTimeOriginMinutes;
            }
        }
        public static void FixFilterRangeDepTimeReturn(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            var smallestDepTimeReturn = filteredResponseDTO.data.Min(f => f.returnFlight.segments[0].departure.at);
            var smallestDepTimeReturnMinutes = smallestDepTimeReturn.Hour * 60 + smallestDepTimeReturn.Minute;

            var largestDepTimeReturn = filteredResponseDTO.data.Max(f => f.returnFlight.segments[0].departure.at);
            var largestDepTimeReturnMinutes = largestDepTimeReturn.Hour * 60 + largestDepTimeReturn.Minute;

            if (filter.departureTimeReturnMinutes.min < smallestDepTimeReturnMinutes)
            {
                filter.departureTimeReturnMinutes.min = smallestDepTimeReturnMinutes;
            }
            if (filter.departureTimeReturnMinutes.max > largestDepTimeReturnMinutes)
            {
                filter.departureTimeReturnMinutes.max = largestDepTimeReturnMinutes;
            }
        }
    }
}
