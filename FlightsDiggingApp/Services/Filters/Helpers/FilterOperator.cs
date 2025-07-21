using FlightsDiggingApp.Models;
using FlightsDiggingApp.Properties;

namespace FlightsDiggingApp.Services.Filters.Helpers
{
    public class FilterOperator
    {
        public static void FilterByDepHourReturnMin(MinMax<int> departureTimeReturnMinutes, RoundtripResponseDTO filteredResponseDTO)
        {
            filteredResponseDTO.data = filteredResponseDTO.data
                .Where(rt => IsFlightWithinDepartureRangeMin(rt.returnFlight, departureTimeReturnMinutes))
                .ToList();
        }
        public static void FilterByDepHourReturnMax(MinMax<int> departureTimeReturnMinutes, RoundtripResponseDTO filteredResponseDTO)
        {
            filteredResponseDTO.data = filteredResponseDTO.data
                .Where(rt => IsFlightWithinDepartureRangeMax(rt.returnFlight, departureTimeReturnMinutes))
                .ToList();
        }
        public static void FilterByDepHourOriginMin(MinMax<int> departureTimeOriginMinutes, RoundtripResponseDTO filteredResponseDTO)
        {
            filteredResponseDTO.data = filteredResponseDTO.data
                .Where(rt => IsFlightWithinDepartureRangeMin(rt.departureFlight, departureTimeOriginMinutes))
                .ToList();
        }
        public static void FilterByDepHourOriginMax(MinMax<int> departureTimeOriginMinutes, RoundtripResponseDTO filteredResponseDTO)
        {
            filteredResponseDTO.data = filteredResponseDTO.data
                .Where(rt => IsFlightWithinDepartureRangeMax(rt.departureFlight, departureTimeOriginMinutes))
                .ToList();
        }

        private static bool IsFlightWithinDepartureRangeMin(FlightDTO flight, MinMax<int> departureHourReturn)
        {
            var segments = flight.segments;
            if (segments == null || segments.Count == 0)
                return false;

            int minMinutes = departureHourReturn.min;

            var departureDateTime = segments[0].departure.at;
            int departureTotalMinutes = departureDateTime.Hour * 60 + departureDateTime.Minute;

            return departureTotalMinutes >= minMinutes;
        }
        private static bool IsFlightWithinDepartureRangeMax(FlightDTO flight, MinMax<int> departureHourReturn)
        {
            var segments = flight.segments;
            if (segments == null || segments.Count == 0)
                return false;

            int maxMinutes = departureHourReturn.max > 0 ? departureHourReturn.max : 24 * 60;

            var departureDateTime = segments[0].departure.at;
            int departureTotalMinutes = departureDateTime.Hour * 60 + departureDateTime.Minute;

            return departureTotalMinutes <= maxMinutes;
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

        public static void FilterByMaxPrice(double maxPrice, RoundtripResponseDTO roundtripResponseDTO)
        {
            roundtripResponseDTO.data = roundtripResponseDTO.data.Where(flight => flight.price.total <= maxPrice).ToList();
        }
       
    }
}
