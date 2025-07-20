using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services.Filters
{
    public class FilterOrdenator
    {
        internal static void OrderByMaxPrice(RoundtripResponseDTO dto)
        {
            if (dto?.data == null) return;
            dto.data = dto.data.OrderByDescending(x => x.price.total).ToList();
        }
        internal static void OrderByMinPrice(RoundtripResponseDTO dto)
        {
            if (dto?.data == null) return;
            dto.data = dto.data.OrderBy(x => x.price.total).ToList();
        }
        internal static void OrderByMaxDuration(RoundtripResponseDTO dto)
        {
            if (dto?.data == null) return;
            dto.data = dto.data.OrderBy(x => x.durationStatsMinutes.max).ToList();
        }
        internal static void OrderByMaxStops(RoundtripResponseDTO dto)
        {
            if (dto?.data == null) return;
            dto.data = dto.data.OrderBy(x => x.maxStops).ToList();
        }

        internal static void OrderByDepTimeOrigin(RoundtripResponseDTO dto)
        {
            if (dto?.data == null) return;
            dto.data = dto.data
                .Where(x => x.departureFlight?.segments?.Count > 0)
                .OrderBy(x => x.departureFlight.segments[0].departure.at).ToList();
        }

        internal static void OrderByDepTimeReturn(RoundtripResponseDTO dto)
        {
            if (dto?.data == null) return;
            dto.data = dto.data
                .Where(x => x.returnFlight?.segments?.Count > 0)
                .OrderBy(x => x.returnFlight.segments[0].departure.at).ToList();
        }
    }
}
