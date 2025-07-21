using System.Linq;
using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services.Filters.Helpers
{
    public class FilterFixer
    {
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
            var shortestPrice = filteredResponseDTO.data.Min(f => f.price.total);
            var largestPrice = filteredResponseDTO.data.Max(f => f.price.total);
            if (filter.maxPrice < shortestPrice)
            {
                filter.maxPrice = shortestPrice;
            } else if (filter.maxPrice > largestPrice)
            {
                filter.maxPrice = largestPrice;
            }
        }
      
        public static void FixFilterRangeDepTimeOriginMin(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            filter.departureTimeOriginMinutes.min = 
                GetTimeWithinRange(filter.departureTimeOriginMinutes.min, filteredResponseDTO, (rt) => rt.departureFlight);
        }
        public static void FixFilterRangeDepTimeOriginMax(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            filter.departureTimeOriginMinutes.max =
                GetTimeWithinRange(filter.departureTimeOriginMinutes.max, filteredResponseDTO, (rt) => rt.departureFlight);
        }
        
        public static void FixFilterRangeDepTimeReturnMin(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            filter.departureTimeReturnMinutes.min =
               GetTimeWithinRange(filter.departureTimeReturnMinutes.min, filteredResponseDTO, (rt) => rt.returnFlight);
        }
        public static void FixFilterRangeDepTimeReturnMax(Filter filter, RoundtripResponseDTO filteredResponseDTO)
        {
            filter.departureTimeReturnMinutes.max =
              GetTimeWithinRange(filter.departureTimeReturnMinutes.max, filteredResponseDTO, (rt) => rt.returnFlight);
        }
        private static int GetTimeWithinRange(int selectedMinutes, RoundtripResponseDTO filteredResponseDTO, Func<RoundTripDTO, FlightDTO> flightSelector)
        {
            var smallestDepTimeReturn = filteredResponseDTO.data
                .Where(f=>flightSelector(f).segments.Count > 0)
                .Min(f => flightSelector(f).segments[0].departure.at);
            var smallestDepTimeReturnMinutes = smallestDepTimeReturn.Hour * 60 + smallestDepTimeReturn.Minute;

            var largestDepTimeReturn = filteredResponseDTO.data
                .Where(f => flightSelector(f).segments.Count > 0)
                .Max(f => flightSelector(f).segments[0].departure.at);
            var largestDepTimeReturnMinutes = largestDepTimeReturn.Hour * 60 + largestDepTimeReturn.Minute;

            if (selectedMinutes < smallestDepTimeReturnMinutes)
            {
                return smallestDepTimeReturnMinutes;
            }
            if (selectedMinutes > largestDepTimeReturnMinutes)
            {
                return largestDepTimeReturnMinutes;
            }
            return selectedMinutes;
        }
    }
}
