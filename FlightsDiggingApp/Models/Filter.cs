namespace FlightsDiggingApp.Models
{
    public class Filter
    {
        public double maxPrice { get; set; }
        public int maxDurationMinutes { get; set; }
        public int maxStops { get; set; }
        public int maxRoundTrips { get; set; }
        public MinMax<int> departureTimeOriginMinutes { get; set; }
        public MinMax<int> departureTimeReturnMinutes { get; set; }
        public FilterType selectedFilter { get; set; }

    }
    public enum FilterType
    {
        None,
        MaxPrice,
        Duration,
        Stops,
        RoundTripsMax,
        DepartureTimeOriginMinutesMin,
        DepartureTimeOriginMinutesMax,
        DepartureTimeReturnMinutesMin,
        DepartureTimeReturnMinutesMax
    }
}
