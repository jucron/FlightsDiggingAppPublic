namespace FlightsDiggingApp.Models
{
    public class RoundTripMetrics
    {
        public Metrics originalMetrics { get; set; }
        public Metrics filteredMetrics { get; set; }

        public class Metrics
        {
            public int totalFlights { get; set; }
            public double maxPrice { get; set; }
            public double minPrice { get; set; }
            public int maxStops { get; set; } 
            public MinMax<int> flightsDurationMinutes { get; set; }
            public MinMax<int> departureTimeOriginMinutes { get; set; }
            public MinMax<int> departureTimeReturnMinutes { get; set; }
        }
    }
}
